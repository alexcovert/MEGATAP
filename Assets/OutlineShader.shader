//alex
//Referenced CustomOutline.shader from 
//https://github.com/Shrimpey/Outlined-Diffuse-Shader-Fixed

Shader "Custom/OutlineShader"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_Outline("Outline Width", Range(0, 0.3)) = 0.1
		_OutlineSize("Outline Size", Range(0.5, 1.5)) = 0.95
		_OffsetX("Outline X Offset", Range(-1.5, 1.5)) = 0
		_OffsetY("Outline Y Offset", Range(-1.5, 1.5)) = 0
		_OffsetZ("Outline Z Offset", Range(-1.5, 1.5)) = 0
	}

	SubShader
	{
	    //Outline
		Pass
		{
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 _OutlineColor;
			float _Outline;
			float _OutlineSize;
			float _OffsetX;
			float _OffsetY;
			float _OffsetZ;

			struct appdata
			{
				float4 vertex: POSITION;
				float3 normal: NORMAL;
				float2 uv: TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex: SV_POSITION;
				float3 normal : NORMAL;
				float3 vertexInWorldCoords : TEXCOORD1;
				float2 uv: TEXCOORD0;
			};


			

			v2f vert(appdata v)
			{
				v2f o;
			
				v.vertex *= (_OutlineSize + _Outline);
				o.vertexInWorldCoords = mul(unity_ObjectToWorld, v.vertex); //Vertex position in WORLD coords
				o.normal = v.normal; //Normal 
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				//Offset
				o.vertex.x += _OffsetX;
				o.vertex.y += _OffsetY;
				o.vertex.z += _OffsetZ;


				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return _OutlineColor;
			}

				ENDCG
		}


		//Main part of object 
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler _MainTex;

			struct appdata
			{
				float4 vertex: POSITION;
				float3 normal: NORMAL;
				float2 uv: TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex: SV_POSITION;
				float2 uv: TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float4 c = tex2D(_MainTex, i.uv);
				return c;
			}

			ENDCG
		}
	}

}
