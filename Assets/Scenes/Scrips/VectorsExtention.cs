using UnityEngine;
using UnityEngine.Animations;

public static class Vector3Extensions
{
    public static Vector3 WithAxis(this Vector3 v, Axis axis, float value)
    {
        switch (axis)
        {
            case Axis.X:
                v.x = value;
                break;
            case Axis.Y:
                v.y = value;
                break;
            case Axis.Z:
                v.z = value;
                break;
        }
        return v;
    }
}
