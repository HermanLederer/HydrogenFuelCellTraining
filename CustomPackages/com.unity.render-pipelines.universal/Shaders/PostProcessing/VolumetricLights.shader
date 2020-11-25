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
        float4x4 _MV;
        float4x4 _MP;

        struct Interpolators
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 ray : TEXCOORD1;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        Interpolators VertMy(Attributes input)
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

        void transformRay(float3 ro, float3 rd, out float3 outro, out float3 outrd, float3 offset, float3 rotation, float scale)
        {
            // offset
            outro = ro + offset;

            // rotation
            float radX = rotation.x * 0.01745329;
            float radY = rotation.y * 0.01745329;
            float radZ = rotation.z * 0.01745329;
            float sinX = sin(radX);
            float cosX = cos(radX);
            float sinY = sin(radY);
            float cosY = cos(radY);
            float sinZ = sin(radZ);
            float cosZ = cos(radZ);

            float3 xAxis = float3(
                cosY * cosZ,
                cosX * sinZ + sinX * sinY * cosZ,
                sinX * sinZ - cosX * sinY * cosZ
            );
            float3 yAxis = float3(
                -cosY * sinZ,
                cosX * cosZ - sinX * sinY * sinZ,
                sinX * cosZ + cosX * sinY * sinZ
            );
            float3 zAxis = float3(
                sinY,
                -sinX * cosY,
                cosX * cosY
            );

            outro /= scale;

            outro = xAxis * outro.x   +   yAxis * outro.y   +   zAxis * outro.z;
            outrd = xAxis * rd.x + yAxis * rd.y + zAxis * rd.z;
            //outrd = xAxis * rd.x   +   yAxis * rd.y   +   zAxis * rd.z;
        }

        float3 rotateVector(float3 rd, float3 rotation)
        {
            float radX = rotation.x * 0.01745329;
            float radY = rotation.y * 0.01745329;
            float radZ = rotation.z * 0.01745329;
            float sinX = sin(radX);
            float cosX = cos(radX);
            float sinY = sin(radY);
            float cosY = cos(radY);
            float sinZ = sin(radZ);
            float cosZ = cos(radZ);

            float3 xAxis = float3(
                cosY * cosZ,
                cosX * sinZ + sinX * sinY * cosZ,
                sinX * sinZ - cosX * sinY * cosZ
            );
            float3 yAxis = float3(
                -cosY * sinZ,
                cosX * cosZ - sinX * sinY * sinZ,
                sinX * cosZ + cosX * sinY * sinZ
            );
            float3 zAxis = float3(
                sinY,
                -sinX * cosY,
                cosX * cosY
            );

            return xAxis * rd.x + yAxis * rd.y + zAxis * rd.z;
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
                float distToVolume = tMiddle - distancePosMiddleToSphereEdge;
                float distThroughVolume = distancePosMiddleToSphereEdge + distancePosMiddleToSphereEdge;
                return float2(distToVolume, distThroughVolume);
            }
            else return float2(0, -1);

            float2(0, 0);
        }

        // cone inscribed in a unit cube centered at 0
        bool rayConeIntersection(float3 rayPos, float3 rayDirection, out float near, out float far)
        {
            // scale and offset into a unit cube
            rayPos.x += 0;
            //rayPos.x += 0.5;
            float s = 0.5;
            rayPos.x *= s;
            rayDirection.x *= s;
            
            // quadratic x^2 = y^2 + z^2
            float a = rayDirection.y * rayDirection.y + rayDirection.z * rayDirection.z - rayDirection.x * rayDirection.x;
            float b = rayPos.y * rayDirection.y + rayPos.z * rayDirection.z - rayPos.x * rayDirection.x;
            float c = rayPos.y * rayPos.y + rayPos.z * rayPos.z - rayPos.x * rayPos.x;
            
            float cap = (s - rayPos.x) / rayDirection.x;
            
            // linear
            if( a == 0.0 )
            {
                near = -0.5 * c/b;
                float x = rayPos.x + near * rayDirection.x;
                if( x < 0.0 || x > s )
                    return false; 

                far = cap;
                float temp = min(far, near); 
                far = max(far, near);
                near = temp;
                return far > 0.0;
            }

            float delta = b * b - a * c;
            if( delta < 0.0 )
                return false;

            // 2 roots
            float deltasqrt = sqrt(delta);
            float arcp = 1.0 / a;
            near = (-b - deltasqrt) * arcp;
            far = (-b + deltasqrt) * arcp;
            
            // order roots
            float temp = min(far, near);
            far = max(far, near);
            near = temp;

            float xnear = rayPos.x + near * rayDirection.x;
            float xfar = rayPos.x + far * rayDirection.x;

            if( xnear < 0.0 )
            {
                if( xfar < 0.0 || xfar > s )
                    return false;
                
                near = far;
                far = cap;
            }
            else if( xnear > s )
            {
                if( xfar < 0.0 || xfar > s )
                    return false;
                
                near = cap;
            }
            else if( xfar < 0.0 )
            {
                // The apex is problematic,
                // additional checks needed to
                // get rid of the blinking tip here.
                far = near;
                near = cap;
            }
            else if( xfar > s )
            {
                far = cap;
            }
            
            return far > 0.0;
        }

        float4 iRoundedConeDistanceAndNormals(
            in float3 ro, in float3 rd, 
            in float3 pa, in float3 pb, 
            in float ra, in float rb
        )
        {
            float3  ba = pb - pa;
            float3  oa = ro - pa;
            float3  ob = ro - pb;
            float rr = ra - rb;
            float m0 = dot(ba,ba);
            float m1 = dot(ba,oa);
            float m2 = dot(ba,rd);
            float m3 = dot(rd,oa);
            float m5 = dot(oa,oa);
            float m6 = dot(ob,rd);
            float m7 = dot(ob,ob);
            
            float d2 = m0-rr*rr;
            
            float k2 = d2    - m2*m2;
            float k1 = d2*m3 - m1*m2 + m2*rr*ra;
            float k0 = d2*m5 - m1*m1 + m1*rr*ra*2.0 - m0*ra*ra;
            
            float h = k1*k1 - k0*k2;
            if(h < 0.0) return float4(-1.0, -1.0, -1.0, -1.0);
            float t = (-sqrt(h)-k1)/k2;
            //if( t<0.0 ) return float4(-1.0);

            float y = m1 - ra*rr + t*m2;
            if( y>0.0 && y<d2 ) 
            {
                return float4(t, normalize( d2*(oa + t*rd)-ba*y) );
            }

            // Caps. I feel this can be done with a single square root instead of two
            float h1 = m3*m3 - m5 + ra*ra;
            float h2 = m6*m6 - m7 + rb*rb;
            if( max(h1,h2)<0.0 ) return float4(-1.0, -1.0, -1.0, -1.0);
            
            float4 r = float4(1e20, 1e20, 1e20, 1e20);
            if( h1>0.0 )
            {        
                t = -m3 - sqrt( h1 );
                r = float4( t, (oa+t*rd)/ra );
            }
            if( h2>0.0 )
            {
                t = -m6 - sqrt( h2 );
                if( t<r.x )
                r = float4( t, (ob+t*rd)/rb );
            }
            
            return r;
        }

        float2 iRoundedCone(
            in float3 ro, in float3 rd, 
            in float3 pa, in float3 pb, 
            in float ra, in float rb
        )
        {
            float3 ba = pb - pa;
            float3 oa = ro - pa;
            float3 ob = ro - pb;
            float rr = ra - rb;
            float m0 = dot(ba,ba);
            float m1 = dot(ba,oa);
            float m2 = dot(ba,rd);
            float m3 = dot(rd,oa);
            float m5 = dot(oa,oa);
            float m6 = dot(ob,rd);
            float m7 = dot(ob,ob);
            
            float d2 = m0-rr*rr;
            
            float k2 = d2    - m2*m2;
            float k1 = d2*m3 - m1*m2 + m2*rr*ra;
            float k0 = d2*m5 - m1*m1 + m1*rr*ra*2.0 - m0*ra*ra;
            
            float h = k1*k1 - k0*k2;
            if (h < 0.0) return float2(1, 1);
            float t = (-sqrt(h)-k1)/k2;
            float t2 = (sqrt(h)-k1)/k2;
            //if( t<0.0 ) return float2(-1.0);

            float y = m1 - ra*rr + t*m2;
            if( y>0.0 && y<d2 ) 
            {
                return float2(t, t2); // looks like the straight part
            }

            // Caps. I feel this can be done with a single square root instead of two
            float h1 = m3*m3 - m5 + ra*ra;
            float h2 = m6*m6 - m7 + rb*rb;
            if (max(h1, h2) < 0.0) return float2(-1, -1);
            
            float2 r = float2(1e20, 0);
            if (h1 > 0)
            {        
                t = -m3 - sqrt(h1);
                t2 = -m3 + sqrt(h1);
                r = float2(t, t2);
            }
            if (h2 > 0)
            {
                t = -m6 - sqrt(h2);
                t2 = -m6 + sqrt(h2);
                if(t < r.x)
                r = float2(t, t2);
            }
            
            return r; // looks like the rounded parts
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
            float3 volumetricLightPositionWS = float3(0, 5, 0);
            float3 volumetricLightRotation = float3(0, 0, 0);
            float3 volumetricLightDirection = float3(0, -1, 0);
            float3 volumetricLightColor = float3(0.2, 0.2, 0.1);
            float volumetricLightRadius = 4;
            float volumetricLightHeight = 6;

            float3 cameraDirection = normalize(GetCameraDirection(uv, depth) - GetCameraPositionWS());
            float3 volumetricLightViewDirection = volumetricLightPositionWS - GetCameraPositionWS();
            float3 volumetricLightViewDirectionNormalized = normalize(volumetricLightViewDirection);

            #define SPHERICAL_VOLUME 0

            #if SPHERICAL_VOLUME
                // Sphere volume intersecion
                float2 volumeIntersection = raySphereIntersection(GetCameraPositionWS(), cameraDirection, volumetricLightPositionWS, volumetricLightRadius);
                float distToVolume = volumeIntersection.x;
                float distThroughVolume = volumeIntersection.y;

                if (distThroughVolume > 0)
                {
                    // Cutting level geometry from the depth
                    distThroughVolume = min(distThroughVolume, max(0, viewDistance - distToVolume));

                    float3 volumeMiddlePos = GetCameraPositionWS() + cameraDirection * (distToVolume + distThroughVolume / 2);
                    float3 volumeMiddleSourceDirection = normalize(volumetricLightPositionWS - volumeMiddlePos);

                    color += pow((1 - (length(volumeMiddlePos - volumetricLightPositionWS)) / volumetricLightRadius), 2)   *   0.1;
                    //color += (distThroughVolume / volumetricLightRadius)   *    pow((1 - (length(volumeMiddle - volumetricLightPositionWS)) / volumetricLightRadius), 2)   *   0.1;
                }
            #else
                // Cone volume intersection
                float near;
                float far;
                float3 ro;
                float3 rd;
                ro = GetCameraPositionWS();
                rd = cameraDirection;
                //transformRay(GetCameraPositionWS(), cameraDirection, ro, rd, -volumetricLightPositionWS, volumetricLightRotation, volumetricLightHeight);
                // if (rayConeIntersection(ro, rd, near, far))
                // {
                //     far = min(far, viewDistance);
                //     near = min(near, viewDistance);
                //     float distThroughVolume = far - max(0, near);
                //     float3 volumeMiddlePos = GetCameraPositionWS() + cameraDirection * lerp(near, far, 0.5);
                    
                //     if (distThroughVolume > 0)
                //     {
                //         color += pow(max(0, 1 - length(volumeMiddlePos - volumetricLightPositionWS)), 2);
                //         //color += pow(max(0, 1 - length(volumeMiddlePos)), 2)   *   pow(max(0, dot(volumeMiddlePos, float3(0, 1, 0))), 1);
                //         //color += distThroughVolume;
                //         //color = 1;
                //     }

                //     //float3 volumeMiddleSourceDirection = normalize(volumetricLightPositionWS - volumeMiddle);

                //     //color += pow((1 - (length(volumeMiddle - volumetricLightPositionWS)) / volumetricLightRadius), 1)   *   pow(dot(-normalize(volumetricLightDirection), volumeMiddleSourceDirection), 2)   *   0.1;
                //     // color += (1 - (length(volumeMiddle - volumetricLightPositionWS) / volumetricLightHeight));
                //     //color = length(volumetricLightPositionWS - volumeMiddle) / 5;
                //     //color = pow((1 - (length(volumeMiddlePos - volumetricLightPositionWS)) / volumetricLightRadius), 2);
                    
                // }

                float2 rc = iRoundedCone(ro, rd, volumetricLightPositionWS, volumetricLightPositionWS + volumetricLightDirection * volumetricLightHeight, 0, volumetricLightRadius);
                near = rc.x;
                far = rc.y;
                if (rc.x > 0)
                {
                    far = min(far, viewDistance);
                    near = min(near, viewDistance);
                    float distThroughVolume = far - max(0, near);
                    float3 volumeMiddlePos = GetCameraPositionWS() + cameraDirection * (near + distThroughVolume / 2);
                    
                    if (distThroughVolume > 0)
                    {
                        //color = pow(max(0, 1 - length(volumeMiddlePos - volumetricLightPositionWS) / volumetricLightHeight), 2);
                        //color = pow(max(0, dot(float3(0, 1, 0), normalize(volumetricLightPositionWS - volumeMiddlePos))), 16);
                        color += max(0, 1 - length(volumeMiddlePos - volumetricLightPositionWS) / volumetricLightHeight)   *   pow(max(0, dot(-volumetricLightDirection, normalize(volumetricLightPositionWS - volumeMiddlePos))), 8)  *   0.1;
                        //color += distThroughVolume;
                        //color = 1;
                        //float3 volumeMiddleSourceDirection = normalize(volumetricLightPositionWS - volumeMiddle);

                        //color += pow((1 - (length(volumeMiddle - volumetricLightPositionWS)) / volumetricLightRadius), 1)   *   pow(dot(-normalize(volumetricLightDirection), volumeMiddleSourceDirection), 2)   *   0.1;
                        // color += (1 - (length(volumeMiddle - volumetricLightPositionWS) / volumetricLightHeight));
                        //color = length(volumetricLightPositionWS - volumeMiddle) / 5;
                        //color = pow((1 - (length(volumeMiddlePos - volumetricLightPositionWS)) / volumetricLightRadius), 2);
                    }
                }
            #endif

            // Cone volume intersection


            // Various debugs
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
