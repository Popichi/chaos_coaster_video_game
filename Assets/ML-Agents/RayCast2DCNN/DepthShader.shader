Shader "Hidden/DepthShader" {
    Properties{
_LogBase("Log Base", Range(1, 100)) = 6
    }
    SubShader{
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
             float _LogBase;
            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float depth : TEXCOORD0;
            };
            
            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.depth = o.vertex.z/o.vertex.w;
                //o.depth = log2(_LogBase + o.vertex.z) / log2(_LogBase + 150);
                return o;
            }
           
            fixed4 frag(v2f i) : SV_Target{
                // i.depth.x /= i.depth.y;
                 // Normalize the depth between 0 and 1.

            // return log2(i.depth * 0.5 + 0.5 + _base)/log2(_base + 1);
            //return i.depth;
            return 1 - (i.depth * 0.5 + 0.5);
            }
            ENDCG
        }
    }
}