// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Archanor/Hyperspace/WarpTunnelCullFront"
{
	Properties
	{
		_WarpTexture("Warp Texture", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_Power("Power", Range( 0 , 5)) = 1
		_Color("Color", Color) = (0,0,0,0)
		_ScrollSpeed("Scroll Speed", Range( 0 , 15)) = 1
		_Transparency("Transparency", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Front
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		uniform float _Power;
		uniform sampler2D _WarpTexture;
		uniform float _ScrollSpeed;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform float _Transparency;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 panner4 = ( _Time.y * ( float2( 0.1,-0.3 ) * _ScrollSpeed ) + i.uv_texcoord);
			float4 tex2DNode5 = tex2D( _WarpTexture, panner4 );
			o.Emission = ( ( _Color * _Power ) + tex2DNode5 ).rgb;
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			o.Alpha = ( tex2DNode5 * tex2D( _Mask, uv_Mask ) * _Transparency ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18930
196;179;1461;899;1615.941;493.8669;1.372404;True;False
Node;AmplifyShaderEditor.Vector2Node;2;-1335.247,76.17635;Inherit;False;Constant;_Vector0;Vector 0;0;0;Create;True;0;0;0;False;0;False;0.1,-0.3;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;24;-1424.958,219.0222;Inherit;False;Property;_ScrollSpeed;Scroll Speed;4;0;Create;True;0;0;0;False;0;False;1;1;0;15;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-1137.55,-39.43662;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;3;-1101.798,248.2703;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1070.194,112.3619;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;4;-853.3087,21.91847;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-753.2391,-133.4177;Inherit;False;Property;_Power;Power;2;0;Create;True;0;0;0;False;0;False;1;5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;11;-687.8374,-319.1761;Inherit;False;Property;_Color;Color;3;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.396652,0.2028302,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;28;-556.4561,589.37;Inherit;False;Property;_Transparency;Transparency;5;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-402.1899,-190.1928;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;9;-596.4122,381.9837;Inherit;True;Property;_Mask;Mask;1;0;Create;True;0;0;0;False;0;False;-1;6e5c8ea417a4fda478db07a14d6b8ed4;6e5c8ea417a4fda478db07a14d6b8ed4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-659.9388,-6.907583;Inherit;True;Property;_WarpTexture;Warp Texture;0;0;Create;True;0;0;0;False;0;False;-1;None;539af42ad6769684abbd032a7f23fc79;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-215.0985,90.80115;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-224.5994,350.494;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Archanor/Hyperspace/WarpTunnelCullFront;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Front;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;23;0;2;0
WireConnection;23;1;24;0
WireConnection;4;0;1;0
WireConnection;4;2;23;0
WireConnection;4;1;3;0
WireConnection;12;0;11;0
WireConnection;12;1;13;0
WireConnection;5;1;4;0
WireConnection;26;0;12;0
WireConnection;26;1;5;0
WireConnection;27;0;5;0
WireConnection;27;1;9;0
WireConnection;27;2;28;0
WireConnection;0;2;26;0
WireConnection;0;9;27;0
ASEEND*/
//CHKSM=14D567134059853D69FD22B3E3401EA3BC3042E9