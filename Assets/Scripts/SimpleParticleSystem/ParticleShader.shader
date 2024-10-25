Shader "Custom/InstancedParticleColorWithAlpha"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent" "RenderType" = "Transparent"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha // Blend mode

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 color : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color) // 
            UNITY_INSTANCING_BUFFER_END(Props)

            sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 tex_color = tex2D(_MainTex, i.uv);

                return tex_color * i.color; // Sử dụng alpha từ texture để hỗ trợ vùng trong suốt
            }
            ENDCG
        }
    }
    FallBack "Transparent"
}