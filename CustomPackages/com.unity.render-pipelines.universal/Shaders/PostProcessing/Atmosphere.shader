Shader "Hidden/Universal Render Pipeline/Atmosphere"
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
        float4x4 _MatrixHClipToWorld;
        float3 _CameraPosition;

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
            #ifndef SHADER_API_GLCORE
                half4 positionCS = half4(uv * 2 - 1, depth, 1) * LinearEyeDepth(depth, _ZBufferParams);
            #else
                half4 positionCS = half4(uv * 2 - 1, depth * 2 - 1, 1) * LinearEyeDepth(depth, _ZBufferParams);
            #endif
            return mul(_MatrixHClipToWorld, positionCS).xyz;
        }

        half4 Frag(Interpolators input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            float2 uv = UnityStereoTransformScreenSpaceTex(input.uv);

            half3 color = SAMPLE_TEXTURE2D_X(_BlitTex, sampler_LinearClamp, uv).xyz;
            float depth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_LinearClamp, uv).x;

            float viewDistance = depth * _ProjectionParams.z - _ProjectionParams.y;
            viewDistance = length(input.ray * Linear01Depth(depth, _ZBufferParams));
            
            float3 ray = normalize(_CameraPosition - GetCameraDirection(uv, depth));
            float3 skyColor = lerp(float3(0.5, 0.6, 1), float3(0.1, 0.3, 1.25), saturate(-ray.y + 0.5));
            Light sun = GetMainLight();
            skyColor = lerp(skyColor, float3(10000, 10000, 10000), smoothstep(0.999, 1, -dot(ray, sun.direction)));

            color = lerp(color, skyColor, smoothstep(0, 100, viewDistance));
            //color = input.eyeRay;

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
