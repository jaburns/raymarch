using UnityEngine;

public class RayMarchCube : RayMarchObject
{
    public override string GetDistanceFunction()
    {
        return "float $name(float3 p, float3 b) {"+
            "return length(max(abs(p)-b,0.0));"+
        "}";
    }

    void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}
