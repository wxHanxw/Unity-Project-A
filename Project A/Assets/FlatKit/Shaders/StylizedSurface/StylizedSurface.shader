﻿Shader "FlatKit/Stylized Surface"
{
    Properties
    {
        [MainColor] _BaseColor ("Color", Color) = (1,1,1,1)

        [KeywordEnum(None, Single, Steps, Curve)]_CelPrimaryMode("Cel Shading Mode", Float) = 1
        _ColorDim ("[_CELPRIMARYMODE_SINGLE]Color Shaded", Color) = (0.85023, 0.85034, 0.85045, 0.85056)
        _ColorDimSteps ("[_CELPRIMARYMODE_STEPS]Color Shaded", Color) = (0.85023, 0.85034, 0.85045, 0.85056)
        _ColorDimCurve ("[_CELPRIMARYMODE_CURVE]Color Shaded", Color) = (0.85023, 0.85034, 0.85045, 0.85056)
        _SelfShadingSize ("[_CELPRIMARYMODE_SINGLE]Self Shading Size", Range(0, 1)) = 0.5
        _ShadowEdgeSize ("[_CELPRIMARYMODE_SINGLE]Edge Size", Range(0, 0.5)) = 0.05
        _Flatness ("[_CELPRIMARYMODE_SINGLE]Localized Shading", Range(0, 1)) = 1.0

        [IntRange]_CelNumSteps ("[_CELPRIMARYMODE_STEPS]Number Of Steps", Range(1, 10)) = 3.0
        _CelStepTexture ("[_CELPRIMARYMODE_STEPS][LAST_PROP_STEPS]Cel steps", 2D) = "black" {}
        _CelCurveTexture ("[_CELPRIMARYMODE_CURVE][LAST_PROP_CURVE]Ramp", 2D) = "black" {}

        [Space(10)]
        [Toggle(DR_CEL_EXTRA_ON)] _CelExtraEnabled("Enable Extra Cel Layer", Int) = 0
        _ColorDimExtra ("[DR_CEL_EXTRA_ON]Color Shaded", Color) = (0.85023, 0.85034, 0.85045, 0.85056)
        _SelfShadingSizeExtra ("[DR_CEL_EXTRA_ON]Self Shading Size", Range(0, 1)) = 0.6
        _ShadowEdgeSizeExtra ("[DR_CEL_EXTRA_ON]Edge Size", Range(0, 0.5)) = 0.05
        _FlatnessExtra ("[DR_CEL_EXTRA_ON]Localized Shading", Range(0, 1)) = 1.0

        [Space(10)]
        [Toggle(DR_SPECULAR_ON)] _SpecularEnabled("Enable Specular", Int) = 0
        [HDR] _FlatSpecularColor("[DR_SPECULAR_ON]Specular Color", Color) = (0.85023, 0.85034, 0.85045, 0.85056)
        _FlatSpecularSize("[DR_SPECULAR_ON]Specular Size", Range(0.0, 1.0)) = 0.1
        _FlatSpecularEdgeSmoothness("[DR_SPECULAR_ON]Specular Edge Smoothness", Range(0.0, 1.0)) = 0

        [Space(10)]
        [Toggle(DR_RIM_ON)] _RimEnabled("Enable Rim", Int) = 0
        [HDR] _FlatRimColor("[DR_RIM_ON]Rim Color", Color) = (0.85023, 0.85034, 0.85045, 0.85056)
        _FlatRimLightAlign("[DR_RIM_ON]Light Align", Range(0.0, 1.0)) = 0
        _FlatRimSize("[DR_RIM_ON]Rim Size", Range(0, 1)) = 0.5
        _FlatRimEdgeSmoothness("[DR_RIM_ON]Rim Edge Smoothness", Range(0, 1)) = 0.5

        [Space(10)]
        [Toggle(DR_GRADIENT_ON)] _GradientEnabled("Enable Height Gradient", Int) = 0
        [HDR] _ColorGradient("[DR_GRADIENT_ON]Gradient Color", Color) = (0.85023, 0.85034, 0.85045, 0.85056)
        [KeywordEnum(World, Local)]_GradientSpace("[DR_GRADIENT_ON]Space", Float) = 0
        _GradientCenterX("[DR_GRADIENT_ON]Center X", Float) = 0
        _GradientCenterY("[DR_GRADIENT_ON]Center Y", Float) = 0
        _GradientSize("[DR_GRADIENT_ON]Size", Float) = 10.0
        _GradientAngle("[DR_GRADIENT_ON]Gradient Angle", Range(0, 360)) = 0

        [Space(10)]
        [Toggle(DR_OUTLINE_ON)] _OutlineEnabled("Enable Outline", Int) = 0
        _OutlineWidth("[DR_OUTLINE_ON]Width", Float) = 1.0
        _OutlineColor("[DR_OUTLINE_ON]Color", Color) = (1, 1, 1, 1)
    	_OutlineScale("[DR_OUTLINE_ON]Scale", Float) = 1.0
        [Toggle(DR_OUTLINE_SMOOTH_NORMALS)] _VertexExtrusionSmoothNormals("[DR_OUTLINE_ON]Smooth Normals", Float) = 0.0
        _OutlineDepthOffset("[DR_OUTLINE_ON]Depth Offset", Range(0, 1)) = 0.0
        [KeywordEnum(Screen, Object)] _OutlineSpace("[DR_OUTLINE_ON]Space", Float) = 0.0
        _CameraDistanceImpact("[DR_OUTLINE_ON][_OUTLINESPACE_SCREEN]Camera Distance Impact", Range(0, 1)) = 0.0

        [Space(10)]
        [Toggle(DR_VERTEX_COLORS_ON)] _VertexColorsEnabled("Enable Vertex Colors", Int) = 0

        _LightContribution("[FOLDOUT(Advanced Lighting){6}]Light Color Contribution", Range(0, 1)) = 0
        _LightFalloffSize("Point / Spot Light Edge", Range(0, 1)) = 0

        // Used to provide light direction to cel shading if all light in the scene is baked.
        [Toggle(DR_ENABLE_LIGHTMAP_DIR)]_OverrideLightmapDir("Override Light Direction", Int) = 0
        _LightmapDirectionPitch("[DR_ENABLE_LIGHTMAP_DIR]Pitch", Range(0, 360)) = 0
        _LightmapDirectionYaw("[DR_ENABLE_LIGHTMAP_DIR]Yaw", Range(0, 360)) = 0
        [HideInInspector] _LightmapDirection("Direction", Vector) = (0, 1, 0, 0)

        [KeywordEnum(None, Multiply, Color)] _UnityShadowMode ("[FOLDOUT(Unity Built-in Shadows){5}]Mode", Float) = 0
        _UnityShadowPower("[_UNITYSHADOWMODE_MULTIPLY]Power", Range(0, 1)) = 0.2
        _UnityShadowColor("[_UNITYSHADOWMODE_COLOR]Color", Color) = (0.85023, 0.85034, 0.85045, 0.85056)
        _UnityShadowSharpness("Sharpness", Range(1, 10)) = 1.0
        [Toggle(_UNITYSHADOW_OCCLUSION)]_UnityShadowOcclusion("Shadow Occlusion", Int) = 0

        [MainTexture] _BaseMap("[FOLDOUT(Texture Maps){11}]Albedo", 2D) = "white" {}
    	[Toggle(_BASEMAP_PREMULTIPLY)]_BaseMapPremultiply("[_]Mix Into Shading", Int) = 0
        [KeywordEnum(Multiply, Add)]_TextureBlendingMode("[_]Blending Mode", Float) = 0
        _TextureImpact("[_]Texture Impact", Range(0, 1)) = 1.0

        _DetailMap("Detail Map", 2D) = "white" {}
        _DetailMapColor("[]Detail Color", Color) = (1,1,1,1)
        [KeywordEnum(Multiply, Add, Interpolate)]_DetailMapBlendingMode("[]Blending Mode", Float) = 0
    	_DetailMapImpact("[]Detail Impact", Range(0, 1)) = 0.0

        _BumpMap ("Normal Map", 2D) = "bump" {}
    	
        _EmissionMap ("Emission Map", 2D) = "white" {}
        [HDR]_EmissionColor("Emission Color", Color) = (1, 1, 1, 1)

        [HideInInspector] _Cutoff ("Base Alpha Cutoff", Range (0, 1)) = .5

        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0

        // Editmode props
        [HideInInspector] _QueueOffset("Queue Offset", Float) = 0.0

		/* start CurvedWorld */
		//[CurvedWorldBendSettings] _CurvedWorldBendSettings("0|1|1", Vector) = (0, 0, 0, 0)
		/* end CurvedWorld */
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
        LOD 300

    	HLSLINCLUDE
    	// #define FLAT_KIT_DOTS_INSTANCING_ON // Uncomment to enable DOTS instancing
    	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Version.hlsl"
    	ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForwardOnly"}

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
            #pragma shader_feature_local_fragment __ _CELPRIMARYMODE_SINGLE _CELPRIMARYMODE_STEPS _CELPRIMARYMODE_CURVE
            #pragma shader_feature_local_fragment DR_CEL_EXTRA_ON
            #pragma shader_feature_local_fragment DR_GRADIENT_ON
            #pragma shader_feature_local_fragment __ _GRADIENTSPACE_WORLD _GRADIENTSPACE_LOCAL
            #pragma shader_feature_local_fragment DR_SPECULAR_ON
            #pragma shader_feature_local_fragment DR_RIM_ON
            #pragma shader_feature_local DR_VERTEX_COLORS_ON
            #pragma shader_feature_local_fragment DR_ENABLE_LIGHTMAP_DIR
            #pragma shader_feature_local_fragment __ _UNITYSHADOWMODE_MULTIPLY _UNITYSHADOWMODE_COLOR
            #pragma shader_feature_local_fragment _TEXTUREBLENDINGMODE_MULTIPLY _TEXTUREBLENDINGMODE_ADD
            #pragma shader_feature_local_fragment _UNITYSHADOW_OCCLUSION
            #pragma shader_feature_local_fragment _BASEMAP_PREMULTIPLY

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
            // #pragma shader_feature_local_fragment _ _SPECGLOSSMAP _SPECULAR_COLOR
            // #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            #if UNITY_VERSION >= 600000
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #endif

            // -------------------------------------
            // Universal Pipeline keywords
            #if VERSION_GREATER_EQUAL(11, 0)
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #else
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #endif
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #if VERSION_GREATER_EQUAL(12, 0)
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #endif
            #if UNITY_VERSION >= 202220 && UNITY_VERSION < 600000
            #pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS
            #endif
            #if UNITY_VERSION >= 600000
            #pragma multi_compile _ _FORWARD_PLUS
            #pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
            #define _ENVIRONMENTREFLECTIONS_OFF 1 // Fixes flickering when Probe Blending is enabled on Renderer.
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
            #else
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #endif
            #if UNITY_VERSION >= 60000012
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ProbeVolumeVariants.hlsl"
            #endif

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #if UNITY_VERSION >= 202220
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
            #endif

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #if defined(FLAT_KIT_DOTS_INSTANCING_ON)
            #pragma target 4.5
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #endif

            // Detail map.
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma shader_feature_local_fragment _DETAILMAPBLENDINGMODE_MULTIPLY _DETAILMAPBLENDINGMODE_ADD _DETAILMAPBLENDINGMODE_INTERPOLATE

            TEXTURE2D(_DetailMap);
            SAMPLER(sampler_DetailMap);

            #pragma vertex StylizedPassVertex
            #pragma fragment StylizedPassFragment
            #if UNITY_VERSION >= 202230
            #define BUMP_SCALE_NOT_SUPPORTED 1
            #endif

            // TODO: Toggle _NORMALMAP from the editor script.
            #define _NORMALMAP

            #include "LibraryUrp/StylizedInput.hlsl"
            #include "LibraryUrp/LitForwardPass_DR.hlsl"
            #include "LibraryUrp/Lighting_DR.hlsl"

			/* start CurvedWorld */
			//#define CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE
			//#define CURVEDWORLD_BEND_ID_1
			//#pragma shader_feature_local CURVEDWORLD_DISABLED_ON
			//#pragma shader_feature_local CURVEDWORLD_NORMAL_TRANSFORMATION_ON
			//#include "Assets/Amazing Assets/Curved World/Shaders/Core/CurvedWorldTransform.cginc"
			/* end CurvedWorld */

            ENDHLSL
        }

        Pass
        {
        	// Renderer Feature outline pass.
            Name "Outline"
            Tags{"LightMode" = "Outline"}

            Cull Front

            HLSLPROGRAM
            #include "LibraryUrp/StylizedInput.hlsl"

            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram

            #pragma multi_compile _ DR_OUTLINE_ON
            #pragma multi_compile _ DR_OUTLINE_SMOOTH_NORMALS
            #pragma multi_compile __ _OUTLINESPACE_SCREEN _OUTLINESPACE_OBJECT
            #pragma multi_compile_fog

			/* start CurvedWorld */
			//#define CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE
			//#define CURVEDWORLD_BEND_ID_1
			//#pragma shader_feature_local CURVEDWORLD_DISABLED_ON
			//#pragma shader_feature_local CURVEDWORLD_NORMAL_TRANSFORMATION_ON
			//#include "Assets/Amazing Assets/Curved World/Shaders/Core/CurvedWorldTransform.cginc"
			/* end CurvedWorld */

            struct VertexInput
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
            	#if defined(DR_OUTLINE_SMOOTH_NORMALS)
                float4 uv2 : TEXCOORD2;
                #endif
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                float fogCoord : TEXCOORD1;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 ObjectToClipPos(float4 pos)
            {
                return mul(UNITY_MATRIX_VP, mul(UNITY_MATRIX_M, float4(pos.xyz, 1)));
            }

            float4 ObjectToClipDir(float3 dir)
            {
                return mul(UNITY_MATRIX_VP, mul(UNITY_MATRIX_M, float4(dir.xyz, 0)));
            }

            VertexOutput VertexProgram(VertexInput v)
            {
                #if defined(CURVEDWORLD_IS_INSTALLED) && !defined(CURVEDWORLD_DISABLED_ON)
                    CURVEDWORLD_TRANSFORM_VERTEX(v.position)
                #endif

                UNITY_SETUP_INSTANCE_ID(v);

                VertexOutput o = (VertexOutput)0;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	            #if defined(DR_OUTLINE_ON)
            		#if defined(DR_OUTLINE_SMOOTH_NORMALS)
            			float3 objectScale = abs(UNITY_MATRIX_M[0].xyz) + abs(UNITY_MATRIX_M[1].xyz) + abs(UNITY_MATRIX_M[2].xyz);
            			v.normal = v.uv2.xyz / objectScale;
            		#endif
            	
            		#if defined(_OUTLINESPACE_OBJECT)
            			float3 offset = v.normal * _OutlineWidth * 0.01;
		                float4 clipPosition = ObjectToClipPos(v.position * _OutlineScale + float4(offset, 0)); 
					#else
		                float4 clipPosition = ObjectToClipPos(v.position * _OutlineScale);
		                const float3 clipNormal = ObjectToClipDir(v.normal).xyz;
						const float2 aspectRatio = float2(_ScreenParams.x / _ScreenParams.y, 1);
		                const half cameraDistanceImpact = lerp(clipPosition.w, 4.0, _CameraDistanceImpact);
		                const float2 offset = normalize(clipNormal.xy) / aspectRatio * _OutlineWidth * cameraDistanceImpact * 0.005;
		                clipPosition.xy += offset;
            		#endif

            		// Depth offset
		            {
			            const half outlineDepthOffset = _OutlineDepthOffset * .1;
                    	#if UNITY_REVERSED_Z
                    	clipPosition.z -= outlineDepthOffset;
                    	#else
                    	clipPosition.z += outlineDepthOffset * (1.0 - UNITY_NEAR_CLIP_VALUE);
                    	#endif
		            }

	                o.position = clipPosition;
	                o.fogCoord = ComputeFogFactor(o.position.z);
                #endif
            	
                return o;
            }

            half4 FragmentProgram(VertexOutput i) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                half4 color = _OutlineColor;
                color.rgb = MixFog(color.rgb, i.fogCoord);
                return color;
            }
            ENDHLSL
        }

        // All the following passes are from URP SimpleLit.shader.
        // UsePass "Universal Render Pipeline/Simple Lit/..." - not included in build and produces z-buffer glitches in
        // local and global outlines combination.

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #if defined(FLAT_KIT_DOTS_INSTANCING_ON)
            #pragma target 4.5
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #endif

            // -------------------------------------
            // Universal Pipeline keywords

            // -------------------------------------
            // Unity defined keywords
            #if UNITY_VERSION >= 202220
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
            #endif

            // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "LibraryUrp/StylizedInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"

			/* start CurvedWorld */
			//#define CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE
			//#define CURVEDWORLD_BEND_ID_1
			//#pragma shader_feature_local CURVEDWORLD_DISABLED_ON
			//#pragma shader_feature_local CURVEDWORLD_NORMAL_TRANSFORMATION_ON
			//#include "Assets/Amazing Assets/Curved World/Shaders/Core/CurvedWorldTransform.cginc"
			/* end CurvedWorld */

            ENDHLSL
        }

		Pass
        {
            Name "GBuffer"
            Tags{"LightMode" = "UniversalGBuffer"}

            ZWrite[_ZWrite]
            ZTest LEqual
            Cull[_Cull]

            HLSLPROGRAM
            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            // #pragma shader_feature _ALPHAPREMULTIPLY_ON
            // #pragma shader_feature_local_fragment _ _SPECGLOSSMAP _SPECULAR_COLOR
            // #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            #if UNITY_VERSION >= 600000
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #endif

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            //#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            //#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #if UNITY_VERSION >= 600000
            #define _ENVIRONMENTREFLECTIONS_OFF 1 // Fixes flickering when Probe Blending is enabled on Renderer.
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
            #else
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #endif
            #if UNITY_VERSION >= 60000012
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ProbeVolumeVariants.hlsl"
            #endif

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
            #if UNITY_VERSION >= 600000
            #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED
            #endif

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #if defined(FLAT_KIT_DOTS_INSTANCING_ON)
            #pragma target 4.5
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #endif

            #pragma vertex LitPassVertexSimple
            #pragma fragment LitPassFragmentSimple

            #define BUMP_SCALE_NOT_SUPPORTED 1

            #include "LibraryUrp/StylizedInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitGBufferPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

            // -------------------------------------
            // Unity defined keywords
            #if UNITY_VERSION >= 202220
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
            #endif

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #if defined(FLAT_KIT_DOTS_INSTANCING_ON)
            #pragma target 4.5
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #endif

            #include "LibraryUrp/StylizedInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"

			/* start CurvedWorld */
			//#define CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE
			//#define CURVEDWORLD_BEND_ID_1
			//#pragma shader_feature_local CURVEDWORLD_DISABLED_ON
			//#pragma shader_feature_local CURVEDWORLD_NORMAL_TRANSFORMATION_ON
			//#include "Assets/Amazing Assets/Curved World/Shaders/Core/CurvedWorldTransform.cginc"
			/* end CurvedWorld */

            ENDHLSL
        }

        // This pass is used when drawing to a _CameraNormalsTexture texture
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

            // -------------------------------------
            // Unity defined keywords
            #if UNITY_VERSION >= 202220
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
            // Universal Pipeline keywords
            #pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS
            #endif

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #if defined(FLAT_KIT_DOTS_INSTANCING_ON)
            #pragma target 4.5
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #endif

            #include "LibraryUrp/StylizedInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"

			/* start CurvedWorld */
			//#define CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE
			//#define CURVEDWORLD_BEND_ID_1
			//#pragma shader_feature_local CURVEDWORLD_DISABLED_ON
			//#pragma shader_feature_local CURVEDWORLD_NORMAL_TRANSFORMATION_ON
			//#include "Assets/Amazing Assets/Curved World/Shaders/Core/CurvedWorldTransform.cginc"
			/* end CurvedWorld */

            ENDHLSL
        }

        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags{ "LightMode" = "Meta" }

            Cull Off

            HLSLPROGRAM
            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMetaSimple
            #if UNITY_VERSION >= 202220
            #pragma shader_feature EDITOR_VISUALIZATION
            #endif

            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _SPECGLOSSMAP

            #include "LibraryUrp/StylizedInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitMetaPass.hlsl"

			/* start CurvedWorld */
			//#define CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE
			//#define CURVEDWORLD_BEND_ID_1
			//#pragma shader_feature_local CURVEDWORLD_DISABLED_ON
			//#pragma shader_feature_local CURVEDWORLD_NORMAL_TRANSFORMATION_ON
			//#include "Assets/Amazing Assets/Curved World/Shaders/Core/CurvedWorldTransform.cginc"
			/* end CurvedWorld */

            ENDHLSL
        }
        Pass
        {
            Name "Universal2D"
            Tags{ "LightMode" = "Universal2D" }
            Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON

            #include "LibraryUrp/StylizedInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Universal2D.hlsl"

			/* start CurvedWorld */
			//#define CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE
			//#define CURVEDWORLD_BEND_ID_1
			//#pragma shader_feature_local CURVEDWORLD_DISABLED_ON
			//#pragma shader_feature_local CURVEDWORLD_NORMAL_TRANSFORMATION_ON
			//#include "Assets/Amazing Assets/Curved World/Shaders/Core/CurvedWorldTransform.cginc"
			/* end CurvedWorld */

            ENDHLSL
        }
    }

    Fallback "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "StylizedSurfaceEditor"
}
