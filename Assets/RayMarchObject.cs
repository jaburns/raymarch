using UnityEngine;

abstract public class RayMarchObject : MonoBehaviour
{
    public abstract string GetDistanceFunction();

    public virtual Matrix4x4 GetInverseMatrix()
    {
        return Matrix4x4.Inverse(transform.localToWorldMatrix);
    }
}
