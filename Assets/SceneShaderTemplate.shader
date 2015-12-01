Shader "Custom/SceneShaderTemplate" { // __SHADER_TITLE
    Properties {
        _MainTex ("Base (RGB)", 2D) = "" {}
    }

CGINCLUDE

    #include "UnityCG.cginc"

    struct v2f
    {
        float4 pos : SV_POSITION;
        half2 uv : TEXCOORD0;
    };

    uniform sampler2D _MainTex;
    uniform float4 _CamPos;
    uniform float4 _CamDir;
    uniform float4 _CamUp;
    uniform float _TanHalfFov;
    uniform float4x4 _Cube; // __UNIFORMS

// ----------------------------------------------------------------------------

float distfunc(float3 p) {return length(p)-1;} // __DISTANCE_FUNCTION

// ----------------------------------------------------------------------------

    const int MAX_ITER = 50;
    const float MAX_DIST = 30.0;
    const float EPSILON = 0.01;

    float distfunc_1(float3 pos)
    {
        float4 liftedPos = float4(pos,1);
        float4 transformed = mul(_Cube, liftedPos);
        return distfunc(transformed.xyz);
    }

    bool march (float3 ro, float3 rd, out float3 pos)
    {
        float totalDist = 0.0;
        pos = ro;
        float dist = EPSILON;

        for (int i = 0; i < MAX_ITER; i++) {
            if (dist < EPSILON || totalDist > MAX_DIST) break;
            dist = distfunc_1(pos);
            totalDist += dist;
            pos += dist * rd;
        }

        return dist < EPSILON;
    }

    v2f vert(appdata_img v)
    {
        v2f o;
        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
        o.uv = v.texcoord.xy;
        return o;
    }

    float4 frag(v2f i) : SV_Target
    {
        const float3 cameraOrigin = _CamPos.xyz;
        const float3 cameraDir    = _CamDir.xyz;
        const float3 cameraUp     = _CamUp.xyz;
        const float3 cameraRight  = cross(cameraUp, cameraDir);
        const float2 screenPos = -1 + 2 * i.uv;
        screenPos.x *= _ScreenParams.x / _ScreenParams.y;
        screenPos *= _TanHalfFov;
        const float3 rayDir = normalize(cameraRight * screenPos.x + cameraUp * screenPos.y + cameraDir);

        float3 pos;
        bool hit = march (cameraOrigin, rayDir, pos);

        if (hit) {
            const float2 eps = float2(0.0, EPSILON);

            float3 normal = normalize(float3(
                distfunc_1(pos + eps.yxx) - distfunc_1(pos - eps.yxx),
                distfunc_1(pos + eps.xyx) - distfunc_1(pos - eps.xyx),
                distfunc_1(pos + eps.xxy) - distfunc_1(pos - eps.xxy)));

            return float4(float3(0.5) + normal/2, 1);
        }

        return tex2D(_MainTex, i.uv);
    }

ENDCG

    Subshader {
        Pass {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }

    Fallback off
}
