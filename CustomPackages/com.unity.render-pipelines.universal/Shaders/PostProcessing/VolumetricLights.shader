Shader "Hidden/Universal Render Pipeline/VolumetricLights"
{
    HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

        TEXTURE2D_X(_BlitTex);
        float3 _FrustumCorners[4];
        float4x4 _MatrixScreenToWorldLeft;
        float4x4 _MatrixScreenToWorldRight;

        struct VertexData {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Interpolators
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 ray : TEXCOORD1;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        Interpolators VertMy(VertexData input)
        {
            Interpolators output;

            UNITY_SETUP_INSTANCE_ID(input);
            //UNITY_INITIALIZE_OUTPUT(Interpolators, output);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
            output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
            output.uv = input.uv;
            output.ray = _FrustumCorners[input.uv.x + 2 * input.uv.y];
            return output;
        }

        inline half3 GetCameraDirection(half2 uv, float depth)
        {
            #ifndef SHADER_API_GLCORE
                half4 positionCS = half4(uv * 2 - 1, depth, 1) * LinearEyeDepth(depth, _ZBufferParams);
            #else
                half4 positionCS = half4(uv * 2 - 1, depth * 2 - 1, 1) * LinearEyeDepth(depth, _ZBufferParams);
            #endif
            return mul(lerp(_MatrixScreenToWorldLeft, _MatrixScreenToWorldRight, unity_StereoEyeIndex), positionCS).xyz;
        }

        float2 raySphereIntersection(float3 rayPos, float3 rayDirection, float3 spherePos, float sphereRadius)
        {
            float3 sphereDirection = spherePos - rayPos;
            sphereDirection *= step(0, dot(normalize(sphereDirection), rayDirection));
            float tMiddle = dot(sphereDirection, rayDirection);
            float3 posMiddle = rayPos + rayDirection*tMiddle;
            float distanceSphereToTMiddle = length(spherePos - posMiddle);

            if (distanceSphereToTMiddle < sphereRadius)
            {
                float distancePosMiddleToSphereEdge = sqrt(sphereRadius*sphereRadius - distanceSphereToTMiddle*distanceSphereToTMiddle);
                //float distancePosMiddleToSphereEdge2 = sqrt(pow(min(tMiddle, sphereRadius), 2) - distanceSphereToTMiddle*distanceSphereToTMiddle);

                
                float distToVolume = tMiddle - distancePosMiddleToSphereEdge;
                float distThroughVolume = distancePosMiddleToSphereEdge + distancePosMiddleToSphereEdge;
                return float2(distToVolume, distThroughVolume);
            }
            else return float2(0, -1);

            float2(0, 0);
        }

        float2 rayConeIntersection(float3 rayPos, float3 rayDirection, float3 conePointPos, float3 coneBasePos, float coneRadius)
        {
            float3 axis = (coneBasePos - conePointPos);
            float3 theta = (axis / length(axis));
            float m = pow(coneRadius, 2) / pow(length(axis), 2);
            float3 w = (rayPos - conePointPos);

            float a = dot(rayDirection, rayDirection) - m * (pow(dot(rayDirection, theta), 2)) - pow(dot(rayDirection, theta), 2);
            float b = 2 * (dot(rayDirection, w) - m * dot(rayDirection, theta) * dot(w, theta) - dot(rayDirection, theta) * dot(w, theta));
            float c = dot(w, w) - m * pow(dot(w, theta), 2) - pow(dot(w, theta), 2);

            float discriminant = pow(b, 2) - (4 * a * c);

            //if (discriminant > 0)
            //{
                float t1 = ((-b - sqrt(discriminant)) / (2 * a));
                float t2 = ((-b + sqrt(discriminant)) / (2 * a));

                float distToVolume = t1;
                float distThroughVolume = t2 - t1;
                return float2(distToVolume, distThroughVolume);
            //}
           // return float2(0, -1);
        }

        half4 Frag(Interpolators input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            float2 uv = UnityStereoTransformScreenSpaceTex(input.uv);

            half3 color = SAMPLE_TEXTURE2D_X(_BlitTex, sampler_LinearClamp, uv).xyz;
            float depth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_LinearClamp, uv).x;
            float linearDepth = LinearEyeDepth(depth, _ZBufferParams);

            float viewDistance = depth * _ProjectionParams.z - _ProjectionParams.y;
            viewDistance = length(input.ray * Linear01Depth(depth, _ZBufferParams));

            // Volumetric light parameters
            float3 volumetricLightPositionWS = float3(1, 5, 0);
            float3 volumetricLightDirection = float3(0, -8, 0);
            float3 volumetricLightColor = float3(0.2, 0.2, 0.1);
            float  volumetricLightRadius = 5;
            float  volumetricLightDiameter = volumetricLightRadius * 2;

            float3 cameraDirection = normalize(GetCameraDirection(uv, depth) - GetCameraPositionWS());
            float3 volumetricLightViewDirection = volumetricLightPositionWS - GetCameraPositionWS();
            float3 volumetricLightViewDirectionNormalized = normalize(volumetricLightViewDirection);
            
            #define SPHERICAL_VOLUME 1

            #if SPHERICAL_VOLUME
                // Sphere volume intersecion
                float2 volumeIntersection = raySphereIntersection(GetCameraPositionWS(), cameraDirection, volumetricLightPositionWS, volumetricLightRadius);
                float distToVolume = volumeIntersection.x;
                float distThroughVolume = volumeIntersection.y;

                if (distThroughVolume > 0)
                {
                    distThroughVolume = min(distThroughVolume / 2, max(0, viewDistance - distToVolume));
                    float3 volumeMiddle = GetCameraPositionWS() + cameraDirection * (distToVolume + distThroughVolume / 2);
                    float3 volumeMiddleSourceDirection = normalize(volumetricLightPositionWS - volumeMiddle);

                    color += (distThroughVolume / volumetricLightRadius)   *   pow(max(0, dot(cameraDirection, volumeMiddleSourceDirection)), 1)   *   0.1;
                    //color += (distThroughVolume / volumetricLightRadius);

                    //color += 1;
                }
            #else
                // Cone volume intersection
                float2 volumeIntersection = rayConeIntersection(GetCameraPositionWS(), cameraDirection, volumetricLightPositionWS, volumetricLightPositionWS + volumetricLightDirection, volumetricLightRadius);
                float distToVolume = volumeIntersection.x;
                float distThroughVolume = volumeIntersection.y;
                distThroughVolume = min(distThroughVolume, max(0, viewDistance - distToVolume));

                if (distThroughVolume > 0)
                {
                    // Cone
                    float3 volumeMiddle = GetCameraPositionWS() + cameraDirection * (distToVolume + distThroughVolume / 2);
                    float3 volumeMiddleSourceDirection = normalize(volumetricLightPositionWS - volumeMiddle);

                    if (dot(volumeMiddleSourceDirection, normalize(volumetricLightDirection)) < 0)
                    {
                        color += max(0, (1 - length(volumeMiddle - volumetricLightPositionWS) / length(volumetricLightDirection)))   *   pow(dot(-normalize(volumetricLightDirection), volumeMiddleSourceDirection), 16)   *   0.1;
                        //color = max(0, (1 - length(volumeMiddle - volumetricLightPositionWS) / length(volumetricLightDirection)));
                        //color = pow(dot(-normalize(volumetricLightDirection), volumeMiddleSourceDirection), 32);
                        //color *= step(length(volumeMiddle - volumetricLightPositionWS), 5);
                        //color += max(0, dot(float3(0, 0, 1), volumeMiddleSourceDirection)) * 100;
                    }
                }
            #endif

            //color = cameraDirection;
            //color = lerp(float3(1, 0, 0), float3(0, 0, 1), unity_StereoEyeIndex);

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
            Name "VolumetricLights"

            HLSLPROGRAM
                #pragma vertex VertMy
                #pragma fragment Frag
            ENDHLSL
        }
    }
}
