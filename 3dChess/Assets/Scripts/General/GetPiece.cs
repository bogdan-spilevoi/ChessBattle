using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetPiece : MonoBehaviour
{
    public EntityData thisEntity;
    public TMP_Text T_Name, T_Type;
    public Image Icon;
    public GetMoveUI OriginalGetMoveUI;
    public List<GetMoveUI> CurrentMoves = new();
    public Slider S_Attack, S_Defense, S_Speed, S_Luck;
    

    public void Create(EntityData e)
    {
        thisEntity = e;
        T_Name.text = e.Name;
        T_Type.text = "<b>" + e.Variant.ToUpper() + "</b> " + e.PieceType.ToString();

        S_Attack.maxValue = 10;
        S_Defense.maxValue = 1000;
        S_Speed.maxValue = 10;
        S_Luck.maxValue = 10;

        S_Attack.value = e.Attack;
        S_Defense.value = e.MaxHealth;
        S_Speed.value = e.Speed;
        S_Luck.value = e.Luck;

        foreach (var m in CurrentMoves)
        {
            Destroy(m.gameObject);
        }
        CurrentMoves.Clear();

        for (int i = 0; i < e.Moves.Count; i++)
        {
            var m = e.Moves[i];
            var newMoveUI = Instantiate(OriginalGetMoveUI, OriginalGetMoveUI.transform.parent);
            CurrentMoves.Add(newMoveUI);
            newMoveUI.gameObject.SetActive(true);
            newMoveUI.Create(m, Move.MoveIndToLvlRequired(i) > e.Level, Move.MoveIndToLvlRequired(i));
        }

        GetIcon();
    }

    public void GetIcon()
    {
        Sprite mySprite = Resources.Load<Sprite>($"Icons/{thisEntity.PieceType}/{thisEntity.Variant}");

        Icon.sprite = mySprite != null ? mySprite : Resources.Load<Sprite>($"Icons/{thisEntity.PieceType}/basic");
        Icon.Fit(100);
    }
}
