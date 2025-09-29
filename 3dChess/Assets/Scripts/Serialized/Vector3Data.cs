using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Data
{
    public float x, y, z;
    public Vector3Data() { }
    public Vector3Data(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public static implicit operator Vector3Data(Vector3 v) => new(v);
    public static implicit operator Vector3(Vector3Data v) => new(v.x, v.y, v.z);
}
