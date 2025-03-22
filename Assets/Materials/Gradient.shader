Shader "Unlit/Gradient"
{
    Properties
    {
        _TopColour("Top Colour", Color) = (1,1,1,1)
        _BottomColour("Bottom Colour", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR; // Ensure 'color' field exists
            };

            fixed4 _TopColour;
            fixed4 _BottomColour;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = lerp(_BottomColour, _TopColour, v.uv.y); // Corrected assignment
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color; // Use 'color' (not 'colour')
            }
            ENDCG
        }
    }
}
