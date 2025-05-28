Shader "Custom/DirtEraseShader_AutoDark"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _DirtDarkness ("Dirt Darkness", Range(0,1)) = 0.5
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
            sampler2D _MaskTex;
            float _DirtDarkness;

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
                float mask = tex2D(_MaskTex, i.uv).r;

                float4 dirtCol = baseCol * _DirtDarkness;

                float4 result = lerp(dirtCol, baseCol, 1 - mask);
                return result;
            }
            ENDCG
        }
    }
}
