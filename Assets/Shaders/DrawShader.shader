Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BrushTex ("Brush", 2D) = "black" {}
        _sPos ("Position", Vector) = (0,0,0)
    }
    SubShader
    {
        Pass
        {
            ZTest Always Cull off ZWrite off
            Fog { mode off }
    
            CGPROGRAM
            #pragma fragment frag
            #pragma vertex vert
            sampler2D _MainTex;
            sampler2D _BrushTex;
            float2 _sPos;
            float2 _size = float2(6,6);
            float4 _color = float4(0,0,0,1);
    
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 v : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.v = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : COLOR
            {
                int x = floor(i.v.x - _sPos.x);
                int y = floor(i.v.y - _sPos.y);
                float4 c = tex2D(_MainTex, i.uv);
                if (x >= 0 &&
                    y >= 0 &&
                    x < _size.x &&
                    y < _size.y)
                {
                    float enabled = tex2D(_BrushTex, float2(x/_size.x, y/_size.y)).w * _color.w;
                    return c * (1-enabled) + float4(_color.x* (enabled), _color.y* (enabled), _color.z* (enabled), 1);
                }
	            return c;
            };
            ENDCG
        }
    }
    FallBack "Diffuse"
}
