Shader "Unlit/shader_Forcefield"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _MaxHeight("MaxHeight", Float) = .8
        _TimeToFade("TimeToFadeHit", Float) = .3
        _MaxDist("MaxHitEffectDistance", Float) = 4
        _HitPos("HitPosition", Vector) = (0,0,0)
        _HitTime("HitTime", Float) = -10
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        LOD 100

        Cull Front

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            #define PI 3.14159265359

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            //UNITY_FOG_COORDS(1)
            float4 vertex : SV_POSITION;
            float3 fragObjPos : TEXCOORD1;
            float3 fragWorldPos : TEXCOORD2;
        };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _MaxHeight;
            float _HitTime;
            float3 _HitPos;
            float _TimeToFade;
            float _MaxDist;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.fragObjPos = v.vertex.xyz;
                o.fragWorldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                // Modify the fragment's alpha based on your logic
                // Example: discard fragments below an alpha threshold
                if (col.a < 0.01) {
                    discard;  // Discards the fragment if alpha is too low
                }

                //height blend
                col.a *= smoothstep(_MaxHeight, 0, i.fragObjPos.z + .1);

                float time = _Time.y;
                bool beenHit = time < _HitTime + _TimeToFade;
                if (!beenHit && col.a < 0.01) {
                    discard;  // Discards the fragment if alpha is too low
                }

                //collision calculation
                if (beenHit) {
                    float timeSinceHit = time - _HitTime;
                    float hitFadeA = (sin((time / _TimeToFade + .5) * PI) + 1.0) / 2.0;

                    float dist = smoothstep(_MaxDist, 0, distance(i.fragWorldPos, _HitPos));

                    col.a = max(col.a, hitFadeA*dist);
                }

                // You can modify depth and other behaviors here
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
