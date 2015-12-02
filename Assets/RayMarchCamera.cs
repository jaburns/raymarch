using UnityEngine;

public class RayMarchCamera : MonoBehaviour
{
    public const string SHADER_PATH_PREFIX = "rmcShader_";

    public Shader ShaderTemplate;

    Material _material;
    Camera _camera;
    RayMarchObject[] _objects;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _camera.depthTextureMode = DepthTextureMode.Depth;

        var shader = Resources.Load<Shader>(SHADER_PATH_PREFIX + GetInstanceID());
        if (shader == null) {
            Debug.LogError("Could not find generated scene shader. Must run \"Build Shader\" on the RayMarchCamera.");
            Destroy(this);
            return;
        }

        _material = new Material(shader);
        _objects = FindObjectsOfType<RayMarchObject>();
    }

    void Update()
    {
        _material.SetVector("_CamPos", transform.position.AsVector4());
        _material.SetVector("_CamDir", transform.forward.AsVector4());
        _material.SetVector("_CamUp",  transform.up.AsVector4());
        _material.SetFloat("_TanHalfFov", Mathf.Tan(_camera.fieldOfView * Mathf.Deg2Rad * .5f));

        foreach (var obj in _objects) {
            _material.SetMatrix("_obj_"+obj.name, obj.GetInverseMatrix());
        }
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
