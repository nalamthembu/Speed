Shader "Custom/VehicleShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(1,0.1)) = 0.0
        _HeadLightColour("HeadLightColour", Color) = (1,1,1,1)
         _CellSize("Cell Size", Range(0.1, 10)) = 0.0
        _HeadLightEmission("Headlight Emission Map", 2D) = "black"{}
        _TailLightEmission("Taillight Emission Map", 2D) = "black"{}
        _Braking("Braking", Range(0, 10)) = 0.0
        _HeadLightBrightness("Headlight Brightness", Range(0, 10)) = 0.0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }

            CGPROGRAM

            #include "Random.cginc"

            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            sampler2D _MainTex;
            sampler2D _HeadLightEmission;
            sampler2D _TailLightEmission;

            struct Input
            {
                float2 uv_MainTex;
                float3 worldPos;
            };

            half _Glossiness;
            half _Braking;
            half _HeadLightBrightness;
            fixed4 _HeadLightColour;
            half _CellSize;

            half _Metallic;
            fixed4 _Color;

            float2 voronoiNoise(float2 value) {
                float2 baseCell = floor(value);

                float minDistToCell = 10;
                float2 closestCell;
                [unroll]
                for (int x = -1; x <= 1; x++) {
                    [unroll]
                    for (int y = -1; y <= 1; y++) {
                        float2 cell = baseCell + float2(x, y);
                        float2 cellPosition = cell + rand2dTo2d(cell);
                        float2 toCell = cellPosition - value;
                        float distToCell = length(toCell);
                        if (distToCell < minDistToCell) {
                            minDistToCell = distToCell;
                            closestCell = cell;
                        }
                    }
                }
                float random = rand2dTo1d(closestCell);
                return float2(minDistToCell, random);
            }


            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

                float2 value = IN.uv_MainTex / (_CellSize / 1000);
                float noise = voronoiNoise(value).y;

                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness + noise * 2;

                fixed4 headLights = tex2D(_HeadLightEmission, IN.uv_MainTex) * (_HeadLightBrightness * _HeadLightBrightness);
                fixed4 tailLights = (tex2D(_TailLightEmission, IN.uv_MainTex) * (_Braking));

                fixed4 overAllEmission = headLights * _HeadLightColour + tailLights;

                o.Emission = overAllEmission;
                o.Alpha = c.a;

                

                o.Albedo = c.rgb;
            }
            ENDCG
        }
            FallBack "Standard"
}
