//Copyright (c) 2016-2018 Kai Clavier [kaiclavier.com] Do Not Distribute
Shader "Super Text Mesh/User/16 Pass Outline" { 
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
                static float ANGLE = 0;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG

                CGPROGRAM
                static float ANGLE = 22.5;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG

                CGPROGRAM
                static float ANGLE = 45;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG

                CGPROGRAM
                static float ANGLE = 67.5;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG
                //1
                CGPROGRAM
                static float ANGLE = 90;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG

                CGPROGRAM
                static float ANGLE = 112.5;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG

                CGPROGRAM
                static float ANGLE = 135;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG

                CGPROGRAM
                static float ANGLE = 157.5;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG
                //2
                CGPROGRAM
                static float ANGLE = 180;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG

                CGPROGRAM
                static float ANGLE = 202.5;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG

                CGPROGRAM
                static float ANGLE = 225;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG

                CGPROGRAM
                static float ANGLE = 247.5;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG
                //3
                CGPROGRAM
                static float ANGLE = 270;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG

                CGPROGRAM
                static float ANGLE = 292.5;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG

                CGPROGRAM
                static float ANGLE = 315;
                #include "../STMOutline.cginc"
                #pragma surface surf Lambert alphatest:_Cutoff noforwardadd vertex:vert
                ENDCG

                CGPROGRAM
                static float ANGLE = 337.5;
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