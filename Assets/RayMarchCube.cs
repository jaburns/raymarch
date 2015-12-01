using UnityEngine;

public class RayMarchCube : RayMarchObject
{
    public override string GetDistanceFunction()
    {
        return string.Format(
                "float box(float3 p, float3 b) {{" +
                "    float3 d = abs(p) - b;" +
                "    return min(max(d.x,max(d.y,d.z)),0.0) + length(max(d,0.0));" +
                "}}" +
                "float distfunc (float3 p) {{" +
                "    return box(p, float3({0},{1},{2}));" +
                "}}"
            , .5f*transform.lossyScale.x
            , .5f*transform.lossyScale.y
            , .5f*transform.lossyScale.z
        );
    }

    void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}
