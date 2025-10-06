using Pixelplacement;
using Pixelplacement.TweenSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBehaviour : MonoBehaviour
{
    public GameObject Box;
    public Vector3 Hidden, Shown;
    private TweenBase ShakeAnim;
    public bool IsReady;
    public PlayerBehaviour player;

    void Start()
    {
        
    }

    private void Update()
    {
        if (IsReady && Input.GetKeyDown(KeyCode.O))
        {
            OpenBox();
        }
    }

    public void PrepareBox()
    {
        Movement.IsPaused = true;
        Box.transform.localPosition = Hidden;
        Box.SetActive(true);
        Tween.LocalPosition(Box.transform, Shown, 0.5f, 0, Tween.EaseOut);
        Helper.ActionAfterTime(0.5f, () => {
            ShakeAnim = Tween.Shake(Box.transform, Box.transform.localPosition, Vector3.one * 0.01f, 2, 0, loop: Tween.LoopType.Loop);
            IsReady = true;
        });       
    }
    public void OpenBox()
    {
        Movement.IsPaused = false;
        IsReady = false;
        ShakeAnim.Stop();
        Box.SetActive(false);        
        var p = player.GetPiece(Variants.GetRandom());
    }
}
