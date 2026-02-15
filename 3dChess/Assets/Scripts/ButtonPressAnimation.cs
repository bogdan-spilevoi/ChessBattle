using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Pixelplacement;

public class ButtonPressAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private readonly static WaitForSecondsRealtime _waitForSecondsRealtime0_15 = new(0.15f);
    private bool wait = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (gameObject.GetComponent<Button>().interactable == false)
        {
            return;
        }
        
        if(wait)
        {
            return;
        }

        Tween.LocalScale(gameObject.transform, new Vector3(0.75f, 0.75f, 0.75f), 0.15f, 0, Tween.EaseOut); 
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        wait = true;
        Tween.LocalScale(gameObject.transform, Vector3.one, 0.15f, 0, Tween.EaseIn);
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return _waitForSecondsRealtime0_15;
        wait = false;
    }
}
