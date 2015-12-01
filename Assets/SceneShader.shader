Shader "Custom/SceneShader" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "" {}
        _CamPos ("CamPos", Vector) = (0,0,0,0)
        _CamDir ("CamDir", Vector) = (0,0,0,0)
        _CamUp ("CamUp", Vector) = (0,0,0,0)
        _CamFov ("CamFov", Float) = 60
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
    uniform float _CamFov;

// ----------------------------------------------------------------------------

        float3 rot (float3 p, float x, float y, float z) {
            float3 a = normalize (float3 (sin(2.0+x),sin(2.0+y),sin(2.0+z)));
            float s = sin (_Time.y);
            float c = cos (_Time.y);
            float oc = 1.0 - c;
            float4x4 m = float4x4(oc*a.x*a.x + c,      oc*a.x*a.y - a.z*s,  oc*a.z*a.x + a.y*s,  0.0,
                        oc*a.x*a.y + a.z*s,  oc*a.y*a.y + c,      oc*a.y*a.z - a.x*s,  0.0,
                        oc*a.z*a.x - a.y*s,  oc*a.y*a.z + a.x*s,  oc*a.z*a.z + c,      0.0,
                        0.0,                 0.0,                 0.0,                 1.0);
            return mul(m,float4(p,0.0)).xyz;
        }
        float ball (float3 p, float x, float y, float z) {
            const float BOX_SIZE = 0.7;
            const float BOX_EDGE = 0.2;
            return length(max(abs(rot(p-float3(x,y,z),x,y,z))-float3(BOX_SIZE),0.0))-BOX_EDGE;
        }
        float ballA (float3 p) {
            return ball (p, 3.0*sin(0.8*_Time.y), 0.5, 0.0);
        }
        float ballB (float3 p) {
            return ball (p, 0.5, 2.0*sin (0.2*_Time.y), 3.0*cos (1.1*_Time.y));
        }
        float ballC (float3 p) {
            return ball (p, 0.1, 3.0*cos (_Time.y), 0.3);
        }
        float ballD (float3 p) {
            return ball (p, 2.0*cos (0.3*_Time.y), 1.5*sin (2.0*_Time.y), -0.2);
        }
        float blend (float a, float b) {
            const float k = 1.0;
            float h = clamp (0.5+0.5*(b-a)/k, 0.0, 1.0);
            return lerp(b,a,h) - k*h*(1.0-h);
        }
        float distfunc (float3 p) {
            return blend (blend (ballA (p), ballB (p)),
                        blend (ballC (p), ballD (p)));
        }

// ----------------------------------------------------------------------------

    const int MAX_ITER = 50;
    const float MAX_DIST = 30.0;
    const float EPSILON = 0.01;

    bool march (float3 ro, float3 rd, out float3 pos)
    {
        float totalDist = 0.0;
        pos = ro;
        float dist = EPSILON;

        for (int i = 0; i < MAX_ITER; i++) {
            if (dist < EPSILON || totalDist > MAX_DIST) break;
            dist = distfunc(pos);
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
        screenPos *= _CamFov / 90;
        const float3 rayDir = normalize(cameraRight * screenPos.x + cameraUp * screenPos.y + cameraDir);

        float3 pos;
        bool hit = march (cameraOrigin, rayDir, pos);

        float3 color = float3 (0.0);

        if (hit) {
            const float2 eps = float2(0.0, EPSILON);

            float3 normal = normalize(float3(
                distfunc(pos + eps.yxx) - distfunc(pos - eps.yxx),
                distfunc(pos + eps.xyx) - distfunc(pos - eps.xyx),
                distfunc(pos + eps.xxy) - distfunc(pos - eps.xxy)));

            color = float3(0.5) + normal/2;
        }
        else {
            color = float3(0);
        }

        return float4(color, 1.0);
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
