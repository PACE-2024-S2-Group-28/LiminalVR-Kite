Shader "Custom/LineRendererWithGradientAndTint"
{
    Properties
    {
        [HDR] _Color ("Tint Color (HDR)", Color) = (1,1,1,1)
        _MainTex ("Base Texture", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            // Input properties
            float4 _Color; // HDR Tint color
            float4 _MainTex_ST; // Texture scaling and offset
            sampler2D _MainTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            // Vertex shader
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // Transform to clip space
                o.color = v.color; // Apply the tint color (HDR)
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            // Fragment shader
            half4 frag(v2f i) : SV_Target
            {
                // Combine the texture color and the tint
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
            col *= (_Color * max(col.w, .2f));
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }

            ENDCG
        }
    }
    Fallback "Diffuse"
}