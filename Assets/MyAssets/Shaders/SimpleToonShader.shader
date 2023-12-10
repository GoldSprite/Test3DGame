// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "TNShaderPractise/ShaderPractise_SimpleToonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Ramp("RampMap",2D)="white"{}
		_ShadowColor("Color",Color)=(1,1,1,1)
		_OutlineWidth("描边粗细",Range(0,0.5))=0.1
		_OutlineColor("描边颜色",Color)=(0,0,0,0)
		[HDR]_SpColor("高光颜色",Color)=(1,1,1,1)
		_SpecScale("高光阈值",Range(0.1,0.99))=1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {	 //给pass起个名字方便以后直接调用这个pass进行外描边渲染
			NAME "OUTLINE"
			 //第一个pass剔出正面只渲染背面进行描边拓展和上色
			Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Ramp;
			fixed4 _ShadowColor;
			float _OutlineWidth;
			fixed4 _OutlineColor;
			fixed4 _SpColor;
			float _SpecScale;

            struct a2v
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };


            v2f vert (a2v v)
            {
                v2f o;
				//将顶点和法线都转换到视角空间下进行计算更精确
				float4 pos =float4( UnityObjectToViewPos(v.vertex) ,0);
                float3 vNormal = mul((float3x3)UNITY_MATRIX_IT_MV,v.normal)	 ;
				//控制描边Z方向拓展上限
				vNormal.z = -0.5;
				//这里需要注意的是 pos外展的 float4中的w分量必须为1才能正常显示描边. 我理解为向量和点在齐次坐标空间下的表示中1为点受位置影响 0则为向量不受位置影响  所以这里必须为1
				pos = pos + float4(normalize(vNormal),1)*_OutlineWidth;
				//将顶点从视角空间转换会裁剪空间
				o.pos = mul(UNITY_MATRIX_P,pos); 
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv);
				//让描边颜色和固有色有正片叠底
                fixed4 outline = _OutlineColor*col;

				//增加描边饱和度
                half luminance = outline.r*0.2125+outline.g*0.7154+outline.b*0.0721;

				fixed4 finalOutline = lerp (luminance,outline,5);
			    return 	finalOutline;
            }
            ENDCG
        }

		Pass
		{
			Tags{"LightMode"="ForwardBase"}
			//剔除背面 因为背面用来渲染外描边了
			Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma mutil_compile_fwdbase
            #include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

			sampler2D _MainTex;
			sampler2D _Ramp;
			fixed4 _ShadowColor;
			float _OutlineWidth;
			fixed4 _OutlineColor;
			fixed4 _SpColor;
			float _SpecScale;

            struct a2v
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD1;
                float4 pos : SV_POSITION;
				float4 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				SHADOW_COORDS (4)
            };

			v2f vert (a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld,v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.uv = v.uv;

				TRANSFER_SHADOW (o);

				return o;
			}

			fixed4 frag (v2f i):SV_Target
			{
					//列出世界空间下需要的各个计算参数  半兰伯特光照
				fixed3 normal = normalize(i.worldNormal);
				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

				half NdotL = saturate (dot(normal,lightDir)) ;
				fixed3 halfDir = normalize (lightDir+viewDir);
				half halfNL = NdotL*0.5+0.5;

				half NdotH = saturate(dot(normal,halfDir));

				fixed4 diff = tex2D(_MainTex,i.uv); 
				fixed4 ramp = tex2D (_Ramp,float2(halfNL,halfNL));
				
				
				//给阴影添加颜色,也就是把_Ramp作为一个插值参考值 与颜色进行相乘 如果为0  即 1-ramp = 0 即ramp为1 也就是白色 则直接显示固有色 其余情况下则都要与 shadowColor相乘   
				fixed4 finaldiff  =lerp(diff,diff*ramp*_ShadowColor,1-ramp);

				half spec =NdotH;
				//取边缘 进行抗锯齿计算
				half w = fwidth(spec);

				 fixed4 specCol = finaldiff*_SpColor*lerp(0,1,(smoothstep(-w,w,spec-_SpecScale)))*step(0.0001,_SpecScale);	   //这里是为了防止为0,的时候高光溢出

				 UNITY_LIGHT_ATTENUATION (atten , i , i.worldPos)

				 fixed3 amb = UNITY_LIGHTMODEL_AMBIENT.xyz*finaldiff.rgb;

				 fixed4 finalCol = (finaldiff+float4(amb,1)+specCol)*atten;

				 return finalCol;

   			}
			ENDCG
		}
    }
	FallBack "Diffuse"
}

