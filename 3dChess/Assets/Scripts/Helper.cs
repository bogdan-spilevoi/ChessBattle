using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Helper : MonoBehaviour
{
    public static void FitImageToSize(Image i, float d)
    {
        i.SetNativeSize();
        float max = Mathf.Max(i.rectTransform.sizeDelta.x, i.rectTransform.sizeDelta.y);
        float rate = max / d;
        i.rectTransform.sizeDelta /= rate;
    }

    public static async void ActionAfterTime(float time, Action A)
    {
        await Task.Delay((int)(time * 1000));
        A?.Invoke();
    }

}
