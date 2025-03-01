Shader "Custom/TransparentDoubleSided"
{
    Properties
    {
        _Color ("Base Color", Color) = (0, 1, 0, 0.5) // Màu vật thể
        _Transparency ("Transparency", Range(0, 1)) = 0.5 // Độ trong suốt
        _FresnelPower ("Fresnel Power", Range(0, 5)) = 2.0 // Cường độ Fresnel
        _FresnelIntensity ("Fresnel Intensity", Range(0, 1)) = 0.5 // Độ sáng Fresnel
        _BlinkSpeed ("Blink Speed", Range(0, 10)) = 2.0 // Tốc độ nhấp nháy
        _BlinkMin ("Blink Min", Range(0, 1)) = 0.3
        _BlinkMax ("Blink Max", Range(0, 1)) = 0.8
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            // 🔹 Render mặt sau trước
            Cull Front
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float fresnel : TEXCOORD0;
                float blink : TEXCOORD1;
            };

            float4 _Color;
            float _Transparency;
            float _FresnelPower;
            float _FresnelIntensity;
            float _BlinkSpeed;
            float _BlinkMin;
            float _BlinkMax;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                o.fresnel = pow(1.0 - abs(dot(viewDir, v.normal)), _FresnelPower) * _FresnelIntensity;

                float sinValue = sin(_Time.y * _BlinkSpeed) * 0.5 + 0.5;
                o.blink = lerp(_BlinkMin, _BlinkMax, sinValue);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float fresnelEffect = i.fresnel * i.blink;
                float alpha = _Color.a * _Transparency * i.blink;

                return float4(_Color.rgb + fresnelEffect, alpha);
            }
            ENDCG
        }

        Pass
        {
            // 🔹 Render mặt trước sau
            Cull Back
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float fresnel : TEXCOORD0;
                float blink : TEXCOORD1;
            };

            float4 _Color;
            float _Transparency;
            float _FresnelPower;
            float _FresnelIntensity;
            float _BlinkSpeed;
            float _BlinkMin;
            float _BlinkMax;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                o.fresnel = pow(1.0 - abs(dot(viewDir, v.normal)), _FresnelPower) * _FresnelIntensity;

                float sinValue = sin(_Time.y * _BlinkSpeed) * 0.5 + 0.5;
                o.blink = lerp(_BlinkMin, _BlinkMax, sinValue);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float fresnelEffect = i.fresnel * i.blink;
                float alpha = _Color.a * _Transparency * i.blink;

                return float4(_Color.rgb + fresnelEffect, alpha);
            }
            ENDCG
        }
    }
}
