//Copyright (c) 2016-2018 Kai Clavier [kaiclavier.com] Do Not Distribute
Shader "Super Text Mesh/UI/Dropshadow" { 
	Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)
        _ShadowDistance ("Shadow Distance", Range(0,1)) = 0.05
        _ShadowAngle ("Shadow Angle", Range(0,360)) = 135
	}
	SubShader {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Lighting Off Cull Off ZWrite Off Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
//dropshadow
        CGPROGRAM
        #include "../STMDropshadow.cginc"
        #pragma surface surf Lambert alpha vertex:vert
        ENDCG
//normal text
        CGPROGRAM
        #include "../STM.cginc"
        #pragma surface surf Lambert alpha
        ENDCG
	}
}