Shader "Custom/DirtEraseShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _DirtTex ("Dirt Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _DirtTex;
            sampler2D _MaskTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 baseCol = tex2D(_MainTex, i.uv);
                float4 dirtCol = tex2D(_DirtTex, i.uv);
                float mask = tex2D(_MaskTex, i.uv).r;

                // Если mask белый (1) — грязь видна. Если 0 — грязь убрана.
                float4 result = lerp(dirtCol, baseCol, 1 - mask);
                return result;
            }
            ENDCG
        }
    }
}
