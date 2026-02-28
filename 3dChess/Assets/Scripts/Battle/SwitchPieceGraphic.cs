using Pixelplacement.TweenSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitchPieceGraphic : MonoBehaviour
{
    private Piece thisPiece;
    public TMP_Text T_Name, T_Level;
    public Image I_Graphic;
    public Slider S_Health;

    public void Create(Piece piece)
    {
        T_Name.text = piece.Name;
        T_Level.text = "lvl " + piece.Level;
        S_Health.maxValue = piece.MaxHealth;
        S_Health.value = piece.Health;

        I_Graphic.sprite = Resources.Load<Sprite>($"Icons/{piece.PieceType}/{piece.Variant}");
        I_Graphic.Fit(200);

        GetComponent<Button>().onClick.AddListener(() =>
        {
            Ref.BattleUI.ToggleSwitchPiece(false);
            Ref.BattleManager.PrepareSwitchPiece(piece, ChessManager.Side);
        });
    }
}
