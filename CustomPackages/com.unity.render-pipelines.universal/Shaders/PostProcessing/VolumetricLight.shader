Shader "Hidden/Universal Render Pipeline/VolumetricLights"
{
    HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
        //#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

        TEXTURE2D_X(_BlitTex);
        float3 _FrustumCorners[4];
        float4x4 _MatrixHClipToWorld;

        struct Interpolators
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 ray : TEXCOORD1;
        };

        struct VertexData {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        Interpolators VertMy(VertexData input)
        {
            Interpolators output;

            UNITY_SETUP_INSTANCE_ID(input);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
            output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
            output.uv = input.uv;
            output.ray = _FrustumCorners[input.uv.x + 2 * input.uv.y];

            return output;
        }

        inline half3 GetCameraDirection(half2 uv, float depth)
        {
            //#ifndef SHADER_API_GLCORE
                half4 positionCS = half4(uv * 2 - 1, depth, 1) * LinearEyeDepth(depth, _ZBufferParams);
            //#else
            //    half4 positionCS = half4(uv * 2 - 1, depth * 2 - 1, 1) * LinearEyeDepth(depth, _ZBufferParams);
            //#endif
            return mul(_MatrixHClipToWorld, positionCS).xyz;
        }

        float2 raySphereIntersection(float3 spherePos, float sphereRadius, float3 rayPos, float3 rayDirection)
        {
            float3 sphereDirection = spherePos - rayPos;
            float tMiddle = dot(sphereDirection, rayDirection);
            float3 posMiddle = rayPos + rayDirection*tMiddle;
            float distanceSphereToTMiddle = length(spherePos - posMiddle);

            if (distanceSphereToTMiddle < sphereRadius)
            {
                float distancePosMiddleToSphereEdge = sqrt(sphereRadius*sphereRadius - distanceSphereToTMiddle*distanceSphereToTMiddle);
                float distToVolume = tMiddle - distancePosMiddleToSphereEdge;
                float distThroughVolume = distancePosMiddleToSphereEdge * 2;
                return float2(distToVolume, distThroughVolume);
            }
            else return float2(-1, 9);
        }

        /*float2 rayConeIntersection(float3 ap_, float3 ad_, float3 coneBasePos, float3 conePointPos, float coneRadius)
        {
            float3 axis = (coneBasePos - conePointPos);
            float3 theta = (axis / length(axis));
            float m = pow(coneRadius, 2) / pow(length(axis), 2);
            float3 w = (ap_ - conePointPos);

            float a = dot(ad_, ad_) - m * (pow(dot(ad_, theta), 2)) - pow(dot(ad_,theta), 2);
            float b = 2 * (dot(ad_, w) - m * dot(ad_, theta) * dot(w, theta) - dot(ad_, theta) * dot(w, theta));
            float c = dot(w, w) - m * pow(dot(w, theta), 2) - pow(dot(w, theta), 2);

            float discriminant = pow(b, 2) - (4 * a * c);

            if (discriminant >= 0) return float2(ap_ + (-b - sqrt(Discriminant) / (2 * a)) * ad_, );
            return float2(0, 0);
        }*/

        half4 Frag(Interpolators input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            float2 uv = UnityStereoTransformScreenSpaceTex(input.uv);

            half3 color = SAMPLE_TEXTURE2D_X(_BlitTex, sampler_LinearClamp, uv).xyz;
            float depth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_LinearClamp, uv).x;
            float linearDepth = LinearEyeDepth(depth, _ZBufferParams);

            float viewDistance = depth * _ProjectionParams.z - _ProjectionParams.y;
            viewDistance = length(input.ray * Linear01Depth(depth, _ZBufferParams));

            float3 volumetricLightPositionWS = float3(0, 0, 0);
            float3 volumetricLightColor = float3(0.2, 0.2, 0.1);
            float  volumetricLightRadius = 7;
            float  volumetricLightDiameter = volumetricLightRadius * 2;

            float3 cameraDirection = normalize(GetCameraDirection(uv, depth) - GetCameraPositionWS());
            float3 volumetricLightViewDirection = volumetricLightPositionWS - GetCameraPositionWS();
            float3 volumetricLightViewDirectionNormalized = normalize(volumetricLightViewDirection);
            
            float cameraLightDot = max(0, dot(volumetricLightViewDirectionNormalized, cameraDirection));
            float2 volumeIntersection = raySphereIntersection(volumetricLightPositionWS, volumetricLightRadius, GetCameraPositionWS(), cameraDirection);
            float distToVolume = volumeIntersection.x;
            float distThroughVolume = volumeIntersection.y;
            distThroughVolume = min(distThroughVolume / 2, max(0, viewDistance - distToVolume));

            if (distToVolume > 0)
            {
                float3 volumeEdge = GetCameraPositionWS() + cameraDirection * distToVolume;
                float3 volumeEdgeToMiddleDirection = normalize(float3(0, 5, 0) - volumeEdge);
                color += (distThroughVolume / volumetricLightRadius) * pow(dot(cameraDirection, volumeEdgeToMiddleDirection), 16) * 2;
            }

            //color += (distThroughVolume / (volumetricLightDiameter));
            //color = cameraLightDot;
            //color = linearDepth / 100;
            //color = cameraDirection;
            //color = lerp(color, distThroughVolume, step(0.5, uv.x));
            //color += lerp((0).xxx, volumetricLightColor, pow(cameraLightDot, 256));
            //color += lerp((0).xxx, volumetricLightColor, step(0.9, cameraLightDot));
            //color = ray;

            return half4(color, 1.0);
        }

    ENDHLSL

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZTest Always ZWrite Off Cull Off

        Pass
        {
            Name "Atmosphere"

            HLSLPROGRAM
                #pragma vertex VertMy
                #pragma fragment Frag
            ENDHLSL
        }
    }
}
