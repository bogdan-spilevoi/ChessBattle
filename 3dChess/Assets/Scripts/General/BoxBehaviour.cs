using Pixelplacement;
using Pixelplacement.TweenSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxBehaviour : MonoBehaviour
{
    public List<GameObject> Pieces = new();
    public GameObject CurrentPiece;
    public GameObject Box;
    public GetPiece GetPiece;
    public Vector3 Hidden, Shown;
    private TweenBase ShakeAnim;
    public bool IsReady;
    public PlayerBehaviour player;

    public ParticleSystem Before, After;

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
        Before.Play();
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
        Before.Stop();
        After.Play();
        IsReady = false;
        ShakeAnim.Stop();
        Box.SetActive(false);        
        var p = player.GetPiece(Variants.GetRandom());

        CurrentPiece = Instantiate(Pieces[(int)p.PieceType], Pieces[(int)p.PieceType].transform.parent);
        CurrentPiece.GetComponent<MeshRenderer>().material = Resources.Load<Material>($"Materials/{p.Variant}");
        CurrentPiece.SetActive(true);

        Helper.ActionAfterTime(0.5f, () =>
        {
            Tween.LocalPosition(CurrentPiece.transform, CurrentPiece.transform.localPosition - new Vector3(0.6f, 0, 0), 0.5f, 0, Tween.EaseInOut);
            Helper.ActionAfterTime(0.5f, () =>
            {
                Tween.Rotate(CurrentPiece.transform, new Vector3(0, 0, 360), Space.Self, 5, 0, loop: Tween.LoopType.Loop);
                GetPiece.gameObject.SetActive(true);
                GetPiece.Create(p);
            });
        });
                
    }
    
    public void Close()
    {
        After.Stop();
        Movement.IsPaused = false;
        Destroy(CurrentPiece);
        GetPiece.gameObject.SetActive(false);
    }
}
