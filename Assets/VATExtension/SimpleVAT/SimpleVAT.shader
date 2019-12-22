Shader "Custom/UnlitSimpleVAT"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _VATex ("VATex", 2D) = "white" {}
        //_AnimTime("AnimTime", float) = 0.0
        _AnimFPS("AnimFPX", float) = 60.0
    }
    SubShader
    {
        Tags { "RenderQueue"="Qpaque" "RenderType"="Opaque" }
        LOD 100
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            Texture2D<float4> _VATex;
            float _AnimTime;
            float _AnimFPS;

            v2f vert (appdata v, uint vId : SV_VertexID)
            {
                v2f o;

                uint timeStep = _Time.y * _AnimFPS;

                uint width, height;
                _VATex.GetDimensions(width, height);

                float3 pos = _VATex[uint2(vId, timeStep % height)];

                o.vertex = UnityObjectToClipPos(pos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
