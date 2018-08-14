//Copyright (c) 2016-2018 Kai Clavier [kaiclavier.com] Do Not Distribute
Shader "Super Text Mesh/User/Wacky Outline" { 
        Properties {
                _MainTex ("Font Texture", 2D) = "white" {}
                _MaskTex ("Mask Texture", 2D) = "white" {}
                [HideInInspector] _Cutoff ("Cutoff", Range(0,1)) = 0.0001
                _OutlineColor ("Outline Color", Color) = (0,0,0,1)
                _OutlineWidth ("Outline Width", Range(0,1)) = 0.05
	}
	SubShader {
                Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
                Lighting Off Cull Off ZWrite Off Fog { Mode Off }
                Blend SrcAlpha OneMinusSrcAlpha
                //0
                CGPROGRAM
                static float ANGLE = _Time.w * 99;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG

                CGPROGRAM
                static float ANGLE = _Time.w * 199;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG

                CGPROGRAM
                static float ANGLE = _Time.w * 199;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG

                CGPROGRAM
                static float ANGLE = _Time.w * 299;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG
                //text
                CGPROGRAM
                #include "../STM.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff addshadow noforwardadd
                ENDCG

	}
}