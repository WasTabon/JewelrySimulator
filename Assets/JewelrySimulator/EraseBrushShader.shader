Shader "Custom/EraseBrushShader"
{
    Properties
    {
        _ErasePos ("Erase Position", Vector) = (0,0,0,0)
        _EraseSize ("Erase Size", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZTest Always Cull Off ZWrite Off
        Blend One Zero

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _ErasePos;
            float _EraseSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
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
                float2 diff = i.uv - _ErasePos.xy;
                float dist = length(diff);
                float erase = 1 - smoothstep(_EraseSize * 0.5, _EraseSize, dist);
                return fixed4(erase, erase, erase, 1);
            }

            ENDCG
        }
    }
}
