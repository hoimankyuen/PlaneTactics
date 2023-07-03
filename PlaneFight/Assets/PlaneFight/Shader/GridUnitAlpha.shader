// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/Grid Colored" {
	Properties{
		_Color("Color (RGB)", color)  = (1, 1, 1, 1)
		_GridSize("Grid Cell Size", Float) = 5
		_GridStrongInterval("Grid Strong Line Interval", Float) = 2
		_LineWidth("Grid Line Width", Float) = 0.01
		_LineStrongWidth("Grid Strong Line Width", Float) = 0.04
	}

	SubShader{
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_fog

				#include "UnityCG.cginc"
				#define PI 3.14159265358979323846

				struct appdata_t {
					float4 vertex : POSITION;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					float4 worldPos : TEXCOORD0;
					float eyeDepth : TEXCOORD1;
					float4 right : TEXCOORD2;
					float4 forward : TEXCOORD3;
					UNITY_FOG_COORDS(1)
					UNITY_VERTEX_OUTPUT_STEREO
				};

				uniform fixed4 _Color;
				uniform float _GridSize;
				uniform float _GridStrongInterval;
				uniform float _LineWidth;
				uniform float _LineStrongWidth;

				float isprojectedPositionAtStrong(float3 v, float angle)
				{
					float range = _GridSize * sqrt(3) / 2;
					return 1 - saturate(abs(floor((dot(v, float3(cos(angle), 0, sin(angle))) + range / 2) / range) % _GridStrongInterval));
				}

				float projectPositionModulo(float3 v, float angle)
				{
					float range = _GridSize * sqrt(3) / 2;
					return ((dot(v, float3(cos(angle), 0, sin(angle))) + range / 2) % range + range) % range - range / 2;
				}

				float projectLength(float unitLength, float angle, float3 right, float3 forward)
				{
					return unitLength / length(right * cos(angle) + forward * sin(angle));
				}

				float getLineShading(float pos, float pixelSize, float lineWidth)
				{
					return 1 - saturate(lineWidth / pixelSize - max(0, (lineWidth - pixelSize - 2 * pos) / 2 / pixelSize) - max(0, (lineWidth - pixelSize + 2 * pos) / 2 / pixelSize));
				}

				v2f vert(appdata_t v)
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);				
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.right = mul(UNITY_MATRIX_VP, float4(1, 0, 0, 0));
					o.forward = mul(UNITY_MATRIX_VP, float4(0, 0, 1, 0));
					UNITY_TRANSFER_FOG(o,o.vertex);
					COMPUTE_EYEDEPTH(o.eyeDepth);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 color = _Color;
					float pixelToWorldScale = i.eyeDepth * unity_CameraProjection._m11 / _ScreenParams.y;
					
					float lineColor = 1;
					lineColor *= getLineShading(projectPositionModulo(i.worldPos, 0), 
												projectLength(pixelToWorldScale, 0, i.right, i.forward),
												lerp(_LineWidth, _LineStrongWidth, isprojectedPositionAtStrong(i.worldPos, 0)));

					lineColor *= getLineShading(projectPositionModulo(i.worldPos, PI * 2 / 3), 
												projectLength(pixelToWorldScale, PI * 2 / 3, i.right, i.forward),
												lerp(_LineWidth, _LineStrongWidth, isprojectedPositionAtStrong(i.worldPos, PI * 2 / 3)));

					lineColor *= getLineShading(projectPositionModulo(i.worldPos, PI * 4 / 3), 
												projectLength(pixelToWorldScale, PI * 4 / 3, i.right, i.forward),
												lerp(_LineWidth, _LineStrongWidth, isprojectedPositionAtStrong(i.worldPos, PI * 4 / 3)));
					color.a *= (1 - lineColor);

					return color;
				}		

			ENDCG
		}
	}
}