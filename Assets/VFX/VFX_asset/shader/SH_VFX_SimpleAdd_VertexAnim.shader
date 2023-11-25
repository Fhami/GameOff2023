// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "/_Kass_/SH_VFX_SimpleAdd_VertexAnim"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin][Header(Main Alpha)]_Texture("Texture", 2D) = "white" {}
		_TextureChannel("Texture Channel", Vector) = (0,0,0,0)
		_TextureRotation("Texture Rotation", Float) = 0
		[Header(Overlay Color)]_ColorTexture("Color Texture", 2D) = "white" {}
		_ColorRotation("Color Rotation", Float) = 0
		[Header(Gradient Shape)]_GradientShape("Gradient Shape", 2D) = "white" {}
		_GradientShapeChannel("Gradient Shape Channel", Vector) = (0,0,0,0)
		_GradientShapeRotation("Gradient Shape Rotation", Float) = 0
		[Header(Gradient Map)]_GradientMap("Gradient Map", 2D) = "white" {}
		_GradientMapDisplacement("Gradient Map Displacement", Float) = 0
		_InvertGradient("Invert Gradient", Float) = 0
		[Header(Different Center Color)]_CorePower("Core Power", Float) = 0
		_CoreIntensity("Core Intensity", Float) = 0
		_DifferentCoreColor("Different Core Color", Float) = 0
		_CoreColor("Core Color", Color) = (0,0,0,0)
		[Header(Brightness and Opacity)]_Brightness("Brightness", Float) = 1
		_AlphaBoldness("Alpha Boldness", Float) = 1
		_FlatAlpha("Flat Alpha", Range( 0 , 1)) = 0
		[Header(Depth Fade)]_UseDepthFade("Use Depth Fade", Float) = 1
		_DepthFadeDivide("Depth Fade Divide", Float) = 1
		[Header(Vertex Displacement)]_VertexDisplacementAmount("Vertex Displacement Amount", Vector) = (0,0,0,0)
		_UseVertexNormals("Use Vertex Normals", Range( 0 , 1)) = 1
		_ClampDisplacement("Clamp Displacement", Range( 0 , 1)) = 0
		_CustomData1XAffectsStrength("Custom Data 1 X Affects Strength", Range( 0 , 1)) = 1
		_CustomData1YAffectsSpeed2("Custom Data 1 Y Affects Speed", Range( 0 , 1)) = 1
		[Header(UV Based Sin Wave Displacement)]_UVSinWaveStrength("UV Sin Wave Strength", Vector) = (0,0,0,0)
		_UVSinWaveFrequency("UV Sin Wave Frequency", Float) = 1
		_UVSinWaveSpeed("UV Sin Wave Speed", Float) = 1
		[Header(Vertex Displacement Noise)]_VertexDisplacementNoise2("Vertex Displacement Noise", 2D) = "white" {}
		_VertexDisplacementNoiseChannel("Vertex Displacement Noise Channel", Vector) = (0,0,0,0)
		_VertexDisplacementNoisePanSpeed("Vertex Displacement Noise  Pan Speed", Vector) = (0,0,0,0)
		_VertexDisplacementNoiseStrength("Vertex Displacement Noise Strength", Float) = 0
		_VertexDisplacementNoiseRotation("Vertex Displacement Noise Rotation", Float) = 0
		[Header(UV Based Displacement Mask)]_UVBasedDisplacementMaskChannel("UV Based Displacement Mask Channel", Vector) = (0,0,0,0)
		_UVBasedDisplacementMaskDisplacement("UV Based Displacement Mask Displacement", Float) = 0
		_UVBasedDisplacementMaskSoften("UV Based Displacement Mask Soften", Float) = 1
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

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_COLOR


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
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
				float4 ase_color : COLOR;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _GradientShape_ST;
			float4 _GradientShapeChannel;
			float4 _VertexDisplacementNoise2_ST;
			float4 _UVBasedDisplacementMaskChannel;
			float4 _VertexDisplacementNoiseChannel;
			float4 _TextureChannel;
			float4 _ColorTexture_ST;
			float4 _Texture_ST;
			float4 _CoreColor;
			float3 _VertexDisplacementAmount;
			float2 _VertexDisplacementNoisePanSpeed;
			float2 _UVSinWaveStrength;
			float _GradientShapeRotation;
			float _InvertGradient;
			float _GradientMapDisplacement;
			float _TextureRotation;
			float _CoreIntensity;
			float _DifferentCoreColor;
			float _Brightness;
			float _AlphaBoldness;
			float _FlatAlpha;
			float _CorePower;
			float _ZWrite;
			float _UseVertexNormals;
			float _DepthFadeDivide;
			float _CustomData1XAffectsStrength;
			float _UVBasedDisplacementMaskDisplacement;
			float _UVBasedDisplacementMaskSoften;
			float _ClampDisplacement;
			float _UVSinWaveSpeed;
			float _UVSinWaveFrequency;
			float _VertexDisplacementNoiseStrength;
			float _CustomData1YAffectsSpeed2;
			float _VertexDisplacementNoiseRotation;
			float _ZTest;
			float _Src;
			float _Dst;
			float _Cull;
			float _ColorRotation;
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
			sampler2D _VertexDisplacementNoise2;
			sampler2D _ColorTexture;
			sampler2D _GradientMap;
			sampler2D _GradientShape;
			sampler2D _Texture;
			uniform float4 _CameraDepthTexture_TexelSize;


						
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 uv_VertexDisplacementNoise2 = v.ase_texcoord * _VertexDisplacementNoise2_ST.xy + _VertexDisplacementNoise2_ST.zw;
				float cos275 = cos( radians( _VertexDisplacementNoiseRotation ) );
				float sin275 = sin( radians( _VertexDisplacementNoiseRotation ) );
				float2 rotator275 = mul( uv_VertexDisplacementNoise2 - float2( 0.5,0.5 ) , float2x2( cos275 , -sin275 , sin275 , cos275 )) + float2( 0.5,0.5 );
				float4 texCoord259 = v.ase_texcoord;
				texCoord259.xy = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float lerpResult260 = lerp( 1.0 , texCoord259.w , _CustomData1YAffectsSpeed2);
				float SpeedVariant264 = lerpResult260;
				float4 uvs4_VertexDisplacementNoise2 = v.ase_texcoord;
				uvs4_VertexDisplacementNoise2.xy = v.ase_texcoord.xy * _VertexDisplacementNoise2_ST.xy + _VertexDisplacementNoise2_ST.zw;
				float4 uv2s4_VertexDisplacementNoise2 = v.ase_texcoord1;
				uv2s4_VertexDisplacementNoise2.xy = v.ase_texcoord1.xy * _VertexDisplacementNoise2_ST.xy + _VertexDisplacementNoise2_ST.zw;
				float4 appendResult277 = (float4(uv2s4_VertexDisplacementNoise2.z , uv2s4_VertexDisplacementNoise2.w , 0.0 , 0.0));
				float dotResult287 = dot( tex2Dlod( _VertexDisplacementNoise2, float4( ( float4( rotator275, 0.0 , 0.0 ) + float4( ( _TimeParameters.x * _VertexDisplacementNoisePanSpeed * SpeedVariant264 ), 0.0 , 0.0 ) + uvs4_VertexDisplacementNoise2.w + appendResult277 ).xy, 0, 0.0) ) , _VertexDisplacementNoiseChannel );
				float2 texCoord274 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float mulTime280 = _TimeParameters.x * ( _UVSinWaveSpeed * SpeedVariant264 );
				float dotResult294 = dot( sin( ( ( texCoord274 * _UVSinWaveFrequency ) + mulTime280 ) ) , _UVSinWaveStrength );
				float temp_output_299_0 = ( ( dotResult287 * _VertexDisplacementNoiseStrength ) + dotResult294 );
				float lerpResult300 = lerp( -1.0 , 1.0 , temp_output_299_0);
				float lerpResult305 = lerp( 0.0 , 1.0 , temp_output_299_0);
				float lerpResult307 = lerp( lerpResult300 , lerpResult305 , _ClampDisplacement);
				float2 texCoord290 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float dotResult295 = dot( float4( texCoord290, 0.0 , 0.0 ) , _UVBasedDisplacementMaskChannel );
				float smoothstepResult308 = smoothstep( 0.0 , _UVBasedDisplacementMaskSoften , saturate( ( dotResult295 + _UVBasedDisplacementMaskDisplacement ) ));
				float lerpResult297 = lerp( 1.0 , texCoord259.z , _CustomData1XAffectsStrength);
				float StrengthVariant302 = lerpResult297;
				float VertexDisplacement310 = ( lerpResult307 * smoothstepResult308 * StrengthVariant302 );
				float3 lerpResult313 = lerp( float3( 1,1,1 ) , v.ase_normal , _UseVertexNormals);
				
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord4 = screenPos;
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(v.vertex.xyz));
				float eyeDepth = -objectToViewPos.z;
				o.ase_texcoord5.x = eyeDepth;
				
				o.ase_texcoord3 = v.ase_texcoord;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord5.yzw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( VertexDisplacement310 * lerpResult313 * _VertexDisplacementAmount );
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
				float2 uv_ColorTexture = IN.ase_texcoord3.xy * _ColorTexture_ST.xy + _ColorTexture_ST.zw;
				float cos107 = cos( radians( _ColorRotation ) );
				float sin107 = sin( radians( _ColorRotation ) );
				float2 rotator107 = mul( uv_ColorTexture - float2( 0.5,0.5 ) , float2x2( cos107 , -sin107 , sin107 , cos107 )) + float2( 0.5,0.5 );
				float2 uv_GradientShape = IN.ase_texcoord3.xy * _GradientShape_ST.xy + _GradientShape_ST.zw;
				float cos101 = cos( radians( _GradientShapeRotation ) );
				float sin101 = sin( radians( _GradientShapeRotation ) );
				float2 rotator101 = mul( uv_GradientShape - float2( 0.5,0.5 ) , float2x2( cos101 , -sin101 , sin101 , cos101 )) + float2( 0.5,0.5 );
				float dotResult98 = dot( tex2D( _GradientShape, rotator101 ) , _GradientShapeChannel );
				float temp_output_116_0 = saturate( dotResult98 );
				float lerpResult118 = lerp( saturate( ( 1.0 - temp_output_116_0 ) ) , temp_output_116_0 , _InvertGradient);
				float2 temp_cast_1 = (( lerpResult118 + _GradientMapDisplacement )).xx;
				float3 temp_output_104_0 = ( (tex2D( _ColorTexture, rotator107 )).rgb * (tex2D( _GradientMap, temp_cast_1 )).rgb * (IN.ase_color).rgb );
				float2 uv_Texture = IN.ase_texcoord3.xy * _Texture_ST.xy + _Texture_ST.zw;
				float cos138 = cos( radians( _TextureRotation ) );
				float sin138 = sin( radians( _TextureRotation ) );
				float2 rotator138 = mul( uv_Texture - float2( 0.5,0.5 ) , float2x2( cos138 , -sin138 , sin138 , cos138 )) + float2( 0.5,0.5 );
				float dotResult126 = dot( tex2D( _Texture, rotator138 ) , _TextureChannel );
				float temp_output_86_0 = ( pow( dotResult126 , _CorePower ) * _CoreIntensity );
				float4 lerpResult105 = lerp( float4( temp_output_104_0 , 0.0 ) , _CoreColor , saturate( temp_output_86_0 ));
				float4 lerpResult76 = lerp( float4( temp_output_104_0 , 0.0 ) , saturate( lerpResult105 ) , _DifferentCoreColor);
				float temp_output_186_0 = ( saturate( ( dotResult126 + temp_output_86_0 ) ) * _AlphaBoldness );
				float lerpResult184 = lerp( temp_output_186_0 , saturate( round( temp_output_186_0 ) ) , _FlatAlpha);
				float4 screenPos = IN.ase_texcoord4;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float eyeDepth174 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float eyeDepth = IN.ase_texcoord5.x;
				float cameraDepthFade175 = (( eyeDepth -_ProjectionParams.y - 0.0 ) / 1.0);
				float lerpResult179 = lerp( 1.0 , saturate( ( ( eyeDepth174 - cameraDepthFade175 ) / _DepthFadeDivide ) ) , _UseDepthFade);
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( saturate( lerpResult76 ) * _Brightness * saturate( ( lerpResult184 * saturate( lerpResult179 ) * IN.ase_color.a ) ) ).rgb;
				float Alpha = 1;
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
			#define _RECEIVE_SHADOWS_OFF 1
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
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
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _GradientShape_ST;
			float4 _GradientShapeChannel;
			float4 _VertexDisplacementNoise2_ST;
			float4 _UVBasedDisplacementMaskChannel;
			float4 _VertexDisplacementNoiseChannel;
			float4 _TextureChannel;
			float4 _ColorTexture_ST;
			float4 _Texture_ST;
			float4 _CoreColor;
			float3 _VertexDisplacementAmount;
			float2 _VertexDisplacementNoisePanSpeed;
			float2 _UVSinWaveStrength;
			float _GradientShapeRotation;
			float _InvertGradient;
			float _GradientMapDisplacement;
			float _TextureRotation;
			float _CoreIntensity;
			float _DifferentCoreColor;
			float _Brightness;
			float _AlphaBoldness;
			float _FlatAlpha;
			float _CorePower;
			float _ZWrite;
			float _UseVertexNormals;
			float _DepthFadeDivide;
			float _CustomData1XAffectsStrength;
			float _UVBasedDisplacementMaskDisplacement;
			float _UVBasedDisplacementMaskSoften;
			float _ClampDisplacement;
			float _UVSinWaveSpeed;
			float _UVSinWaveFrequency;
			float _VertexDisplacementNoiseStrength;
			float _CustomData1YAffectsSpeed2;
			float _VertexDisplacementNoiseRotation;
			float _ZTest;
			float _Src;
			float _Dst;
			float _Cull;
			float _ColorRotation;
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
			sampler2D _VertexDisplacementNoise2;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 uv_VertexDisplacementNoise2 = v.ase_texcoord * _VertexDisplacementNoise2_ST.xy + _VertexDisplacementNoise2_ST.zw;
				float cos275 = cos( radians( _VertexDisplacementNoiseRotation ) );
				float sin275 = sin( radians( _VertexDisplacementNoiseRotation ) );
				float2 rotator275 = mul( uv_VertexDisplacementNoise2 - float2( 0.5,0.5 ) , float2x2( cos275 , -sin275 , sin275 , cos275 )) + float2( 0.5,0.5 );
				float4 texCoord259 = v.ase_texcoord;
				texCoord259.xy = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float lerpResult260 = lerp( 1.0 , texCoord259.w , _CustomData1YAffectsSpeed2);
				float SpeedVariant264 = lerpResult260;
				float4 uvs4_VertexDisplacementNoise2 = v.ase_texcoord;
				uvs4_VertexDisplacementNoise2.xy = v.ase_texcoord.xy * _VertexDisplacementNoise2_ST.xy + _VertexDisplacementNoise2_ST.zw;
				float4 uv2s4_VertexDisplacementNoise2 = v.ase_texcoord1;
				uv2s4_VertexDisplacementNoise2.xy = v.ase_texcoord1.xy * _VertexDisplacementNoise2_ST.xy + _VertexDisplacementNoise2_ST.zw;
				float4 appendResult277 = (float4(uv2s4_VertexDisplacementNoise2.z , uv2s4_VertexDisplacementNoise2.w , 0.0 , 0.0));
				float dotResult287 = dot( tex2Dlod( _VertexDisplacementNoise2, float4( ( float4( rotator275, 0.0 , 0.0 ) + float4( ( _TimeParameters.x * _VertexDisplacementNoisePanSpeed * SpeedVariant264 ), 0.0 , 0.0 ) + uvs4_VertexDisplacementNoise2.w + appendResult277 ).xy, 0, 0.0) ) , _VertexDisplacementNoiseChannel );
				float2 texCoord274 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float mulTime280 = _TimeParameters.x * ( _UVSinWaveSpeed * SpeedVariant264 );
				float dotResult294 = dot( sin( ( ( texCoord274 * _UVSinWaveFrequency ) + mulTime280 ) ) , _UVSinWaveStrength );
				float temp_output_299_0 = ( ( dotResult287 * _VertexDisplacementNoiseStrength ) + dotResult294 );
				float lerpResult300 = lerp( -1.0 , 1.0 , temp_output_299_0);
				float lerpResult305 = lerp( 0.0 , 1.0 , temp_output_299_0);
				float lerpResult307 = lerp( lerpResult300 , lerpResult305 , _ClampDisplacement);
				float2 texCoord290 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float dotResult295 = dot( float4( texCoord290, 0.0 , 0.0 ) , _UVBasedDisplacementMaskChannel );
				float smoothstepResult308 = smoothstep( 0.0 , _UVBasedDisplacementMaskSoften , saturate( ( dotResult295 + _UVBasedDisplacementMaskDisplacement ) ));
				float lerpResult297 = lerp( 1.0 , texCoord259.z , _CustomData1XAffectsStrength);
				float StrengthVariant302 = lerpResult297;
				float VertexDisplacement310 = ( lerpResult307 * smoothstepResult308 * StrengthVariant302 );
				float3 lerpResult313 = lerp( float3( 1,1,1 ) , v.ase_normal , _UseVertexNormals);
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( VertexDisplacement310 * lerpResult313 * _VertexDisplacementAmount );
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

				
				float Alpha = 1;
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
-1127;74;1334;838;2000.151;176.0423;1.722983;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;259;-3935.73,2430.88;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;258;-3832.547,2309.528;Inherit;False;Property;_CustomData1YAffectsSpeed2;Custom Data 1 Y Affects Speed;24;0;Create;False;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;261;-2906.568,1401.407;Inherit;False;3676.525;1880.749;Vertex Animation Noise;45;310;309;308;307;306;305;304;303;301;300;299;298;295;294;293;292;291;290;289;288;287;286;285;284;283;282;281;280;279;278;277;276;275;274;273;272;271;270;269;268;267;266;265;263;262;;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;260;-3448.688,2269.898;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;262;-2887.565,1443.197;Inherit;True;Property;_VertexDisplacementNoise2;Vertex Displacement Noise;28;1;[Header];Create;False;1;Vertex Displacement Noise;0;0;False;0;False;None;359096eeeced08a4fb16ffd42838845f;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;263;-2830.801,1830.446;Inherit;False;Property;_VertexDisplacementNoiseRotation;Vertex Displacement Noise Rotation;32;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;264;-3244.688,2260.898;Inherit;False;SpeedVariant;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;269;-2471.396,1845.108;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;272;-1926.704,2647.553;Inherit;False;Property;_UVSinWaveSpeed;UV Sin Wave Speed;27;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;268;-2335.39,2043.309;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;271;-2337.633,2137.067;Inherit;False;Property;_VertexDisplacementNoisePanSpeed;Vertex Displacement Noise  Pan Speed;30;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;267;-2569.395,1683.108;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;265;-2253.746,2346.863;Inherit;False;264;SpeedVariant;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;270;-1876.334,2784.557;Inherit;False;264;SpeedVariant;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;266;-2284.23,1451.407;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;275;-2289.397,1709.108;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;274;-1814.705,2324.953;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;273;-2323.956,1819.216;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;276;-1676.135,2680.557;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;279;-1925.392,1971.308;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;278;-1894.705,2514.953;Inherit;False;Property;_UVSinWaveFrequency;UV Sin Wave Frequency;26;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;277;-1980.571,1459.436;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleTimeNode;280;-1574.706,2516.953;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;282;-1510.706,2340.953;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;10;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;281;-1759.094,1822.568;Inherit;False;4;4;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;284;-1376.298,1796.736;Inherit;True;Property;_TextureSample10;Texture Sample 10;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;283;-1350.706,2340.953;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;285;-1311.833,2036.63;Inherit;False;Property;_VertexDisplacementNoiseChannel;Vertex Displacement Noise Channel;29;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;288;-1085.219,2901.114;Inherit;False;Property;_UVBasedDisplacementMaskChannel;UV Based Displacement Mask Channel;33;1;[Header];Create;True;1;UV Based Displacement Mask;0;0;False;0;False;0,0,0,0;1,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;289;-1191.606,2219.796;Inherit;False;Property;_VertexDisplacementNoiseStrength;Vertex Displacement Noise Strength;31;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;286;-1214.261,2341.076;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;291;-1468.511,2680.485;Inherit;False;Property;_UVSinWaveStrength;UV Sin Wave Strength;25;1;[Header];Create;True;1;UV Based Sin Wave Displacement;0;0;False;0;False;0,0;1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DotProductOpNode;287;-980.8733,1809.2;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;290;-1072.768,2709.901;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;294;-880.0024,2438.931;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;1,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;293;-999.1503,3105.46;Inherit;False;Property;_UVBasedDisplacementMaskDisplacement;UV Based Displacement Mask Displacement;34;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;295;-740.7357,2759.473;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;296;-3799.223,2676.771;Inherit;False;Property;_CustomData1XAffectsStrength;Custom Data 1 X Affects Strength;23;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;292;-735.6055,1857.796;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;297;-3442.688,2635.899;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;298;-590.5779,3084.938;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;299;-545.2499,2108.708;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;305;-312.42,1787.715;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;301;-448.1057,2959.459;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;304;-514.1057,3181.459;Inherit;False;Property;_UVBasedDisplacementMaskSoften;UV Based Displacement Mask Soften;35;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;303;-392.42,1689.715;Inherit;False;Property;_ClampDisplacement;Clamp Displacement;22;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;302;-3244.688,2638.899;Inherit;False;StrengthVariant;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;300;-332.9482,1925.164;Inherit;False;3;0;FLOAT;-1;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;307;-102.4199,1795.715;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;308;-238.1057,2841.458;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;306;-407.6768,2241.75;Inherit;False;302;StrengthVariant;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;309;-61.67987,2040.775;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;314;648.512,1039.047;Inherit;False;Property;_UseVertexNormals;Use Vertex Normals;21;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;310;110.6751,1798.87;Inherit;False;VertexDisplacement;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;311;690.5517,832.0568;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;313;1021.081,825.6139;Inherit;False;3;0;FLOAT3;1,1,1;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;315;1004.323,1253.535;Inherit;False;Property;_VertexDisplacementAmount;Vertex Displacement Amount;20;1;[Header];Create;True;1;Vertex Displacement;0;0;False;0;False;0,0,0;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;312;806.872,661.9744;Inherit;False;310;VertexDisplacement;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;178;298.9942,844.6931;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;100;-2269.268,-455.09;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;132;-1772.814,-465.6055;Inherit;False;Property;_GradientShapeChannel;Gradient Shape Channel;6;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;116;-1130.521,-374.8206;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;79;1188.4,-162.607;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;185;-429.4622,604.2207;Inherit;False;Property;_FlatAlpha;Flat Alpha;17;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-738.6611,-900.308;Inherit;False;Property;_ColorRotation;Color Rotation;4;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;125;-889.1999,-1184.606;Inherit;True;Property;_ColorTexture;Color Texture;3;1;[Header];Create;True;1;Overlay Color;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SaturateNode;182;-399.721,477.3039;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;179;450.4449,759.957;Inherit;True;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;106;62.4616,-428.677;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;1238.077,-63.43907;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RoundOpNode;183;-567.5531,548.8067;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;109;-270.9781,-360.6806;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;136;354.8574,-852.2254;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;181;537.2037,577.1567;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;115;-837.9782,-438.6806;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;112;1008.103,344.5316;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;124;-592.6614,-1056.307;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;157;1576.511,-715.4269;Inherit;False;Property;_ZWrite;ZWrite;37;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;135;-841.0867,309.4075;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;130;384.7512,-285.808;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;75;901.9648,-199.1521;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;108;386.0715,-376.6457;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;105;760.417,-78.32274;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;80;993.318,-32.62639;Inherit;False;Property;_Brightness;Brightness;15;1;[Header];Create;True;1;Brightness and Opacity;0;0;False;0;False;1;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;129;-2868.836,20.70718;Inherit;True;Property;_Texture;Texture;0;1;[Header];Create;True;1;Main Alpha;0;0;False;0;False;None;500f8a2f183eb2240a782fe49c37aa1d;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;114;-897.9782,-250.6806;Inherit;False;Property;_InvertGradient;Invert Gradient;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;117;-583.9781,-218.6806;Inherit;False;Property;_GradientMapDisplacement;Gradient Map Displacement;9;0;Create;True;0;0;0;False;0;False;0;-0.51;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;98;-1381.646,-479.0361;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;121;771.9019,-749.282;Inherit;False;Property;_Cull;Cull;36;1;[Header];Create;True;1;Rendering;0;0;True;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;118;-649.9781,-482.6806;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;113;-999.9781,-408.6806;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;120;-291.1222,-781.3338;Inherit;True;Property;_GradientMap;Gradient Map;8;1;[Header];Create;True;1;Gradient Map;0;0;False;0;False;None;b05e1593bbede88428883f011d607ac3;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RotatorNode;101;-2087.267,-591.0903;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;184;-120.7252,394.7925;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-1160.116,640.0095;Inherit;False;Property;_AlphaBoldness;Alpha Boldness;16;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;103;500.2039,66.3109;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;123;-494.6613,-894.308;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;119;14.34905,-674.3876;Inherit;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;99;-2673.34,-825.0892;Inherit;True;Property;_GradientShape;Gradient Shape;5;1;[Header];Create;True;1;Gradient Shape;0;0;False;0;False;None;59ba710e7146e4041b2f3f488e0703bb;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;180;-142.3401,1241.271;Inherit;False;Property;_DepthFadeDivide;Depth Fade Divide;19;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;83;-1105.019,242.604;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;634.7449,390.9565;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;177;141.0238,896.6915;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;745.75;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;122;-72.98848,-1069.069;Inherit;True;Property;_TextureSample3;Texture Sample 3;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;158;1322.511,-717.4269;Inherit;False;Property;_Dst;Dst;40;0;Create;True;0;0;0;True;0;False;10;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;102;-1832.032,-705.4958;Inherit;True;Property;_TextureSample2;Texture Sample 2;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;107;-312.6609,-1030.307;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;81;445.8063,-151.9188;Inherit;False;Property;_CoreColor;Core Color;14;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;76;1094.851,-305.3001;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;171;150.51,1037.263;Inherit;False;Property;_UseDepthFade;Use Depth Fade;18;1;[Header];Create;True;1;Depth Fade;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;160;1064.511,-715.4269;Inherit;False;Property;_Src;Src;39;0;Create;True;0;0;0;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;140;-1651.129,603.5795;Inherit;False;Property;_CoreIntensity;Core Intensity;12;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-2513.271,-461.0901;Inherit;False;Property;_GradientShapeRotation;Gradient Shape Rotation;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;655.0266,-310.9217;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;176;-1.912445,848.1799;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;74;-2367.268,-617.0903;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;316;1340.534,660.3117;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;159;1834.511,-717.4269;Inherit;False;Property;_ZTest;ZTest;38;0;Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;128;-2572.296,152.4865;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;82;943.4619,-129.8322;Inherit;False;Property;_DifferentCoreColor;Different Core Color;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;-1152.641,435.9595;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;85;-1325.956,371.7473;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;175;-386.6537,1001.82;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;126;-1638.4,142.6949;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;133;-2474.296,314.4864;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-1625.092,443.9026;Inherit;False;Property;_CorePower;Core Power;11;1;[Header];Create;False;1;Different Center Color;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;127;-2071.739,360.816;Inherit;False;Property;_TextureChannel;Texture Channel;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;134;-2052.625,139.7253;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;138;-2292.296,178.4865;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;131;-2718.298,308.4864;Inherit;False;Property;_TextureRotation;Texture Rotation;2;0;Create;True;0;0;0;False;0;False;0;-90;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenDepthNode;174;-234.0835,823.6605;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-704.9964,414.14;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;162;1799.307,34.03413;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;/_Kass_/SH_VFX_SimpleAdd_VertexAnim;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;True;True;2;True;121;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;0;True;True;4;1;True;160;1;True;158;3;1;True;160;10;True;158;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;True;157;True;3;True;159;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;0;Hidden/InternalErrorShader;0;0;Standard;22;Surface;1;  Blend;2;Two Sided;0;Cast Shadows;0;  Use Shadow Threshold;0;Receive Shadows;0;GPU Instancing;1;LOD CrossFade;0;Built-in Fog;0;DOTS Instancing;0;Meta Pass;0;Extra Pre Pass;0;Tessellation;0;  Phong;0;  Strength;0.5,False,-1;  Type;0;  Tess;16,False,-1;  Min;10,False,-1;  Max;25,False,-1;  Edge Length;16,False,-1;  Max Displacement;25,False,-1;Vertex Position,InvertActionOnDeselection;1;0;5;False;True;False;True;False;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;161;1799.307,34.03413;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;165;1799.307,34.03413;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;163;1799.307,34.03413;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;164;1799.307,34.03413;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;260;1;259;4
WireConnection;260;2;258;0
WireConnection;264;0;260;0
WireConnection;269;0;263;0
WireConnection;267;2;262;0
WireConnection;266;2;262;0
WireConnection;275;0;267;0
WireConnection;275;2;269;0
WireConnection;273;2;262;0
WireConnection;276;0;272;0
WireConnection;276;1;270;0
WireConnection;279;0;268;0
WireConnection;279;1;271;0
WireConnection;279;2;265;0
WireConnection;277;0;266;3
WireConnection;277;1;266;4
WireConnection;280;0;276;0
WireConnection;282;0;274;0
WireConnection;282;1;278;0
WireConnection;281;0;275;0
WireConnection;281;1;279;0
WireConnection;281;2;273;4
WireConnection;281;3;277;0
WireConnection;284;0;262;0
WireConnection;284;1;281;0
WireConnection;283;0;282;0
WireConnection;283;1;280;0
WireConnection;286;0;283;0
WireConnection;287;0;284;0
WireConnection;287;1;285;0
WireConnection;294;0;286;0
WireConnection;294;1;291;0
WireConnection;295;0;290;0
WireConnection;295;1;288;0
WireConnection;292;0;287;0
WireConnection;292;1;289;0
WireConnection;297;1;259;3
WireConnection;297;2;296;0
WireConnection;298;0;295;0
WireConnection;298;1;293;0
WireConnection;299;0;292;0
WireConnection;299;1;294;0
WireConnection;305;2;299;0
WireConnection;301;0;298;0
WireConnection;302;0;297;0
WireConnection;300;2;299;0
WireConnection;307;0;300;0
WireConnection;307;1;305;0
WireConnection;307;2;303;0
WireConnection;308;0;301;0
WireConnection;308;2;304;0
WireConnection;309;0;307;0
WireConnection;309;1;308;0
WireConnection;309;2;306;0
WireConnection;310;0;309;0
WireConnection;313;1;311;0
WireConnection;313;2;314;0
WireConnection;178;0;177;0
WireConnection;100;0;97;0
WireConnection;116;0;98;0
WireConnection;79;0;76;0
WireConnection;182;0;183;0
WireConnection;179;1;178;0
WireConnection;179;2;171;0
WireConnection;78;0;79;0
WireConnection;78;1;80;0
WireConnection;78;2;112;0
WireConnection;183;0;186;0
WireConnection;109;0;118;0
WireConnection;109;1;117;0
WireConnection;136;0;122;0
WireConnection;181;0;179;0
WireConnection;115;0;113;0
WireConnection;112;0;110;0
WireConnection;124;2;125;0
WireConnection;135;0;83;0
WireConnection;130;0;106;0
WireConnection;75;0;105;0
WireConnection;108;0;119;0
WireConnection;105;0;104;0
WireConnection;105;1;81;0
WireConnection;105;2;103;0
WireConnection;98;0;102;0
WireConnection;98;1;132;0
WireConnection;118;0;115;0
WireConnection;118;1;116;0
WireConnection;118;2;114;0
WireConnection;113;0;116;0
WireConnection;101;0;74;0
WireConnection;101;2;100;0
WireConnection;184;0;186;0
WireConnection;184;1;182;0
WireConnection;184;2;185;0
WireConnection;103;0;86;0
WireConnection;123;0;137;0
WireConnection;119;0;120;0
WireConnection;119;1;109;0
WireConnection;83;0;126;0
WireConnection;83;1;86;0
WireConnection;110;0;184;0
WireConnection;110;1;181;0
WireConnection;110;2;106;4
WireConnection;177;0;176;0
WireConnection;177;1;180;0
WireConnection;122;0;125;0
WireConnection;122;1;107;0
WireConnection;102;0;99;0
WireConnection;102;1;101;0
WireConnection;107;0;124;0
WireConnection;107;2;123;0
WireConnection;76;0;104;0
WireConnection;76;1;75;0
WireConnection;76;2;82;0
WireConnection;104;0;136;0
WireConnection;104;1;108;0
WireConnection;104;2;130;0
WireConnection;176;0;174;0
WireConnection;176;1;175;0
WireConnection;74;2;99;0
WireConnection;316;0;312;0
WireConnection;316;1;313;0
WireConnection;316;2;315;0
WireConnection;128;2;129;0
WireConnection;86;0;85;0
WireConnection;86;1;140;0
WireConnection;85;0;126;0
WireConnection;85;1;87;0
WireConnection;126;0;134;0
WireConnection;126;1;127;0
WireConnection;133;0;131;0
WireConnection;134;0;129;0
WireConnection;134;1;138;0
WireConnection;138;0;128;0
WireConnection;138;2;133;0
WireConnection;186;0;135;0
WireConnection;186;1;73;0
WireConnection;162;2;78;0
WireConnection;162;5;316;0
ASEEND*/
//CHKSM=894EC1A5A51424C2AFDE192BF941A92F4899B8A4