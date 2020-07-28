Shader "Custom/LightsLayerShader"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _StateTexture("StateTexture", 2D) = "white" {}
    }
    SubShader
    {
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            int height = 72;
            int width = 128;
            sampler2D _MainTex;
            sampler2D _StateTexture;
            int x_offset;
            int y_offset;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct fragmentInput
            {
                float2 worldPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fragmentInput vert(appdata v)
            {
                fragmentInput o;
                float3 pos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = float2(pos.x, pos.y);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(fragmentInput fragInput) : SV_Target
            {
                int x = floor(fragInput.worldPos.x) - x_offset;
                int y = floor(fragInput.worldPos.y) - y_offset;
                float2 uv = float2((x + 0.5) / 128.0, (y + 0.5) / 72.0);

                return tex2D(_StateTexture, uv);
            }
            ENDCG
        }
    }
}