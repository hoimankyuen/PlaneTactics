Shader "Unlit/Boundary"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Size ("Size", Range(1, 100)) = 1
    }
    SubShader
    {
        Tags 
		{ 
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}
        LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _Size;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
				float effectCoord = mul(float2x2(0.5, 0.5, 0.5, -0.5), i.worldPos.xz).x;
				i.uv.x = (effectCoord - floor(effectCoord / _Size)) / _Size;
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
            }
            ENDCG
        }
    }
}
