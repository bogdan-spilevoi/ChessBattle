using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static void ChangeX(this Vector3 v, float x)
    {
        v.x = x;
    }

    public static void ChangeY(this Vector3 v, float y)
    {
        v.y = y;
    }

    public static void AddToY(this Vector3 v, float y)
    {
        v.y += y;
    }
}
