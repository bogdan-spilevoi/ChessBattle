using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class OpponentMoveGraphic : MonoBehaviour
{
    public TMP_Text T_Info;
    private CanvasGroup self;
    public float timeToShow = 1f;

    private void Awake()
    {
        self = GetComponent<CanvasGroup>();
    }

    public void Create(Move move)
    {
        self.Activate();
        T_Info.text = $"Opponent used <b>{move.Name}</b>!";
        
        this.ActionAfterTime(timeToShow, () => self.Deactivate());
    }

    public void Create(Piece prev, Piece @new)
    {
        self.Activate();
        T_Info.text = $"Opponent switched <b>{prev.Name}</b> with <b>{@new.Name}</b>!";

        this.ActionAfterTime(timeToShow, () => self.Deactivate());
    }

    public void Create(PotionData potion)
    {
        self.Activate();
        T_Info.text = $"Opponent used <b>{potion.Name}</b> potion!";

        this.ActionAfterTime(timeToShow, () => self.Deactivate());
    }

    public void Create()
    {
        self.Activate();
        T_Info.text = $"Opponent <b>fled</b>!";

        this.ActionAfterTime(timeToShow, () => self.Deactivate());
    }
}
