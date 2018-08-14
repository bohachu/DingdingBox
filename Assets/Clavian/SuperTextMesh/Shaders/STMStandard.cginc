//Copyright (c) 2016-2018 Kai Clavier [kaiclavier.com] Do Not Distribute

sampler2D _MainTex;
sampler2D _MaskTex;

struct Input {
    float2 uv_MainTex : TEXCOORD0;
    float2 uv2_MaskTex : TEXCOORD1;
    float4 color : COLOR;
};

void surf (Input IN, inout SurfaceOutputStandard surface) {
    half4 text = tex2D (_MainTex, IN.uv_MainTex.xy);
    half4 mask = tex2D (_MaskTex, IN.uv2_MaskTex.xy);
    surface.Albedo = mask.rgb * IN.color.rgb;
    surface.Alpha = text.a * mask.a * IN.color.a;
}