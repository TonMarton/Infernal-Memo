using UnityEngine;

internal static class MathUtils
{
    internal static Vector3 GetDirectionXZ(Vector3 origin, Vector3 destination)
    {
        Vector3 result = destination - origin;
        result.y = 0;
        result.Normalize();
        return result;
    }
}
