// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "/_Kass_/SH_VFX_PanDissolvePremult_NoZW"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin][Header(Main Alpha)]_Texture("Texture", 2D) = "white" {}
		_TextureChannel("Texture Channel", Vector) = (0,0,0,0)
		_TextureRotation("Texture Rotation", Float) = 0
		[Header(Static Alpha)]_StaticAlphaTexture("Static Alpha Texture", 2D) = "white" {}
		_StaticAlphaChannel("Static Alpha Channel", Vector) = (1,0,0,0)
		_StaticAlphaRotation("Static Alpha Rotation", Float) = 0
		_StaticAlphaInvert("Static Alpha Invert", Float) = 0
		[Header(Dissolve Mask)]_DissolveMask("Dissolve Mask", 2D) = "white" {}
		_DissolveMaskChannel("Dissolve Mask Channel", Vector) = (0,0,0,0)
		_DissolveMaskRotation("Dissolve Mask Rotation", Float) = 0
		_DissolveMaskInvert("Dissolve Mask Invert", Float) = 0
		_DissolveMaskPanSpeed("Dissolve Mask Pan Speed", Vector) = (0,0,0,0)
		[Header(Dissolve Direction)]_DissolveDirection("Dissolve Direction", 2D) = "white" {}
		_DissolveDirectionChannel("Dissolve Direction Channel", Vector) = (0,0,0,0)
		_DissolveDirectionRotation("Dissolve Direction Rotation", Float) = 0
		_DissolveDirectionInvert("Dissolve Direction Invert", Float) = 0
		_MoveWithTexture("Move With Texture", Range( 0 , 1)) = 0
		[Header(Distort Mask)]_DistortMask("Distort Mask", 2D) = "white" {}
		_DistortMaskChannel("Distort Mask Channel", Vector) = (0,0,0,0)
		_DistortMaskRotation("Distort Mask Rotation", Float) = 0
		_DistortPanSpeed("Distort Pan Speed", Vector) = (0,0,0,0)
		_DistortionPower("Distortion Power", Float) = 0
		[Header(Overlay Color)]_ColorTexture("Color Texture", 2D) = "white" {}
		_ColorRotation("Color Rotation", Float) = 0
		[Header(Gradient Shape)]_GradientShape("Gradient Shape", 2D) = "white" {}
		_GradientShapeChannel("Gradient Shape Channel", Vector) = (0,0,0,0)
		_GradientShapeRotation("Gradient Shape Rotation", Float) = 0
		[Header(Gradient Map)]_GradientMap("Gradient Map", 2D) = "white" {}
		_GradientMapDisplacement("Gradient Map Displacement", Float) = 0
		_InvertGradient("Invert Gradient", Float) = 0
		_DifferentCoreColor("Different Core Color", Float) = 0
		_CoreColor("Core Color", Color) = (0,0,0,0)
		[Header(Brightness and Opacity)]_Brightness("Brightness", Float) = 1
		_AlphaBoldness("Alpha Boldness", Float) = 1
		_FlatAlpha("Flat Alpha", Range( 0 , 1)) = 0
		[Header(Depth Fade)]_UseDepthFade("Use Depth Fade", Float) = 1
		_DepthFadeDivide("Depth Fade Divide", Float) = 1
		[Header(Rendering)]_Cull("Cull", Float) = 0
		_ZWrite("ZWrite", Float) = 0
		_ZTest("ZTest", Float) = 2
		_Src("Src", Float) = 5
		[ASEEnd]_Dst("Dst", Float) = 10

		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
		
		Cull [_Cull]
		AlphaToMask Off
		HLSLINCLUDE
		#pragma target 2.0

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS

		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend [_Src] [_Dst], [_Src] [_Dst]
			ZWrite [_ZWrite]
			ZTest [_ZTest]
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			#define _ALPHAPREMULTIPLY_ON 1
			#define _RECEIVE_SHADOWS_OFF 1
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 999999
			#define REQUIRE_DEPTH_TEXTURE 1

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_VERT_POSITION


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef ASE_FOG
				float fogFactor : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_color : COLOR;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _GradientShapeChannel;
			float4 _CoreColor;
			float4 _DissolveDirectionChannel;
			float4 _Texture_ST;
			float4 _DissolveDirection_ST;
			float4 _TextureChannel;
			float4 _DissolveMaskChannel;
			float4 _StaticAlphaTexture_ST;
			float4 _DissolveMask_ST;
			float4 _DistortMaskChannel;
			float4 _ColorTexture_ST;
			float4 _StaticAlphaChannel;
			float4 _DistortMask_ST;
			float4 _GradientShape_ST;
			float2 _DissolveMaskPanSpeed;
			float2 _DistortPanSpeed;
			float _StaticAlphaRotation;
			float _ColorRotation;
			float _StaticAlphaInvert;
			float _DifferentCoreColor;
			float _TextureRotation;
			float _Brightness;
			float _AlphaBoldness;
			float _FlatAlpha;
			float _ZWrite;
			float _MoveWithTexture;
			float _InvertGradient;
			float _DissolveDirectionInvert;
			float _DepthFadeDivide;
			float _DissolveDirectionRotation;
			float _DissolveMaskInvert;
			float _DissolveMaskRotation;
			float _DistortionPower;
			float _DistortMaskRotation;
			float _GradientShapeRotation;
			float _Cull;
			float _Dst;
			float _ZTest;
			float _Src;
			float _GradientMapDisplacement;
			float _UseDepthFade;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _GradientMap;
			sampler2D _GradientShape;
			sampler2D _DistortMask;
			sampler2D _DissolveMask;
			sampler2D _DissolveDirection;
			sampler2D _ColorTexture;
			sampler2D _Texture;
			sampler2D _StaticAlphaTexture;
			uniform float4 _CameraDepthTexture_TexelSize;


						
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord5 = screenPos;
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(v.vertex.xyz));
				float eyeDepth = -objectToViewPos.z;
				o.ase_texcoord6.x = eyeDepth;
				
				o.ase_texcoord3.xy = v.ase_texcoord2.xy;
				o.ase_texcoord3.zw = v.ase_texcoord.xy;
				o.ase_texcoord4 = v.ase_texcoord1;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord6.yzw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				#ifdef ASE_FOG
				o.fogFactor = ComputeFogFactor( positionCS.z );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord2 = v.ase_texcoord2;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif
				float2 uv3_GradientShape = IN.ase_texcoord3.xy * _GradientShape_ST.xy + _GradientShape_ST.zw;
				float cos164 = cos( radians( _GradientShapeRotation ) );
				float sin164 = sin( radians( _GradientShapeRotation ) );
				float2 rotator164 = mul( uv3_GradientShape - float2( 0.5,0.5 ) , float2x2( cos164 , -sin164 , sin164 , cos164 )) + float2( 0.5,0.5 );
				float2 uv_DistortMask = IN.ase_texcoord3.zw * _DistortMask_ST.xy + _DistortMask_ST.zw;
				float cos187 = cos( radians( _DistortMaskRotation ) );
				float sin187 = sin( radians( _DistortMaskRotation ) );
				float2 rotator187 = mul( uv_DistortMask - float2( 0.5,0.5 ) , float2x2( cos187 , -sin187 , sin187 , cos187 )) + float2( 0.5,0.5 );
				float4 uv2s4_DistortMask = IN.ase_texcoord4;
				uv2s4_DistortMask.xy = IN.ase_texcoord4.xy * _DistortMask_ST.xy + _DistortMask_ST.zw;
				float dotResult195 = dot( tex2D( _DistortMask, ( rotator187 + ( _TimeParameters.x * _DistortPanSpeed ) + uv2s4_DistortMask.y ) ) , _DistortMaskChannel );
				float Distortion192 = ( dotResult195 * _DistortionPower );
				float dotResult114 = dot( tex2D( _GradientShape, ( rotator164 + Distortion192 ) ) , _GradientShapeChannel );
				float2 uv_DissolveMask = IN.ase_texcoord3.zw * _DissolveMask_ST.xy + _DissolveMask_ST.zw;
				float cos94 = cos( radians( _DissolveMaskRotation ) );
				float sin94 = sin( radians( _DissolveMaskRotation ) );
				float2 rotator94 = mul( uv_DissolveMask - float2( 0.5,0.5 ) , float2x2( cos94 , -sin94 , sin94 , cos94 )) + float2( 0.5,0.5 );
				float4 uv2s4_DissolveMask = IN.ase_texcoord4;
				uv2s4_DissolveMask.xy = IN.ase_texcoord4.xy * _DissolveMask_ST.xy + _DissolveMask_ST.zw;
				float dotResult116 = dot( tex2D( _DissolveMask, ( rotator94 + ( _TimeParameters.x * _DissolveMaskPanSpeed ) + uv2s4_DissolveMask.y + Distortion192 ) ) , _DissolveMaskChannel );
				float temp_output_72_0 = saturate( dotResult116 );
				float lerpResult86 = lerp( temp_output_72_0 , saturate( ( 1.0 - temp_output_72_0 ) ) , _DissolveMaskInvert);
				float2 uv_DissolveDirection = IN.ase_texcoord3.zw * _DissolveDirection_ST.xy + _DissolveDirection_ST.zw;
				float cos133 = cos( radians( _DissolveDirectionRotation ) );
				float sin133 = sin( radians( _DissolveDirectionRotation ) );
				float2 rotator133 = mul( uv_DissolveDirection - float2( 0.5,0.5 ) , float2x2( cos133 , -sin133 , sin133 , cos133 )) + float2( 0.5,0.5 );
				float2 uv3_DissolveDirection = IN.ase_texcoord3.xy * _DissolveDirection_ST.xy + _DissolveDirection_ST.zw;
				float dotResult136 = dot( tex2D( _DissolveDirection, ( rotator133 + ( uv3_DissolveDirection * _MoveWithTexture ) + Distortion192 ) ) , _DissolveDirectionChannel );
				float temp_output_146_0 = saturate( dotResult136 );
				float lerpResult144 = lerp( temp_output_146_0 , saturate( ( 1.0 - temp_output_146_0 ) ) , _DissolveDirectionInvert);
				float4 texCoord77 = IN.ase_texcoord4;
				texCoord77.xy = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float lerpResult149 = lerp( -1.0 , 1.0 , saturate( ( saturate( lerpResult144 ) + texCoord77.x ) ));
				float temp_output_74_0 = ( saturate( lerpResult86 ) + lerpResult149 );
				float temp_output_80_0 = saturate( ( saturate( dotResult114 ) * temp_output_74_0 ) );
				float lerpResult32 = lerp( saturate( ( 1.0 - temp_output_80_0 ) ) , temp_output_80_0 , _InvertGradient);
				float2 temp_cast_4 = (( lerpResult32 + _GradientMapDisplacement )).xx;
				float2 uv_ColorTexture = IN.ase_texcoord3.zw * _ColorTexture_ST.xy + _ColorTexture_ST.zw;
				float cos173 = cos( radians( _ColorRotation ) );
				float sin173 = sin( radians( _ColorRotation ) );
				float2 rotator173 = mul( uv_ColorTexture - float2( 0.5,0.5 ) , float2x2( cos173 , -sin173 , sin173 , cos173 )) + float2( 0.5,0.5 );
				float2 uv3_ColorTexture = IN.ase_texcoord3.xy * _ColorTexture_ST.xy + _ColorTexture_ST.zw;
				float3 temp_output_39_0 = ( (tex2D( _GradientMap, temp_cast_4 )).rgb * (IN.ase_color).rgb * (tex2D( _ColorTexture, ( rotator173 + uv3_ColorTexture + Distortion192 ) )).rgb );
				float2 uv_Texture = IN.ase_texcoord3.zw * _Texture_ST.xy + _Texture_ST.zw;
				float cos53 = cos( radians( _TextureRotation ) );
				float sin53 = sin( radians( _TextureRotation ) );
				float2 rotator53 = mul( uv_Texture - float2( 0.5,0.5 ) , float2x2( cos53 , -sin53 , sin53 , cos53 )) + float2( 0.5,0.5 );
				float dotResult115 = dot( tex2D( _Texture, ( rotator53 + Distortion192 ) ) , _TextureChannel );
				float2 uv_StaticAlphaTexture = IN.ase_texcoord3.zw * _StaticAlphaTexture_ST.xy + _StaticAlphaTexture_ST.zw;
				float cos243 = cos( radians( _StaticAlphaRotation ) );
				float sin243 = sin( radians( _StaticAlphaRotation ) );
				float2 rotator243 = mul( uv_StaticAlphaTexture - float2( 0.5,0.5 ) , float2x2( cos243 , -sin243 , sin243 , cos243 )) + float2( 0.5,0.5 );
				float dotResult256 = dot( tex2D( _StaticAlphaTexture, rotator243 ) , _StaticAlphaChannel );
				float temp_output_251_0 = saturate( dotResult256 );
				float lerpResult246 = lerp( temp_output_251_0 , saturate( ( 1.0 - temp_output_251_0 ) ) , _StaticAlphaInvert);
				float AdditionalAlpha247 = lerpResult246;
				float temp_output_81_0 = saturate( ( saturate( dotResult115 ) * temp_output_74_0 * AdditionalAlpha247 ) );
				float4 lerpResult124 = lerp( float4( temp_output_39_0 , 0.0 ) , _CoreColor , saturate( temp_output_81_0 ));
				float4 lerpResult127 = lerp( float4( temp_output_39_0 , 0.0 ) , saturate( lerpResult124 ) , _DifferentCoreColor);
				
				float temp_output_101_0 = ( temp_output_81_0 * _AlphaBoldness );
				float lerpResult225 = lerp( temp_output_101_0 , saturate( round( temp_output_101_0 ) ) , _FlatAlpha);
				float4 screenPos = IN.ase_texcoord5;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float eyeDepth234 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float eyeDepth = IN.ase_texcoord6.x;
				float cameraDepthFade235 = (( eyeDepth -_ProjectionParams.y - 0.0 ) / 1.0);
				float lerpResult239 = lerp( 1.0 , saturate( ( ( eyeDepth234 - cameraDepthFade235 ) / _DepthFadeDivide ) ) , _UseDepthFade);
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( saturate( lerpResult127 ) * _Brightness ).rgb;
				float Alpha = saturate( ( lerpResult225 * IN.ase_color.a * saturate( lerpResult239 ) ) );
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				return half4( Color, Alpha );
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM
			#define _ALPHAPREMULTIPLY_ON 1
			#define _RECEIVE_SHADOWS_OFF 1
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 999999
			#define REQUIRE_DEPTH_TEXTURE 1

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_POSITION


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _GradientShapeChannel;
			float4 _CoreColor;
			float4 _DissolveDirectionChannel;
			float4 _Texture_ST;
			float4 _DissolveDirection_ST;
			float4 _TextureChannel;
			float4 _DissolveMaskChannel;
			float4 _StaticAlphaTexture_ST;
			float4 _DissolveMask_ST;
			float4 _DistortMaskChannel;
			float4 _ColorTexture_ST;
			float4 _StaticAlphaChannel;
			float4 _DistortMask_ST;
			float4 _GradientShape_ST;
			float2 _DissolveMaskPanSpeed;
			float2 _DistortPanSpeed;
			float _StaticAlphaRotation;
			float _ColorRotation;
			float _StaticAlphaInvert;
			float _DifferentCoreColor;
			float _TextureRotation;
			float _Brightness;
			float _AlphaBoldness;
			float _FlatAlpha;
			float _ZWrite;
			float _MoveWithTexture;
			float _InvertGradient;
			float _DissolveDirectionInvert;
			float _DepthFadeDivide;
			float _DissolveDirectionRotation;
			float _DissolveMaskInvert;
			float _DissolveMaskRotation;
			float _DistortionPower;
			float _DistortMaskRotation;
			float _GradientShapeRotation;
			float _Cull;
			float _Dst;
			float _ZTest;
			float _Src;
			float _GradientMapDisplacement;
			float _UseDepthFade;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _Texture;
			sampler2D _DistortMask;
			sampler2D _DissolveMask;
			sampler2D _DissolveDirection;
			sampler2D _StaticAlphaTexture;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord4 = screenPos;
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(v.vertex.xyz));
				float eyeDepth = -objectToViewPos.z;
				o.ase_texcoord5.x = eyeDepth;
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				o.ase_texcoord3 = v.ase_texcoord1;
				o.ase_texcoord2.zw = v.ase_texcoord2.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord5.yzw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = TransformWorldToHClip( positionWS );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord2 = v.ase_texcoord2;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_Texture = IN.ase_texcoord2.xy * _Texture_ST.xy + _Texture_ST.zw;
				float cos53 = cos( radians( _TextureRotation ) );
				float sin53 = sin( radians( _TextureRotation ) );
				float2 rotator53 = mul( uv_Texture - float2( 0.5,0.5 ) , float2x2( cos53 , -sin53 , sin53 , cos53 )) + float2( 0.5,0.5 );
				float2 uv_DistortMask = IN.ase_texcoord2.xy * _DistortMask_ST.xy + _DistortMask_ST.zw;
				float cos187 = cos( radians( _DistortMaskRotation ) );
				float sin187 = sin( radians( _DistortMaskRotation ) );
				float2 rotator187 = mul( uv_DistortMask - float2( 0.5,0.5 ) , float2x2( cos187 , -sin187 , sin187 , cos187 )) + float2( 0.5,0.5 );
				float4 uv2s4_DistortMask = IN.ase_texcoord3;
				uv2s4_DistortMask.xy = IN.ase_texcoord3.xy * _DistortMask_ST.xy + _DistortMask_ST.zw;
				float dotResult195 = dot( tex2D( _DistortMask, ( rotator187 + ( _TimeParameters.x * _DistortPanSpeed ) + uv2s4_DistortMask.y ) ) , _DistortMaskChannel );
				float Distortion192 = ( dotResult195 * _DistortionPower );
				float dotResult115 = dot( tex2D( _Texture, ( rotator53 + Distortion192 ) ) , _TextureChannel );
				float2 uv_DissolveMask = IN.ase_texcoord2.xy * _DissolveMask_ST.xy + _DissolveMask_ST.zw;
				float cos94 = cos( radians( _DissolveMaskRotation ) );
				float sin94 = sin( radians( _DissolveMaskRotation ) );
				float2 rotator94 = mul( uv_DissolveMask - float2( 0.5,0.5 ) , float2x2( cos94 , -sin94 , sin94 , cos94 )) + float2( 0.5,0.5 );
				float4 uv2s4_DissolveMask = IN.ase_texcoord3;
				uv2s4_DissolveMask.xy = IN.ase_texcoord3.xy * _DissolveMask_ST.xy + _DissolveMask_ST.zw;
				float dotResult116 = dot( tex2D( _DissolveMask, ( rotator94 + ( _TimeParameters.x * _DissolveMaskPanSpeed ) + uv2s4_DissolveMask.y + Distortion192 ) ) , _DissolveMaskChannel );
				float temp_output_72_0 = saturate( dotResult116 );
				float lerpResult86 = lerp( temp_output_72_0 , saturate( ( 1.0 - temp_output_72_0 ) ) , _DissolveMaskInvert);
				float2 uv_DissolveDirection = IN.ase_texcoord2.xy * _DissolveDirection_ST.xy + _DissolveDirection_ST.zw;
				float cos133 = cos( radians( _DissolveDirectionRotation ) );
				float sin133 = sin( radians( _DissolveDirectionRotation ) );
				float2 rotator133 = mul( uv_DissolveDirection - float2( 0.5,0.5 ) , float2x2( cos133 , -sin133 , sin133 , cos133 )) + float2( 0.5,0.5 );
				float2 uv3_DissolveDirection = IN.ase_texcoord2.zw * _DissolveDirection_ST.xy + _DissolveDirection_ST.zw;
				float dotResult136 = dot( tex2D( _DissolveDirection, ( rotator133 + ( uv3_DissolveDirection * _MoveWithTexture ) + Distortion192 ) ) , _DissolveDirectionChannel );
				float temp_output_146_0 = saturate( dotResult136 );
				float lerpResult144 = lerp( temp_output_146_0 , saturate( ( 1.0 - temp_output_146_0 ) ) , _DissolveDirectionInvert);
				float4 texCoord77 = IN.ase_texcoord3;
				texCoord77.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float lerpResult149 = lerp( -1.0 , 1.0 , saturate( ( saturate( lerpResult144 ) + texCoord77.x ) ));
				float temp_output_74_0 = ( saturate( lerpResult86 ) + lerpResult149 );
				float2 uv_StaticAlphaTexture = IN.ase_texcoord2.xy * _StaticAlphaTexture_ST.xy + _StaticAlphaTexture_ST.zw;
				float cos243 = cos( radians( _StaticAlphaRotation ) );
				float sin243 = sin( radians( _StaticAlphaRotation ) );
				float2 rotator243 = mul( uv_StaticAlphaTexture - float2( 0.5,0.5 ) , float2x2( cos243 , -sin243 , sin243 , cos243 )) + float2( 0.5,0.5 );
				float dotResult256 = dot( tex2D( _StaticAlphaTexture, rotator243 ) , _StaticAlphaChannel );
				float temp_output_251_0 = saturate( dotResult256 );
				float lerpResult246 = lerp( temp_output_251_0 , saturate( ( 1.0 - temp_output_251_0 ) ) , _StaticAlphaInvert);
				float AdditionalAlpha247 = lerpResult246;
				float temp_output_81_0 = saturate( ( saturate( dotResult115 ) * temp_output_74_0 * AdditionalAlpha247 ) );
				float temp_output_101_0 = ( temp_output_81_0 * _AlphaBoldness );
				float lerpResult225 = lerp( temp_output_101_0 , saturate( round( temp_output_101_0 ) ) , _FlatAlpha);
				float4 screenPos = IN.ase_texcoord4;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float eyeDepth234 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float eyeDepth = IN.ase_texcoord5.x;
				float cameraDepthFade235 = (( eyeDepth -_ProjectionParams.y - 0.0 ) / 1.0);
				float lerpResult239 = lerp( 1.0 , saturate( ( ( eyeDepth234 - cameraDepthFade235 ) / _DepthFadeDivide ) ) , _UseDepthFade);
				
				float Alpha = saturate( ( lerpResult225 * IN.ase_color.a * saturate( lerpResult239 ) ) );
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

	
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18900
524;102;1334;650;6022.062;-1027.564;3.943433;True;False
Node;AmplifyShaderEditor.RangedFloatNode;182;-4831.441,1895.422;Inherit;False;Property;_DistortMaskRotation;Distort Mask Rotation;19;0;Create;True;0;0;0;False;0;False;0;45;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;181;-4953.408,1559.338;Inherit;True;Property;_DistortMask;Distort Mask;17;1;[Header];Create;True;1;Distort Mask;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleTimeNode;185;-4376.229,2218.083;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;186;-4666.236,1755.883;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;183;-4350.472,2301.841;Inherit;False;Property;_DistortPanSpeed;Distort Pan Speed;20;0;Create;True;0;0;0;False;0;False;0,0;0.25,0.25;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RadiansOpNode;184;-4568.237,1917.884;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;188;-4022.229,2044.084;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;189;-4337.595,1943.991;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;187;-4386.237,1781.883;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;190;-3855.931,1895.344;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;193;-3413.919,2109.404;Inherit;False;Property;_DistortMaskChannel;Distort Mask Channel;18;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;194;-3473.135,1869.513;Inherit;True;Property;_TextureSample6;Texture Sample 6;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;197;-3011.625,2050;Inherit;False;Property;_DistortionPower;Distortion Power;21;0;Create;True;0;0;0;False;0;False;0;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;195;-2981.711,1877.975;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;196;-2731.625,1922;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;129;-4783.844,-33.61034;Inherit;False;Property;_DissolveDirectionRotation;Dissolve Direction Rotation;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;130;-5203.246,-332.8102;Inherit;True;Property;_DissolveDirection;Dissolve Direction;12;1;[Header];Create;True;1;Dissolve Direction;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;138;-4885.846,60.98962;Inherit;False;2;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;209;-4937.917,203.3546;Inherit;False;Property;_MoveWithTexture;Move With Texture;16;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;131;-4657.844,-189.6104;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RadiansOpNode;132;-4543.843,-17.61036;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;192;-2533.407,1921.351;Inherit;False;Distortion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;200;-4345.434,146.0742;Inherit;False;192;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;210;-4592.917,93.55455;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;133;-4351.843,-177.6104;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-4501.974,678.0761;Inherit;False;Property;_DissolveMaskRotation;Dissolve Mask Rotation;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;63;-4603.757,319.5667;Inherit;True;Property;_DissolveMask;Dissolve Mask;7;1;[Header];Create;True;1;Dissolve Mask;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleAddOpNode;139;-4111.843,-81.61037;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;135;-3807.845,-369.6103;Inherit;True;Property;_TextureSample4;Texture Sample 4;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;152;-4046.761,1000.738;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;92;-4238.77,700.5384;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;250;-4602.178,2903.204;Inherit;False;Property;_StaticAlphaRotation;Static Alpha Rotation;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;252;-4829.928,2566.004;Inherit;True;Property;_StaticAlphaTexture;Static Alpha Texture;3;1;[Header];Create;True;1;Static Alpha;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.Vector2Node;154;-4079.324,1080.97;Inherit;False;Property;_DissolveMaskPanSpeed;Dissolve Mask Pan Speed;11;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector4Node;134;-3743.845,-129.6104;Inherit;False;Property;_DissolveDirectionChannel;Dissolve Direction Channel;13;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;93;-4336.769,538.5381;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;202;-3716.085,978.937;Inherit;False;192;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;136;-3311.845,-353.6102;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;248;-4456.178,2749.804;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;94;-4056.77,564.5381;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;84;-4010.431,726.6459;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;153;-3692.761,826.7382;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RadiansOpNode;255;-4358.179,2911.803;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;146;-3171.228,-335.5703;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;83;-3529.561,606.7538;Inherit;False;4;4;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;243;-4176.179,2775.804;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;245;-3623.524,2981.163;Inherit;False;Property;_StaticAlphaChannel;Static Alpha Channel;4;0;Create;True;0;0;0;False;0;False;1,0,0,0;1,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;141;-3034.359,-216.4409;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;66;-3214.967,680.8755;Inherit;False;Property;_DissolveMaskChannel;Dissolve Mask Channel;8;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,1.5,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;64;-3274.183,440.9854;Inherit;True;Property;_TextureSample3;Texture Sample 3;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;249;-3682.741,2741.272;Inherit;True;Property;_TextureSample7;Texture Sample 7;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;256;-3258.626,2770.921;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;143;-2856.682,-223.2741;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;142;-3026.386,-66.97019;Inherit;False;Property;_DissolveDirectionInvert;Dissolve Direction Invert;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-3644.705,-635.0155;Inherit;False;Property;_TextureRotation;Texture Rotation;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;2;-3872.455,-963.0154;Inherit;True;Property;_Texture;Texture;0;1;[Header];Create;True;1;Main Alpha;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.DotProductOpNode;116;-2782.759,449.4473;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;72;-2476.965,660.8755;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;251;-3173.292,2902.945;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;144;-2705.699,-315.5128;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;52;-3498.705,-791.0151;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RadiansOpNode;54;-3400.706,-629.0155;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;253;-3098.421,3036.073;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;77;-2881.057,30.75589;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;199;-3225.063,-528.9745;Inherit;False;192;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;53;-3218.706,-765.0151;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;145;-2522.031,-272.4654;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;87;-2337.148,766.0294;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;244;-3090.449,3183.544;Inherit;False;Property;_StaticAlphaInvert;Static Alpha Invert;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;254;-2920.744,3029.24;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;88;-2174.763,764.5561;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;147;-2369.349,-153.2989;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;140;-3029.231,-643.1213;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-2344.464,920.86;Inherit;False;Property;_DissolveMaskInvert;Dissolve Mask Invert;10;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;86;-2023.78,672.3174;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;148;-2062.261,-150.4445;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;6;-2666.051,-559.657;Inherit;False;Property;_TextureChannel;Texture Channel;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.5,0,0.5,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-2725.268,-799.5468;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;246;-2739.761,2925.001;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;90;-1835.359,753.3912;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;247;-2598.458,2796.986;Inherit;False;AdditionalAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;149;-1880.261,-96.4445;Inherit;False;3;0;FLOAT;-1;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;115;-2231.153,-775.899;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;257;-1484.516,-193.2066;Inherit;False;247;AdditionalAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-1612.281,125.2031;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;12;-1880.052,-571.657;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-1110.306,-237.2536;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenDepthNode;234;255.2987,813.2134;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;235;102.7283,991.3724;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;240;347.042,1230.823;Inherit;False;Property;_DepthFadeDivide;Depth Fade Divide;36;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;81;-410.2265,189.0497;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;236;487.4695,837.7327;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;102;149.7728,384.3555;Inherit;False;Property;_AlphaBoldness;Alpha Boldness;33;0;Create;True;0;0;0;False;0;False;1;2.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;101;334.2433,213.1518;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;237;630.4059,886.2444;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;745.75;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;238;788.3761,834.246;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;228;514.4946,434.6829;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;231;639.892,1026.816;Inherit;False;Property;_UseDepthFade;Use Depth Fade;35;1;[Header];Create;True;1;Depth Fade;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;229;682.3267,363.1801;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;239;939.827,749.5099;Inherit;True;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;230;652.5856,490.0969;Inherit;False;Property;_FlatAlpha;Flat Alpha;34;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;241;1224.122,746.2878;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;40;335.6935,-339.0652;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;225;961.3226,280.6687;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;242;1653.354,354.8839;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;128;1636.634,-181.6373;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;126;1316.199,-196.1824;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;114;-1411.903,-1231.454;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;41;650.4175,-311.5128;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;43;1491.387,66.19839;Inherit;False;Property;_Brightness;Brightness;32;1;[Header];Create;True;1;Brightness and Opacity;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;198;-2399.797,-995.5161;Inherit;False;192;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-71.31783,-280.1248;Inherit;False;Property;_GradientMapDisplacement;Gradient Map Displacement;28;0;Create;True;0;0;0;False;0;False;0;0.61;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;124;1114.651,-171.3531;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;123;800.04,-244.9492;Inherit;False;Property;_CoreColor;Core Color;31;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;163;-2689.291,-1395.585;Inherit;False;2;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;30;-363.3178,-438.1248;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;166;-2185.817,-1224.777;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-385.3179,-312.1248;Inherit;False;Property;_InvertGradient;Invert Gradient;29;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;80;-542.7728,-381.853;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;16;-1966.231,-1199.073;Inherit;False;Property;_GradientShapeChannel;Gradient Shape Channel;25;0;Create;True;0;0;0;False;0;False;0,0,0,0;1.25,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;168;612.4243,-962.8915;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;169;-720.3197,-1276.536;Inherit;True;Property;_ColorTexture;Color Texture;22;1;[Header];Create;True;1;Overlay Color;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;167;273.9433,-1166.968;Inherit;True;Property;_TextureSample5;Texture Sample 5;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RadiansOpNode;162;-2557.292,-1210.671;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;32;-13.31781,-512.1246;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;125;1357.696,-126.8626;Inherit;False;Property;_DifferentCoreColor;Different Core Color;30;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;37;760.3088,-464.6559;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;170;-592.4467,-1002.408;Inherit;False;Property;_ColorRotation;Color Rotation;23;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;962.0413,-330.4809;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-2974.216,-1311.579;Inherit;False;Property;_GradientShapeRotation;Gradient Shape Rotation;26;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;122;854.4377,-26.71929;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;173;-166.4467,-1132.407;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RadiansOpNode;171;-348.4471,-996.4084;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-2025.449,-1438.963;Inherit;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;24;24.60729,-784.7709;Inherit;True;Property;_GradientMap;Gradient Map;27;1;[Header];Create;True;1;Gradient Map;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.LerpOp;127;1509.085,-302.3303;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;13;-3025.837,-1592.447;Inherit;True;Property;_GradientShape;Gradient Shape;24;1;[Header];Create;True;1;Gradient Shape;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleAddOpNode;28;241.6822,-422.1248;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;157;2346.372,-433.9586;Inherit;False;Property;_Dst;Dst;41;0;Create;True;0;0;0;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;172;-446.4472,-1158.407;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;159;2090.372,-433.9586;Inherit;False;Property;_Src;Src;40;0;Create;True;0;0;0;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;158;2856.372,-435.9586;Inherit;False;Property;_ZTest;ZTest;39;0;Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;1843.115,-476.3185;Inherit;False;Property;_Cull;Cull;37;1;[Header];Create;True;1;Rendering;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;164;-2375.292,-1346.67;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;201;-112.346,-902.3562;Inherit;False;192;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;103;1914.016,294.7071;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;31;-201.3178,-468.1248;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;1727.388,-69.80159;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-1042.648,-1087.283;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;175;-404.0316,-926.8129;Inherit;False;2;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;416.193,-752.0449;Inherit;True;Property;_TextureSample2;Texture Sample 2;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;156;2602.372,-433.9586;Inherit;False;Property;_ZWrite;ZWrite;38;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;22;-1200.232,-1215.073;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;174;83.57852,-1035.155;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;178;2519.028,-75.02066;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;177;2519.028,-75.02066;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;/_Kass_/SH_VFX_PanDissolvePremult_NoZW;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;2;True;95;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;0;False;True;3;1;True;159;10;True;157;3;1;True;159;10;True;157;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;True;156;True;3;True;158;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;0;Hidden/InternalErrorShader;0;0;Standard;22;Surface;1;  Blend;1;Two Sided;0;Cast Shadows;0;  Use Shadow Threshold;0;Receive Shadows;0;GPU Instancing;1;LOD CrossFade;0;Built-in Fog;0;DOTS Instancing;0;Meta Pass;0;Extra Pre Pass;0;Tessellation;0;  Phong;0;  Strength;0.5,False,-1;  Type;0;  Tess;16,False,-1;  Min;10,False,-1;  Max;25,False,-1;  Edge Length;16,False,-1;  Max Displacement;25,False,-1;Vertex Position,InvertActionOnDeselection;1;0;5;False;True;False;True;False;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;179;2519.028,-75.02066;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;180;2519.028,-75.02066;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;176;2519.028,-75.02066;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;186;2;181;0
WireConnection;184;0;182;0
WireConnection;188;0;185;0
WireConnection;188;1;183;0
WireConnection;189;2;181;0
WireConnection;187;0;186;0
WireConnection;187;2;184;0
WireConnection;190;0;187;0
WireConnection;190;1;188;0
WireConnection;190;2;189;2
WireConnection;194;0;181;0
WireConnection;194;1;190;0
WireConnection;195;0;194;0
WireConnection;195;1;193;0
WireConnection;196;0;195;0
WireConnection;196;1;197;0
WireConnection;138;2;130;0
WireConnection;131;2;130;0
WireConnection;132;0;129;0
WireConnection;192;0;196;0
WireConnection;210;0;138;0
WireConnection;210;1;209;0
WireConnection;133;0;131;0
WireConnection;133;2;132;0
WireConnection;139;0;133;0
WireConnection;139;1;210;0
WireConnection;139;2;200;0
WireConnection;135;0;130;0
WireConnection;135;1;139;0
WireConnection;92;0;91;0
WireConnection;93;2;63;0
WireConnection;136;0;135;0
WireConnection;136;1;134;0
WireConnection;248;2;252;0
WireConnection;94;0;93;0
WireConnection;94;2;92;0
WireConnection;84;2;63;0
WireConnection;153;0;152;0
WireConnection;153;1;154;0
WireConnection;255;0;250;0
WireConnection;146;0;136;0
WireConnection;83;0;94;0
WireConnection;83;1;153;0
WireConnection;83;2;84;2
WireConnection;83;3;202;0
WireConnection;243;0;248;0
WireConnection;243;2;255;0
WireConnection;141;0;146;0
WireConnection;64;0;63;0
WireConnection;64;1;83;0
WireConnection;249;0;252;0
WireConnection;249;1;243;0
WireConnection;256;0;249;0
WireConnection;256;1;245;0
WireConnection;143;0;141;0
WireConnection;116;0;64;0
WireConnection;116;1;66;0
WireConnection;72;0;116;0
WireConnection;251;0;256;0
WireConnection;144;0;146;0
WireConnection;144;1;143;0
WireConnection;144;2;142;0
WireConnection;52;2;2;0
WireConnection;54;0;55;0
WireConnection;253;0;251;0
WireConnection;53;0;52;0
WireConnection;53;2;54;0
WireConnection;145;0;144;0
WireConnection;87;0;72;0
WireConnection;254;0;253;0
WireConnection;88;0;87;0
WireConnection;147;0;145;0
WireConnection;147;1;77;1
WireConnection;140;0;53;0
WireConnection;140;1;199;0
WireConnection;86;0;72;0
WireConnection;86;1;88;0
WireConnection;86;2;89;0
WireConnection;148;0;147;0
WireConnection;1;0;2;0
WireConnection;1;1;140;0
WireConnection;246;0;251;0
WireConnection;246;1;254;0
WireConnection;246;2;244;0
WireConnection;90;0;86;0
WireConnection;247;0;246;0
WireConnection;149;2;148;0
WireConnection;115;0;1;0
WireConnection;115;1;6;0
WireConnection;74;0;90;0
WireConnection;74;1;149;0
WireConnection;12;0;115;0
WireConnection;78;0;12;0
WireConnection;78;1;74;0
WireConnection;78;2;257;0
WireConnection;81;0;78;0
WireConnection;236;0;234;0
WireConnection;236;1;235;0
WireConnection;101;0;81;0
WireConnection;101;1;102;0
WireConnection;237;0;236;0
WireConnection;237;1;240;0
WireConnection;238;0;237;0
WireConnection;228;0;101;0
WireConnection;229;0;228;0
WireConnection;239;1;238;0
WireConnection;239;2;231;0
WireConnection;241;0;239;0
WireConnection;225;0;101;0
WireConnection;225;1;229;0
WireConnection;225;2;230;0
WireConnection;242;0;225;0
WireConnection;242;1;40;4
WireConnection;242;2;241;0
WireConnection;128;0;127;0
WireConnection;126;0;124;0
WireConnection;114;0;14;0
WireConnection;114;1;16;0
WireConnection;41;0;40;0
WireConnection;124;0;39;0
WireConnection;124;1;123;0
WireConnection;124;2;122;0
WireConnection;163;2;13;0
WireConnection;30;0;80;0
WireConnection;166;0;164;0
WireConnection;166;1;198;0
WireConnection;80;0;79;0
WireConnection;168;0;167;0
WireConnection;167;0;169;0
WireConnection;167;1;174;0
WireConnection;162;0;51;0
WireConnection;32;0;31;0
WireConnection;32;1;80;0
WireConnection;32;2;33;0
WireConnection;37;0;23;0
WireConnection;39;0;37;0
WireConnection;39;1;41;0
WireConnection;39;2;168;0
WireConnection;122;0;81;0
WireConnection;173;0;172;0
WireConnection;173;2;171;0
WireConnection;171;0;170;0
WireConnection;14;0;13;0
WireConnection;14;1;166;0
WireConnection;127;0;39;0
WireConnection;127;1;126;0
WireConnection;127;2;125;0
WireConnection;28;0;32;0
WireConnection;28;1;29;0
WireConnection;172;2;169;0
WireConnection;164;0;163;0
WireConnection;164;2;162;0
WireConnection;103;0;242;0
WireConnection;31;0;30;0
WireConnection;42;0;128;0
WireConnection;42;1;43;0
WireConnection;79;0;22;0
WireConnection;79;1;74;0
WireConnection;175;2;169;0
WireConnection;23;0;24;0
WireConnection;23;1;28;0
WireConnection;22;0;114;0
WireConnection;174;0;173;0
WireConnection;174;1;175;0
WireConnection;174;2;201;0
WireConnection;177;2;42;0
WireConnection;177;3;103;0
ASEEND*/
//CHKSM=2235644762AD47DAA0016A921F953B8466B4EC96