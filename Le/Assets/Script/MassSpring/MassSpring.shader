Shader "MassSpring/MassSpringShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows addshadow vertex:vert

        struct Input
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float4 _MainPos, _FollowPos;//world space
        float _MeshH, _W_Bottom;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            float3 mainPos = mul(unity_WorldToObject, _MainPos).xyz;//主动点在模型空间的位置
            float3 follow = mul(unity_WorldToObject, _FollowPos).xyz;//从动点在模型空间的位置
            float3 offDir = follow - mainPos;//偏移方向
            float3 followVert = v.vertex.xyz + offDir;//从动的模型顶点进行位置偏移
            float3 wPos = mul(unity_ObjectToWorld, v.vertex).xyz;//模型的世界坐标
            float mask = (wPos.y - _W_Bottom) / max(0.00001, _MeshH);//将模型世界顶点y值, 映射[0, 1], 作为上下的遮罩
            v.vertex.xyz = lerp(v.vertex.xyz, followVert, mask);//用遮罩来插值顶点该的主动点坐标, 还是从动点坐标
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}