using System;
using UnityEngine;

public class RayMarchCamera : MonoBehaviour
{
    Material _material;
    Camera _camera;

    GameObject _cube;

    void Start()
    {
        _camera = GetComponent<Camera>();

        var shader = Shader.Find("Generated/SceneShader");
        if (shader == null) {
            Debug.LogError("Could not find generated scene shader. Must run \"Build Shader\" on the RayMarchCamera.");
            Destroy(this);
            return;
        }

        _material = new Material(shader);

        _cube = GameObject.Find("Cube");
    }

    void Update()
    {
        _material.SetVector("_CamPos", transform.position.AsVector4());
        _material.SetVector("_CamDir", transform.forward.AsVector4());
        _material.SetVector("_CamUp",  transform.up.AsVector4());
        _material.SetFloat("_TanHalfFov", Mathf.Tan(_camera.fieldOfView * Mathf.Deg2Rad * .5f));
        _material.SetMatrix("_Cube", Matrix4x4.Inverse(_cube.transform.localToWorldMatrix));
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _material);
    }
}

static class Vec3_Ext {
    static public Vector4 AsVector4(this Vector3 vec, float w = 0) {
        return new Vector4 {
            x = vec.x, y = vec.y, z = vec.z, w = w
        };
    }
}
