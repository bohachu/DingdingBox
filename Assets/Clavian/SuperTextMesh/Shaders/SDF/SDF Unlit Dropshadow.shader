//Copyright (c) 2016-2018 Kai Clavier [kaiclavier.com] Do Not Distribute
Shader "Super Text Mesh/SDF/Unlit Dropshadow" { 
	Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)
        [HideInInspector] _Cutoff ("Shadow Cutoff", Range(0,1)) = 0.0001 //should never change. can effect blend
        _SDFCutoff ("Cutoff", Range(0,1)) = 0.5
        _ShadowDistance ("Shadow Distance", Range(0,1)) = 0.2
        _ShadowAngle ("Shadow Angle", Range(0,360)) = 135
        _Blend ("Blend Width", Range(0,1)) = 0.025 //would doing all this with a gradient be possible?
	}
    
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off Cull Off ZWrite Off Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
	//dropshadow
		CGPROGRAM
        #include "../STMSDFDropshadow.cginc"
        #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
        ENDCG

	//normal text
		CGPROGRAM
        #include "../STMSDF.cginc"
        #pragma surface surf Lambert alphatest:_Cutoff addshadow noforwardadd
        ENDCG
	}
}