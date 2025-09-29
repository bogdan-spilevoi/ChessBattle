using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
}
