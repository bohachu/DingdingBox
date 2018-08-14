//Copyright (c) 2016-2018 Kai Clavier [kaiclavier.com] Do Not Distribute
Shader "Super Text Mesh/SDF/Unlit Dropshadow and Outline" { 
	Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)
        [HideInInspector] _Cutoff ("Shadow Cutoff", Range(0,1)) = 0.0001 //should never change. can effect blend
        _SDFCutoff ("Cutoff", Range(0,1)) = 0.5
        _OutlineWidth ("Outline Width", Range(0,1)) = 0.2
        _ShadowDistance ("Shadow Distance", Range(0,1)) = 0.2
        _ShadowAngle ("Shadow Angle", Range(0,360)) = 135
        _Blend ("Blend Width", Range(0,1)) = 0.025 //would doing all this with a gradient be possible?
	}
    
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off Cull Off ZWrite Off Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
	//shadow 
		CGPROGRAM
        #include "../STMSDFDropshadow.cginc"
        #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
        ENDCG
	//outline
		//cardinals
	    CGPROGRAM
        static float ANGLE = 0;
        #include "../STMSDFOutline.cginc"
        #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
        ENDCG

        CGPROGRAM
        static float ANGLE = 90;
        #include "../STMSDFOutline.cginc"
        #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
        ENDCG

        CGPROGRAM
        static float ANGLE = 180;
        #include "../STMSDFOutline.cginc"
        #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
        ENDCG

        CGPROGRAM
        static float ANGLE = 270;
        #include "../STMSDFOutline.cginc"
        #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
        ENDCG
    //intercardinals
        CGPROGRAM
        static float ANGLE = 45;
        #include "../STMSDFOutline.cginc"
        #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
        ENDCG

        CGPROGRAM
        static float ANGLE = 135;
        #include "../STMSDFOutline.cginc"
        #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
        ENDCG

        CGPROGRAM
        static float ANGLE = 225;
        #include "../STMSDFOutline.cginc"
        #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
        ENDCG

        CGPROGRAM
        static float ANGLE = 315;
        #include "../STMSDFOutline.cginc"
        #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
        ENDCG

    //the actual letter
		CGPROGRAM
        #include "../STMSDFInside.cginc"
        #pragma surface surf Lambert alphatest:_Cutoff addshadow noforwardadd
        ENDCG
	}
}