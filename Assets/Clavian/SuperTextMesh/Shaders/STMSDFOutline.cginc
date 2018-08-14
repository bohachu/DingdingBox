//Copyright (c) 2016-2018 Kai Clavier [kaiclavier.com] Do Not Distribute

sampler2D _MainTex;
sampler2D _MaskTex;
float4 _OutlineColor;
float _OutlineWidth;
float _SDFCutoff;
float _Blend;

struct Input {
    float2 uv_MainTex : TEXCOORD0;
    float2 uv2_MaskTex : TEXCOORD1;
    float4 color : COLOR;
};

//uniform float ANGLE = 135;

void vert (inout appdata_full v) { //modify vertex data
    v.vertex.x += sin(ANGLE * 0.01745327777) * _OutlineWidth;
    v.vertex.y += cos(ANGLE * 0.01745327777) * _OutlineWidth;
}
void surf (Input IN, inout SurfaceOutput surface) {
    half4 text = tex2D (_MainTex, IN.uv_MainTex.xy);
    half4 mask = tex2D (_MaskTex, IN.uv2_MaskTex.xy);
    if(text.a < _SDFCutoff){
        surface.Alpha = 0; //cut!
    }
    else if(text.a < _SDFCutoff + _Blend){ //blend between nothing and outline
        surface.Emission = _OutlineColor;
        surface.Alpha = (text.a - _SDFCutoff + (_Blend/100)) / _Blend * _OutlineColor.a;
    }
    else{
        surface.Emission = mask.rgb * _OutlineColor.rgb;
        surface.Alpha = mask.a * _OutlineColor.a * IN.color.a;
    }
}