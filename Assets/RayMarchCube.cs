using UnityEngine;

public class RayMarchCube : RayMarchObject
{
    public Vector3 Size = Vector3.one;
    public float Radius = .1f;

    public override string GetDistanceFunction()
    {
        return string.Format(
            "const float3 b = float3({0},{1},{2});" +
            "const float r = {3};" +
            "return length(max(abs(p)-b,0.0))-r;"
          , .5f*Size.x
          , .5f*Size.y
          , .5f*Size.z
          , Radius
        );
    }

    void OnDrawGizmos()
    {
        var realSize = new Vector3 {
            x = transform.lossyScale.x * Size.x,
            y = transform.lossyScale.y * Size.y,
            z = transform.lossyScale.z * Size.z
        };

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, realSize);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}
