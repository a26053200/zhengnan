Shader "SFA/UVAnim" 
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Sequence Image", 2D) = "white" {}
        _Speed("Speed", Range(1,100)) = 50
        _HorizontalAmount ("Horizontal Amount",float) = 4
        _VerticalAmount ("Vertical Amount",float) = 4
    }
    SubShader
    {
        Tags{"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

        Pass
        {
            Tags{"LightMode" = "ForwardBase"}
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Speed;
            float _HorizontalAmount;
            float _VerticalAmount;

            float4 _Rects[64];

            struct a2v
            {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                //float time = floor(_Time.y * _Speed) / 64;
                //int idx = floor(fmod(time, 64));
                float4 rect = _Rects[1];
                o.uv = o.uv * rect.zw + float2(rect.y,-rect.x);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //float time = floor(_Time.y * _Speed);
                //float row = floor(time / _HorizontalAmount);
                //float colum = time - row * _HorizontalAmount;
                //half2 uv = i.uv + half2(colum,-row);
                //uv.x /=  _HorizontalAmount;
                //uv.y /=  _VerticalAmount;

                fixed4 c = tex2D(_MainTex,i.uv);
                c.rgb += _Color;
                return c;
            }


            ENDCG

        }

    }
    Fallback "Transparent/VertexLit"
}