// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "/_Kass_/SH_VFX_SimplePremult_VertexAnim"
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
		_CustomData1YAffectsSpeed("Custom Data 1 Y Affects Speed", Range( 0 , 1)) = 1
		[Header(UV Based Sin Wave Displacement)]_UVSinWaveStrength("UV Sin Wave Strength", Vector) = (0,0,0,0)
		_UVSinWaveFrequency("UV Sin Wave Frequency", Float) = 1
		_UVSinWaveSpeed("UV Sin Wave Speed", Float) = 1
		[Header(Vertex Displacement Noise)]_VertexDisplacementNoise("Vertex Displacement Noise", 2D) = "white" {}
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
			float4 _VertexDisplacementNoise_ST;
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
			float _Src;
			float _UseVertexNormals;
			float _DepthFadeDivide;
			float _CustomData1XAffectsStrength;
			float _UVBasedDisplacementMaskDisplacement;
			float _UVBasedDisplacementMaskSoften;
			float _ClampDisplacement;
			float _UVSinWaveSpeed;
			float _UVSinWaveFrequency;
			float _VertexDisplacementNoiseStrength;
			float _CustomData1YAffectsSpeed;
			float _VertexDisplacementNoiseRotation;
			float _ZWrite;
			float _Dst;
			float _ZTest;
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
			sampler2D _VertexDisplacementNoise;
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

				float2 uv_VertexDisplacementNoise = v.ase_texcoord * _VertexDisplacementNoise_ST.xy + _VertexDisplacementNoise_ST.zw;
				float cos204 = cos( radians( _VertexDisplacementNoiseRotation ) );
				float sin204 = sin( radians( _VertexDisplacementNoiseRotation ) );
				float2 rotator204 = mul( uv_VertexDisplacementNoise - float2( 0.5,0.5 ) , float2x2( cos204 , -sin204 , sin204 , cos204 )) + float2( 0.5,0.5 );
				float4 texCoord188 = v.ase_texcoord;
				texCoord188.xy = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float lerpResult189 = lerp( 1.0 , texCoord188.w , _CustomData1YAffectsSpeed);
				float SpeedVariant192 = lerpResult189;
				float4 uvs4_VertexDisplacementNoise = v.ase_texcoord;
				uvs4_VertexDisplacementNoise.xy = v.ase_texcoord.xy * _VertexDisplacementNoise_ST.xy + _VertexDisplacementNoise_ST.zw;
				float4 uv2s4_VertexDisplacementNoise = v.ase_texcoord1;
				uv2s4_VertexDisplacementNoise.xy = v.ase_texcoord1.xy * _VertexDisplacementNoise_ST.xy + _VertexDisplacementNoise_ST.zw;
				float4 appendResult209 = (float4(uv2s4_VertexDisplacementNoise.z , uv2s4_VertexDisplacementNoise.w , 0.0 , 0.0));
				float dotResult223 = dot( tex2Dlod( _VertexDisplacementNoise, float4( ( float4( rotator204, 0.0 , 0.0 ) + float4( ( _TimeParameters.x * _VertexDisplacementNoisePanSpeed * SpeedVariant192 ), 0.0 , 0.0 ) + uvs4_VertexDisplacementNoise.w + appendResult209 ).xy, 0, 0.0) ) , _VertexDisplacementNoiseChannel );
				float2 texCoord205 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float mulTime213 = _TimeParameters.x * ( _UVSinWaveSpeed * SpeedVariant192 );
				float dotResult228 = dot( sin( ( ( texCoord205 * _UVSinWaveFrequency ) + mulTime213 ) ) , _UVSinWaveStrength );
				float temp_output_234_0 = ( ( dotResult223 * _VertexDisplacementNoiseStrength ) + dotResult228 );
				float lerpResult243 = lerp( -1.0 , 1.0 , temp_output_234_0);
				float lerpResult261 = lerp( 0.0 , 1.0 , temp_output_234_0);
				float lerpResult262 = lerp( lerpResult243 , lerpResult261 , _ClampDisplacement);
				float2 texCoord214 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float dotResult217 = dot( float4( texCoord214, 0.0 , 0.0 ) , _UVBasedDisplacementMaskChannel );
				float smoothstepResult233 = smoothstep( 0.0 , _UVBasedDisplacementMaskSoften , saturate( ( dotResult217 + _UVBasedDisplacementMaskDisplacement ) ));
				float lerpResult211 = lerp( 1.0 , texCoord188.z , _CustomData1XAffectsStrength);
				float StrengthVariant216 = lerpResult211;
				float VertexDisplacement236 = ( lerpResult262 * smoothstepResult233 * StrengthVariant216 );
				float3 lerpResult267 = lerp( float3( 1,1,1 ) , v.ase_normal , _UseVertexNormals);
				
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
				float3 vertexValue = ( VertexDisplacement236 * lerpResult267 * _VertexDisplacementAmount );
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
				
				float temp_output_135_0 = saturate( ( dotResult126 + temp_output_86_0 ) );
				float lerpResult184 = lerp( temp_output_135_0 , saturate( round( ( temp_output_135_0 * _AlphaBoldness ) ) ) , _FlatAlpha);
				float4 screenPos = IN.ase_texcoord4;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float eyeDepth174 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float eyeDepth = IN.ase_texcoord5.x;
				float cameraDepthFade175 = (( eyeDepth -_ProjectionParams.y - 0.0 ) / 1.0);
				float lerpResult179 = lerp( 1.0 , saturate( ( ( eyeDepth174 - cameraDepthFade175 ) / _DepthFadeDivide ) ) , _UseDepthFade);
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( saturate( lerpResult76 ) * _Brightness ).rgb;
				float Alpha = saturate( ( lerpResult184 * saturate( lerpResult179 ) * IN.ase_color.a ) );
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

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION


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
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _GradientShape_ST;
			float4 _GradientShapeChannel;
			float4 _VertexDisplacementNoise_ST;
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
			float _Src;
			float _UseVertexNormals;
			float _DepthFadeDivide;
			float _CustomData1XAffectsStrength;
			float _UVBasedDisplacementMaskDisplacement;
			float _UVBasedDisplacementMaskSoften;
			float _ClampDisplacement;
			float _UVSinWaveSpeed;
			float _UVSinWaveFrequency;
			float _VertexDisplacementNoiseStrength;
			float _CustomData1YAffectsSpeed;
			float _VertexDisplacementNoiseRotation;
			float _ZWrite;
			float _Dst;
			float _ZTest;
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
			sampler2D _VertexDisplacementNoise;
			sampler2D _Texture;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 uv_VertexDisplacementNoise = v.ase_texcoord * _VertexDisplacementNoise_ST.xy + _VertexDisplacementNoise_ST.zw;
				float cos204 = cos( radians( _VertexDisplacementNoiseRotation ) );
				float sin204 = sin( radians( _VertexDisplacementNoiseRotation ) );
				float2 rotator204 = mul( uv_VertexDisplacementNoise - float2( 0.5,0.5 ) , float2x2( cos204 , -sin204 , sin204 , cos204 )) + float2( 0.5,0.5 );
				float4 texCoord188 = v.ase_texcoord;
				texCoord188.xy = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float lerpResult189 = lerp( 1.0 , texCoord188.w , _CustomData1YAffectsSpeed);
				float SpeedVariant192 = lerpResult189;
				float4 uvs4_VertexDisplacementNoise = v.ase_texcoord;
				uvs4_VertexDisplacementNoise.xy = v.ase_texcoord.xy * _VertexDisplacementNoise_ST.xy + _VertexDisplacementNoise_ST.zw;
				float4 uv2s4_VertexDisplacementNoise = v.ase_texcoord1;
				uv2s4_VertexDisplacementNoise.xy = v.ase_texcoord1.xy * _VertexDisplacementNoise_ST.xy + _VertexDisplacementNoise_ST.zw;
				float4 appendResult209 = (float4(uv2s4_VertexDisplacementNoise.z , uv2s4_VertexDisplacementNoise.w , 0.0 , 0.0));
				float dotResult223 = dot( tex2Dlod( _VertexDisplacementNoise, float4( ( float4( rotator204, 0.0 , 0.0 ) + float4( ( _TimeParameters.x * _VertexDisplacementNoisePanSpeed * SpeedVariant192 ), 0.0 , 0.0 ) + uvs4_VertexDisplacementNoise.w + appendResult209 ).xy, 0, 0.0) ) , _VertexDisplacementNoiseChannel );
				float2 texCoord205 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float mulTime213 = _TimeParameters.x * ( _UVSinWaveSpeed * SpeedVariant192 );
				float dotResult228 = dot( sin( ( ( texCoord205 * _UVSinWaveFrequency ) + mulTime213 ) ) , _UVSinWaveStrength );
				float temp_output_234_0 = ( ( dotResult223 * _VertexDisplacementNoiseStrength ) + dotResult228 );
				float lerpResult243 = lerp( -1.0 , 1.0 , temp_output_234_0);
				float lerpResult261 = lerp( 0.0 , 1.0 , temp_output_234_0);
				float lerpResult262 = lerp( lerpResult243 , lerpResult261 , _ClampDisplacement);
				float2 texCoord214 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float dotResult217 = dot( float4( texCoord214, 0.0 , 0.0 ) , _UVBasedDisplacementMaskChannel );
				float smoothstepResult233 = smoothstep( 0.0 , _UVBasedDisplacementMaskSoften , saturate( ( dotResult217 + _UVBasedDisplacementMaskDisplacement ) ));
				float lerpResult211 = lerp( 1.0 , texCoord188.z , _CustomData1XAffectsStrength);
				float StrengthVariant216 = lerpResult211;
				float VertexDisplacement236 = ( lerpResult262 * smoothstepResult233 * StrengthVariant216 );
				float3 lerpResult267 = lerp( float3( 1,1,1 ) , v.ase_normal , _UseVertexNormals);
				
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord3 = screenPos;
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(v.vertex.xyz));
				float eyeDepth = -objectToViewPos.z;
				o.ase_texcoord4.x = eyeDepth;
				
				o.ase_texcoord2 = v.ase_texcoord;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord4.yzw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( VertexDisplacement236 * lerpResult267 * _VertexDisplacementAmount );
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
				float cos138 = cos( radians( _TextureRotation ) );
				float sin138 = sin( radians( _TextureRotation ) );
				float2 rotator138 = mul( uv_Texture - float2( 0.5,0.5 ) , float2x2( cos138 , -sin138 , sin138 , cos138 )) + float2( 0.5,0.5 );
				float dotResult126 = dot( tex2D( _Texture, rotator138 ) , _TextureChannel );
				float temp_output_86_0 = ( pow( dotResult126 , _CorePower ) * _CoreIntensity );
				float temp_output_135_0 = saturate( ( dotResult126 + temp_output_86_0 ) );
				float lerpResult184 = lerp( temp_output_135_0 , saturate( round( ( temp_output_135_0 * _AlphaBoldness ) ) ) , _FlatAlpha);
				float4 screenPos = IN.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float eyeDepth174 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float eyeDepth = IN.ase_texcoord4.x;
				float cameraDepthFade175 = (( eyeDepth -_ProjectionParams.y - 0.0 ) / 1.0);
				float lerpResult179 = lerp( 1.0 , saturate( ( ( eyeDepth174 - cameraDepthFade175 ) / _DepthFadeDivide ) ) , _UseDepthFade);
				
				float Alpha = saturate( ( lerpResult184 * saturate( lerpResult179 ) * IN.ase_color.a ) );
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
-1531;116;1334;838;-192.3606;-626.3593;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;187;-3850.873,2377.648;Inherit;False;Property;_CustomData1YAffectsSpeed;Custom Data 1 Y Affects Speed;24;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;188;-3954.056,2499.001;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;129;-2868.836,20.70718;Inherit;True;Property;_Texture;Texture;0;1;[Header];Create;True;1;Main Alpha;0;0;False;0;False;None;7579b8992ccf5124491219f77ea690a6;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;131;-2718.298,308.4864;Inherit;False;Property;_TextureRotation;Texture Rotation;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;189;-3467.014,2338.019;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;190;-2924.894,1469.528;Inherit;False;3676.525;1880.749;Vertex Animation Noise;45;236;235;234;233;231;230;229;228;227;226;225;224;223;222;221;220;219;218;217;215;214;213;212;210;209;207;206;205;204;203;202;201;200;199;198;197;196;195;194;193;191;243;260;261;262;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;128;-2572.296,152.4865;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RadiansOpNode;133;-2474.296,314.4864;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;193;-2905.891,1511.318;Inherit;True;Property;_VertexDisplacementNoise;Vertex Displacement Noise;28;1;[Header];Create;True;1;Vertex Displacement Noise;0;0;False;0;False;None;359096eeeced08a4fb16ffd42838845f;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;191;-2849.126,1898.568;Inherit;False;Property;_VertexDisplacementNoiseRotation;Vertex Displacement Noise Rotation;32;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;192;-3263.014,2329.019;Inherit;False;SpeedVariant;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;199;-2272.072,2414.983;Inherit;False;192;SpeedVariant;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;195;-2302.556,1519.528;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;194;-2587.721,1751.229;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;201;-2353.716,2111.429;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;196;-2489.722,1913.229;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;200;-1894.66,2852.677;Inherit;False;192;SpeedVariant;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;197;-2355.959,2205.188;Inherit;False;Property;_VertexDisplacementNoisePanSpeed;Vertex Displacement Noise  Pan Speed;30;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;198;-1945.03,2715.673;Inherit;False;Property;_UVSinWaveSpeed;UV Sin Wave Speed;27;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;138;-2292.296,178.4865;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;202;-2342.282,1887.337;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;205;-1833.031,2393.073;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;127;-2071.739,360.816;Inherit;False;Property;_TextureChannel;Texture Channel;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;204;-2307.723,1777.229;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;134;-2052.625,139.7253;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;207;-1694.461,2748.677;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;209;-1998.897,1527.557;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;206;-1913.031,2583.073;Inherit;False;Property;_UVSinWaveFrequency;UV Sin Wave Frequency;26;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;203;-1943.718,2039.429;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-1625.092,443.9026;Inherit;False;Property;_CorePower;Core Power;11;1;[Header];Create;False;1;Different Center Color;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;126;-1638.4,142.6949;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;213;-1593.032,2585.073;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;210;-1777.42,1890.689;Inherit;False;4;4;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;215;-1529.032,2409.073;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;10;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;140;-1651.129,603.5795;Inherit;False;Property;_CoreIntensity;Core Intensity;12;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;85;-1325.956,371.7473;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;218;-1369.032,2409.073;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;219;-1394.624,1864.858;Inherit;True;Property;_TextureSample10;Texture Sample 10;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;221;-1330.158,2104.75;Inherit;False;Property;_VertexDisplacementNoiseChannel;Vertex Displacement Noise Channel;29;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;222;-1232.587,2409.196;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;-1152.641,435.9595;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;223;-999.1991,1877.321;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;212;-1103.545,2969.234;Inherit;False;Property;_UVBasedDisplacementMaskChannel;UV Based Displacement Mask Channel;33;1;[Header];Create;True;1;UV Based Displacement Mask;0;0;False;0;False;0,0,0,0;0,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;226;-1209.931,2287.917;Inherit;False;Property;_VertexDisplacementNoiseStrength;Vertex Displacement Noise Strength;31;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;214;-1091.094,2778.021;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;227;-1486.837,2748.605;Inherit;False;Property;_UVSinWaveStrength;UV Sin Wave Strength;25;1;[Header];Create;True;1;UV Based Sin Wave Displacement;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;230;-753.931,1925.917;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;220;-1017.476,3173.58;Inherit;False;Property;_UVBasedDisplacementMaskDisplacement;UV Based Displacement Mask Displacement;34;0;Create;True;0;0;0;False;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenDepthNode;174;-234.0835,823.6605;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;228;-898.328,2507.051;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;1,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;217;-759.0612,2827.593;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;208;-3817.549,2744.891;Inherit;False;Property;_CustomData1XAffectsStrength;Custom Data 1 X Affects Strength;23;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;175;-386.6537,1001.82;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;83;-1009.835,265.0002;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;135;-843.8864,368.1976;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;211;-3461.014,2704.019;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-1160.116,640.0095;Inherit;False;Property;_AlphaBoldness;Alpha Boldness;16;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;180;-142.3401,1241.271;Inherit;False;Property;_DepthFadeDivide;Depth Fade Divide;19;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;176;-1.912445,848.1799;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;224;-608.9033,3153.058;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;234;-563.5752,2176.828;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;243;-351.2736,1993.285;Inherit;False;3;0;FLOAT;-1;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;177;141.0238,896.6915;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;745.75;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;229;-466.4311,3027.579;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;216;-3263.014,2707.019;Inherit;False;StrengthVariant;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;260;-410.7454,1757.837;Inherit;False;Property;_ClampDisplacement;Clamp Displacement;22;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;231;-532.431,3249.579;Inherit;False;Property;_UVBasedDisplacementMaskSoften;UV Based Displacement Mask Soften;35;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;261;-330.7454,1855.837;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-701.5504,524.4108;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;171;150.51,1037.263;Inherit;False;Property;_UseDepthFade;Use Depth Fade;18;1;[Header];Create;True;1;Depth Fade;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;178;298.9942,844.6931;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;183;-567.5531,548.8067;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;225;-426.0021,2309.871;Inherit;False;216;StrengthVariant;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;262;-120.7454,1863.837;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;233;-256.4311,2909.578;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;182;-399.721,477.3039;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;235;-80.00522,2108.895;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;179;450.4449,759.957;Inherit;True;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;185;-429.4622,604.2207;Inherit;False;Property;_FlatAlpha;Flat Alpha;17;0;Create;True;0;0;0;False;0;False;0;0.144;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;268;596.0069,1087.14;Inherit;False;Property;_UseVertexNormals;Use Vertex Normals;21;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;236;92.34973,1866.991;Inherit;False;VertexDisplacement;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;181;734.7398,756.7349;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;106;62.4616,-428.677;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;184;-120.7252,394.7925;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;258;674.0466,1228.15;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;237;778.3668,842.0676;Inherit;False;236;VertexDisplacement;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;267;992.576,1005.707;Inherit;False;3;0;FLOAT3;1,1,1;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;634.7449,390.9565;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;239;979.2106,1433.628;Inherit;False;Property;_VertexDisplacementAmount;Vertex Displacement Amount;20;1;[Header];Create;True;1;Vertex Displacement;0;0;False;0;False;0,0,0;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureCoordinatesNode;74;-2367.268,-617.0903;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;1238.077,-63.43907;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;75;901.9648,-199.1521;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;81;445.8063,-151.9188;Inherit;False;Property;_CoreColor;Core Color;14;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;107;-312.6609,-1030.307;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;168;1323.081,579.4114;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;102;-1832.032,-705.4958;Inherit;True;Property;_TextureSample2;Texture Sample 2;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;79;1188.4,-162.607;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;167;997.7284,699.9562;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;80;993.318,-32.62639;Inherit;False;Property;_Brightness;Brightness;15;1;[Header];Create;True;1;Brightness and Opacity;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;116;-1130.521,-374.8206;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;98;-1381.646,-479.0361;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;166;991.7284,471.9562;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;169;1363.882,422.1359;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;120;-291.1222,-781.3338;Inherit;True;Property;_GradientMap;Gradient Map;8;1;[Header];Create;True;1;Gradient Map;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;1611.882,378.1359;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;114;-897.9782,-250.6806;Inherit;False;Property;_InvertGradient;Invert Gradient;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;105;760.417,-78.32274;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;118;-649.9781,-482.6806;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;113;-999.9781,-408.6806;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;125;-889.1999,-1184.606;Inherit;True;Property;_ColorTexture;Color Texture;3;1;[Header];Create;True;1;Overlay Color;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ComponentMaskNode;130;384.7512,-285.808;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RadiansOpNode;100;-2269.268,-455.09;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;121;771.9019,-749.282;Inherit;False;Property;_Cull;Cull;36;1;[Header];Create;True;1;Rendering;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;117;-583.9781,-218.6806;Inherit;False;Property;_GradientMapDisplacement;Gradient Map Displacement;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-2513.271,-461.0901;Inherit;False;Property;_GradientShapeRotation;Gradient Shape Rotation;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;160;1064.511,-715.4269;Inherit;False;Property;_Src;Src;39;0;Create;True;0;0;0;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;124;-592.6614,-1056.307;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;115;-837.9782,-438.6806;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;158;1322.511,-717.4269;Inherit;False;Property;_Dst;Dst;40;0;Create;True;0;0;0;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;112;1008.103,344.5316;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;99;-2673.34,-825.0892;Inherit;True;Property;_GradientShape;Gradient Shape;5;1;[Header];Create;True;1;Gradient Shape;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ComponentMaskNode;136;354.8574,-852.2254;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RotatorNode;101;-2087.267,-591.0903;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;157;1576.511,-715.4269;Inherit;False;Property;_ZWrite;ZWrite;37;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;122;-72.98848,-1069.069;Inherit;True;Property;_TextureSample3;Texture Sample 3;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;103;500.2039,66.3109;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;159;1834.511,-717.4269;Inherit;False;Property;_ZTest;ZTest;38;0;Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;76;1094.851,-305.3001;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;108;386.0715,-376.6457;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector4Node;132;-1772.814,-465.6055;Inherit;False;Property;_GradientShapeChannel;Gradient Shape Channel;6;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;240;1370.643,950.847;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RadiansOpNode;123;-494.6613,-894.308;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;655.0266,-310.9217;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;82;943.4619,-129.8322;Inherit;False;Property;_DifferentCoreColor;Different Core Color;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;119;14.34905,-674.3876;Inherit;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;109;-270.9781,-360.6806;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-738.6611,-900.308;Inherit;False;Property;_ColorRotation;Color Rotation;4;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;164;1799.307,34.03413;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;162;1799.307,34.03413;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;/_Kass_/SH_VFX_SimplePremult_VertexAnim;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;True;121;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;0;False;True;3;1;True;160;10;True;158;3;1;True;160;10;True;158;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;True;157;True;3;True;159;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;0;Hidden/InternalErrorShader;0;0;Standard;22;Surface;1;  Blend;1;Two Sided;1;Cast Shadows;0;  Use Shadow Threshold;0;Receive Shadows;0;GPU Instancing;1;LOD CrossFade;0;Built-in Fog;0;DOTS Instancing;0;Meta Pass;0;Extra Pre Pass;0;Tessellation;0;  Phong;0;  Strength;0.5,False,-1;  Type;0;  Tess;16,False,-1;  Min;10,False,-1;  Max;25,False,-1;  Edge Length;16,False,-1;  Max Displacement;25,False,-1;Vertex Position,InvertActionOnDeselection;1;0;5;False;True;False;True;False;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;163;1799.307,34.03413;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;161;1799.307,34.03413;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;165;1799.307,34.03413;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;189;1;188;4
WireConnection;189;2;187;0
WireConnection;128;2;129;0
WireConnection;133;0;131;0
WireConnection;192;0;189;0
WireConnection;195;2;193;0
WireConnection;194;2;193;0
WireConnection;196;0;191;0
WireConnection;138;0;128;0
WireConnection;138;2;133;0
WireConnection;202;2;193;0
WireConnection;204;0;194;0
WireConnection;204;2;196;0
WireConnection;134;0;129;0
WireConnection;134;1;138;0
WireConnection;207;0;198;0
WireConnection;207;1;200;0
WireConnection;209;0;195;3
WireConnection;209;1;195;4
WireConnection;203;0;201;0
WireConnection;203;1;197;0
WireConnection;203;2;199;0
WireConnection;126;0;134;0
WireConnection;126;1;127;0
WireConnection;213;0;207;0
WireConnection;210;0;204;0
WireConnection;210;1;203;0
WireConnection;210;2;202;4
WireConnection;210;3;209;0
WireConnection;215;0;205;0
WireConnection;215;1;206;0
WireConnection;85;0;126;0
WireConnection;85;1;87;0
WireConnection;218;0;215;0
WireConnection;218;1;213;0
WireConnection;219;0;193;0
WireConnection;219;1;210;0
WireConnection;222;0;218;0
WireConnection;86;0;85;0
WireConnection;86;1;140;0
WireConnection;223;0;219;0
WireConnection;223;1;221;0
WireConnection;230;0;223;0
WireConnection;230;1;226;0
WireConnection;228;0;222;0
WireConnection;228;1;227;0
WireConnection;217;0;214;0
WireConnection;217;1;212;0
WireConnection;83;0;126;0
WireConnection;83;1;86;0
WireConnection;135;0;83;0
WireConnection;211;1;188;3
WireConnection;211;2;208;0
WireConnection;176;0;174;0
WireConnection;176;1;175;0
WireConnection;224;0;217;0
WireConnection;224;1;220;0
WireConnection;234;0;230;0
WireConnection;234;1;228;0
WireConnection;243;2;234;0
WireConnection;177;0;176;0
WireConnection;177;1;180;0
WireConnection;229;0;224;0
WireConnection;216;0;211;0
WireConnection;261;2;234;0
WireConnection;186;0;135;0
WireConnection;186;1;73;0
WireConnection;178;0;177;0
WireConnection;183;0;186;0
WireConnection;262;0;243;0
WireConnection;262;1;261;0
WireConnection;262;2;260;0
WireConnection;233;0;229;0
WireConnection;233;2;231;0
WireConnection;182;0;183;0
WireConnection;235;0;262;0
WireConnection;235;1;233;0
WireConnection;235;2;225;0
WireConnection;179;1;178;0
WireConnection;179;2;171;0
WireConnection;236;0;235;0
WireConnection;181;0;179;0
WireConnection;184;0;135;0
WireConnection;184;1;182;0
WireConnection;184;2;185;0
WireConnection;267;1;258;0
WireConnection;267;2;268;0
WireConnection;110;0;184;0
WireConnection;110;1;181;0
WireConnection;110;2;106;4
WireConnection;74;2;99;0
WireConnection;78;0;79;0
WireConnection;78;1;80;0
WireConnection;75;0;105;0
WireConnection;107;0;124;0
WireConnection;107;2;123;0
WireConnection;102;0;99;0
WireConnection;102;1;101;0
WireConnection;79;0;76;0
WireConnection;116;0;98;0
WireConnection;98;0;102;0
WireConnection;98;1;132;0
WireConnection;169;0;166;0
WireConnection;169;1;167;0
WireConnection;170;0;169;0
WireConnection;170;1;168;3
WireConnection;105;0;104;0
WireConnection;105;1;81;0
WireConnection;105;2;103;0
WireConnection;118;0;115;0
WireConnection;118;1;116;0
WireConnection;118;2;114;0
WireConnection;113;0;116;0
WireConnection;130;0;106;0
WireConnection;100;0;97;0
WireConnection;124;2;125;0
WireConnection;115;0;113;0
WireConnection;112;0;110;0
WireConnection;136;0;122;0
WireConnection;101;0;74;0
WireConnection;101;2;100;0
WireConnection;122;0;125;0
WireConnection;122;1;107;0
WireConnection;103;0;86;0
WireConnection;76;0;104;0
WireConnection;76;1;75;0
WireConnection;76;2;82;0
WireConnection;108;0;119;0
WireConnection;240;0;237;0
WireConnection;240;1;267;0
WireConnection;240;2;239;0
WireConnection;123;0;137;0
WireConnection;104;0;136;0
WireConnection;104;1;108;0
WireConnection;104;2;130;0
WireConnection;119;0;120;0
WireConnection;119;1;109;0
WireConnection;109;0;118;0
WireConnection;109;1;117;0
WireConnection;162;2;78;0
WireConnection;162;3;112;0
WireConnection;162;5;240;0
ASEEND*/
//CHKSM=F15A8200301E6A4DA0AC2CEA79C4B7EEA12DC771