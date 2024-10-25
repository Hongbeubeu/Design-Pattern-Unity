Shader "Simple/Particle"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _Scale("Scale", Float) = 1
        _MainTex("Particle Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha // Thêm blend mode để hỗ trợ alpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "UnityInstancing.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color) // Màu sắc riêng cho mỗi instance
                UNITY_DEFINE_INSTANCED_PROP(float, _Scale)  // Kích thước riêng cho mỗi instance
            UNITY_INSTANCING_BUFFER_END(Props)

            sampler2D _MainTex;

            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);

                v2f o;
                float scale = UNITY_ACCESS_INSTANCED_PROP(Props, _Scale);
                o.vertex = UnityObjectToClipPos(v.vertex * scale);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                float4 texColor = tex2D(_MainTex, i.uv);

                return texColor * color; // Sử dụng alpha từ texture để hỗ trợ vùng trong suốt
            }
            ENDCG
        }
    }
    FallBack "Transparent"
}
