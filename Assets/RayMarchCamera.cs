using UnityEngine;

public class RayMarchCamera : MonoBehaviour
{
    public Material SceneShaderMaterial;

    Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {

        SceneShaderMaterial.SetVector("_CamPos", transform.position.AsVector4());
        SceneShaderMaterial.SetVector("_CamDir", transform.forward.AsVector4());
        SceneShaderMaterial.SetVector("_CamUp",  transform.up.AsVector4());
        SceneShaderMaterial.SetFloat("_CamFov", _camera.fieldOfView);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, SceneShaderMaterial);
    }
}

static class Vec3_Ext {
    static public Vector4 AsVector4(this Vector3 vec, float w = 0) {
        return new Vector4 {
            x = vec.x, y = vec.y, z = vec.z, w = w
        };
    }
}
