//Copyright (c) 2016-2018 Kai Clavier [kaiclavier.com] Do Not Distribute
Shader "Super Text Mesh/UI/Masked" { 
	Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
	}
    
	SubShader {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Lighting Off Cull Off ZWrite Off Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		//required for UI.Mask
		Stencil {
			Ref 0
			Comp Less
		}
		CGPROGRAM
        #include "../STM.cginc"
        #pragma surface surf Lambert alpha
        ENDCG
	}
}