//Copyright (c) 2016-2018 Kai Clavier [kaiclavier.com] Do Not Distribute

//base stuff that can be used by any STM shader

sampler2D _MainTex;
sampler2D _MaskTex;

//appdata_t
struct Input {
    float2 uv_MainTex : TEXCOORD0;
    float2 uv2_MaskTex : TEXCOORD1;
    fixed4 color : COLOR;
};
//render normal text
void surf (Input IN, inout SurfaceOutput surface) {
    half4 text = tex2D (_MainTex, IN.uv_MainTex.xy);
    half4 mask = tex2D (_MaskTex, IN.uv2_MaskTex.xy);
    surface.Emission = mask.rgb * IN.color.rgb;
    surface.Alpha = text.a * mask.a * IN.color.a;
}
//vertex pixel snap
void vert(inout appdata_full v){
    v.vertex = UnityPixelSnap (v.vertex);
}