Shader "Custom/DrawParticle"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            uint BIT_NOISE1 = 0xB5297A4D;
            uint BIT_NOISE2 = 0x68E31DA4;
            uint BIT_NOISE3 = 0x1B56C4E9;

            uint squirrel3(int position, uint seed)
            {
                uint mangled = (uint)position;
                mangled *= BIT_NOISE1;
                mangled += seed;
                mangled ^= (mangled >> 8);
                mangled += BIT_NOISE2;
                mangled ^= (mangled << 8);
                mangled *= BIT_NOISE3;
                mangled ^= (mangled >> 8);
                return mangled;
            };

            uniform float time;
            uniform float frequence;
            float rand(float2 uv) {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            };
            struct appdata
            {
                uint vertexID : SV_VertexID;
            };

            struct v2g
            {
                float4 position : POSITION;
                uint id : TEXCOORD0;
                float f : TEXCOORD1;
                float sizeF : TEXCOORD2;
            };

            struct g2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float f : TEXCOORD1;
            };

            struct MyParticle
            {
                float3 pos;
                float3 vel;
                float mass;
            };
            uniform float4 parent;
            StructuredBuffer<MyParticle> particleBuffer;
            uniform float size;
            v2g vert(appdata v)
            {
                v2g o;
                o.position = float4(particleBuffer[v.vertexID].pos,1) + parent;
                o.id = v.vertexID;

                
                float r1 = rand(float2(v.vertexID, v.vertexID+1));
                float r2 = squirrel3(v.vertexID,o.position.x);
                float fun = (sin((time - 10*r1) * frequence) + 1) / 2;
                float f = clamp(fun,0.5, 1);
                o.f = f;
                o.sizeF = pow(particleBuffer[v.vertexID].mass, 1.0 / 3.0);
                return o;
            }

        

            [maxvertexcount(4)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
            {
                // Compute the billboard position
                float3 particlePosition = IN[0].position ;
                float3 cameraPosition = _WorldSpaceCameraPos;
                float3 directionToCamera = normalize(cameraPosition - particlePosition);
                float f = IN[0].f;
                // Compute the quad corners
                // Compute the quad corners
                float3 right = cross(directionToCamera, float3(0,1,0));
                float3 up = cross(right, directionToCamera);

                 // Change this to control the size of the billboards
                float s = IN[0].sizeF * size;
                g2f o;
                
                o.vertex = mul(UNITY_MATRIX_VP, float4(particlePosition - right * s - up * s, 1));
                o.uv = float2(0,0);
                o.f = f;
                triStream.Append(o);

                o.vertex = mul(UNITY_MATRIX_VP, float4(particlePosition - right * s + up * s, 1));
                o.uv = float2(0, 1);
                o.f = f;
                triStream.Append(o);

                o.vertex = mul(UNITY_MATRIX_VP, float4(particlePosition + right * s - up * s, 1));
                o.uv = float2(1,0);
                o.f = f;
                triStream.Append(o);

       

                o.vertex = mul(UNITY_MATRIX_VP, float4(particlePosition + right * s + up * s, 1));
                o.uv = float2(1,1);
                o.f = f;
                triStream.Append(o);
            }

            sampler2D _MainTex;
            
            fixed4 frag(g2f i) : SV_Target
            {
                // Sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
            col *= i.f;
            // Set the color of the particle
            return col;
        }
        ENDCG
    }
    }
}
