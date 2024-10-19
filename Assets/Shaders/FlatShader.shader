// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Made with Amplify Shader Editor v1.9.6.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FlatShader"
{
	Properties
	{
		[HDR]_BaseColor("BaseColor", Color) = (0,0,0,0)
		_MainTex("MainTex", 2D) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustomLighting keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 screenPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float4 _BaseColor;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			c.rgb = 0;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 appendResult70 = (float2(1.0 , ( _ScreenParams.y / _ScreenParams.x )));
			float4 matrixToPos58 = float4( unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
			float4 worldPos62 = matrixToPos58;
			float4 worldToClip67 = mul(UNITY_MATRIX_VP, float4(worldPos62.xyz, 1.0));
			float4 worldToClip67NDC = worldToClip67/worldToClip67.w;
			float3 worldToView72 = mul( UNITY_MATRIX_V, float4( worldPos62.xyz, 1 ) ).xyz;
			float4 texCoord86 = (float4( 0,0,0,0 ) + (( (float4( -1,-1,0,0 ) + (( float4( ( ( ( ( ( (ase_screenPosNorm).xy + -0.5 ) * 2.0 ) * appendResult70 ) + 1.0 ) * 0.5 ), 0.0 , 0.0 ) - ( float4( appendResult70, 0.0 , 0.0 ) * ( float4( float2( 0.5,-0.5 ), 0.0 , 0.0 ) * worldToClip67NDC ) ) ) - float4( 0,0,0,0 )) * (float4( 1,1,1,1 ) - float4( -1,-1,0,0 )) / (float4( 1,1,1,1 ) - float4( 0,0,0,0 ))) * worldToView72.z ) - float4( -1,-1,0,0 )) * (float4( 1,1,1,1 ) - float4( 0,0,0,0 )) / (float4( 1,1,1,1 ) - float4( -1,-1,0,0 )));
			o.Emission = ( _BaseColor * tex2D( _MainTex, ( ( texCoord86 * float4( _MainTex_ST.xy, 0.0 , 0.0 ) ) + float4( _MainTex_ST.zw, 0.0 , 0.0 ) ).xy ) ).rgb;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19603
Node;AmplifyShaderEditor.TransformVariables;57;-4740.374,-272.2183;Inherit;False;_Object2World;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.PosFromTransformMatrix;58;-4427.539,-260.0603;Inherit;False;1;0;FLOAT4x4;1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;62;-4159.648,-256.4146;Inherit;False;worldPos;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;61;-3636.792,-1336.265;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenParams;60;-3497.842,-1114.544;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;65;-3297.842,-1070.544;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;64;-3453.514,-1356.764;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;63;-3445.08,-656.6409;Inherit;False;62;worldPos;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;70;-3122.842,-1117.544;Inherit;False;FLOAT2;4;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TransformPositionNode;67;-3205.686,-679.4065;Inherit;False;World;Clip;True;Fast;True;1;0;FLOAT3;0,0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;69;-3213.514,-890.8448;Inherit;False;Constant;_Vector3;Vector 0;5;0;Create;True;0;0;0;False;0;False;0.5,-0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;68;-3303.032,-1341.787;Inherit;False;ConstantBiasScale;-1;;3;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT2;0,0;False;1;FLOAT;-0.5;False;2;FLOAT;2;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-2934.71,-1282.271;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-2856.71,-827.2706;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-2801.215,-1046.445;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;79;-2686.501,-1320.953;Inherit;False;ConstantBiasScale;-1;;4;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT2;0,0;False;1;FLOAT;1;False;2;FLOAT;0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;81;-2351.526,-1245.246;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;71;-3562.812,-115.8261;Inherit;False;62;worldPos;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TransformPositionNode;72;-3400.79,-146.0887;Inherit;False;World;View;False;Fast;True;1;0;FLOAT3;0,0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;82;-1814.828,-1275.57;Inherit;False;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;1,1,1,1;False;3;FLOAT4;-1,-1,0,0;False;4;FLOAT4;1,1,1,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-1499.515,-1127.421;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TFHCRemapNode;85;-1356.076,-1073.013;Inherit;False;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;-1,-1,0,0;False;2;FLOAT4;1,1,1,1;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;1,1,1,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TexturePropertyNode;9;-819,244;Inherit;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;0;False;0;False;None;44efd0011d6a9bc4fb0b3a82753dac4e;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RegisterLocalVarNode;86;-1114.156,-1105.529;Inherit;False;texCoord;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;87;-573.5441,-27.51144;Inherit;False;86;texCoord;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureTransformNode;12;-526,276;Inherit;False;-1;False;1;0;SAMPLER2D;;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-355,-16;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-173.1558,1.550092;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;2;35.45939,-362;Inherit;False;Property;_BaseColor;BaseColor;0;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;3;-25,50;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;193,-75;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;319,-133;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;FlatShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;56;-4790.374,-322.2183;Inherit;False;855.5251;266.1579;;0;World Position;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;59;-3724.021,-1511.265;Inherit;False;1551.285;1074.304;TexCoord;0;StableScreenCoords;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;66;-3645.671,-343.2888;Inherit;False;1202.444;523.4009;;0;DistanceScaleFactor;1,1,1,1;0;0
WireConnection;58;0;57;0
WireConnection;62;0;58;0
WireConnection;65;0;60;2
WireConnection;65;1;60;1
WireConnection;64;0;61;0
WireConnection;70;1;65;0
WireConnection;67;0;63;0
WireConnection;68;3;64;0
WireConnection;74;0;68;0
WireConnection;74;1;70;0
WireConnection;73;0;69;0
WireConnection;73;1;67;0
WireConnection;76;0;70;0
WireConnection;76;1;73;0
WireConnection;79;3;74;0
WireConnection;81;0;79;0
WireConnection;81;1;76;0
WireConnection;72;0;71;0
WireConnection;82;0;81;0
WireConnection;84;0;82;0
WireConnection;84;1;72;3
WireConnection;85;0;84;0
WireConnection;86;0;85;0
WireConnection;12;0;9;0
WireConnection;14;0;87;0
WireConnection;14;1;12;0
WireConnection;15;0;14;0
WireConnection;15;1;12;1
WireConnection;3;0;9;0
WireConnection;3;1;15;0
WireConnection;6;0;2;0
WireConnection;6;1;3;0
WireConnection;0;2;6;0
ASEEND*/
//CHKSM=0CD685900A4972FCE842D5C5B590E9BD28022AB6