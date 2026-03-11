using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public int Index;
    public GameObject gLeft, gRight;

    private void Awake()
    {
        this.ActionAfterTime(0.5f, () => Open());
    }

    public void Open(float time = 1f)
    {
        Tween.LocalRotation(gLeft.transform, new Vector3(0, -90, 0), time, 0f, Tween.EaseInOut);
        Tween.LocalRotation(gRight.transform, new Vector3(0, 90, 0), time, 0f, Tween.EaseInOut);
    }
}
