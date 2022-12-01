using UnityEngine;

internal static class MathUtils
{
    internal static Vector3 GetDirectionXZ(Vector3 a, Vector3 b)
    {
        Vector3 result = b - a;
        result.y = 0;
        result.Normalize();
        return result;
    }
}
