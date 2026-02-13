using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ImageExtensions
{
    public static void Fit(this Image image, float size)
    {
        image.SetNativeSize();
        float max = Mathf.Max(image.rectTransform.sizeDelta.x, image.rectTransform.sizeDelta.y);
        float rate = max / size;
        image.rectTransform.sizeDelta /= rate;
    }
}
