using UnityEngine;

public class RayMarchCamera : MonoBehaviour
{
    public Material SceneShaderMaterial;

    Material _material;
    Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _material = Instantiate(SceneShaderMaterial);
    }

    void Update()
    {
        _material.SetVector("_CamPos", transform.position.AsVector4());
        _material.SetVector("_CamDir", transform.forward.AsVector4());
        _material.SetVector("_CamUp",  transform.up.AsVector4());
        _material.SetFloat("_TanHalfFov", Mathf.Tan(_camera.fieldOfView * Mathf.Deg2Rad * .5f));
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
