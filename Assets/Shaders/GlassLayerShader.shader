Shader "Custom/GlassLayerShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _GlassTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _GlassTex;
            //float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct fragmentInput
            {
                float2 worldPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
                //float2 uv : TEXCOORD1;
            };

            fragmentInput vert (appdata v)
            {
                fragmentInput o;
                float3 pos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = float2(pos.x, pos.y);
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(fragmentInput fragInput) : SV_Target
            {
                //float worldx = fragInput.worldPos.x;
                //float worldy = fragInput.worldPos.y;
                //worldx = worldx - floor(worldx);
                //worldy = worldy - floor(worldy);
                //float2 coords = float2(worldx, worldy);
                return tex2D(_GlassTex, fragInput.worldPos.xy);
            }
            ENDCG
        }
    }
}
