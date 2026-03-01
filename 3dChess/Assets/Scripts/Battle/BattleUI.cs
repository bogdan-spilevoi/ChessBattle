using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public Slider S_P1, S_P2;
    public TMP_Text T_Name1, T_Name2, T_Lvl1, T_Lvl2, T_Health1, T_Health2;

    public BattleManager BattleManager;

    public BattleMoveUI OrgMove;
    public List<BattleMoveUI> MovesUI;
    public List<PotionSlotUI> PotionSlotsUI;
    public GameObject PlayerUIOverlay, AttackUI, PotionsUI, DialogueUI;
    public Button B_Attack;

    public GameObject Tab_SwitchPiece;
    public List<SwitchPieceGraphic> SwitchPieces;
    public SwitchPieceGraphic OriginalPieceGraphic;
    

    public void Create(BattleManager battleManager)
    {
        BattleManager = battleManager;

        T_Name1.text = BattleManager.ActivePlayer1.Name == "" ? BattleManager.ActivePlayer1.GetType().ToString() : BattleManager.ActivePlayer1.Name;
        T_Name2.text = BattleManager.ActivePlayer2.Name == "" ? BattleManager.ActivePlayer2.GetType().ToString() : BattleManager.ActivePlayer2.Name;

        T_Lvl1.text = "lvl " +  BattleManager.ActivePlayer1.Level.ToString();
        T_Lvl2.text = "lvl " + BattleManager.ActivePlayer2.Level.ToString();

        S_P1.maxValue = BattleManager.ActivePlayer1.MaxHealth;
        S_P1.value = BattleManager.ActivePlayer1.Health;
        S_P1.fillRect.GetComponent<Image>().color = GameColors.GetColorBySide(ChessManager.Side);

        S_P2.maxValue = BattleManager.ActivePlayer2.MaxHealth;
        S_P2.value = BattleManager.ActivePlayer2.Health;
        S_P2.fillRect.GetComponent<Image>().color = GameColors.GetColorBySide(!ChessManager.Side);


        MovesUI.ClearObjects();
        for (int i = 0; i < BattleManager.ActivePlayer1.Moves.Count; i++)
        {
            var m = BattleManager.ActivePlayer1.Moves[i];
            var g = Instantiate(OrgMove, OrgMove.transform.parent);
            g.gameObject.SetActive(true);
            g.Create(m, Move.MoveIndToLvlRequired(i) > BattleManager.ActivePlayer1.Level);
            MovesUI.Add(g);         
        }

        foreach(var potion in Ref.ChessManager.MyData.Potions)
        {
            PotionSlotsUI[potion.Position].Create(potion);
        }
        T_Health1.text = $"{BattleManager.ActivePlayer1.Health}/{BattleManager.ActivePlayer1.MaxHealth}";
        T_Health2.text = $"{BattleManager.ActivePlayer2.Health}/{BattleManager.ActivePlayer2.MaxHealth}";
        UpdateUI();
    }

    public void UpdateHealth()
    {
        int h1 = BattleManager.ActivePlayer1.Health, h2 = BattleManager.ActivePlayer2.Health;
        T_Health2.text = $"{h2}/{BattleManager.ActivePlayer2.MaxHealth}";
        T_Health1.text = $"{h1}/{BattleManager.ActivePlayer1.MaxHealth}";
        Tween.Value(S_P1.value, BattleManager.ActivePlayer1.Health, (val) => { S_P1.value = val; }, 0.5f, 0, Tween.EaseInOut, completeCallback: () => { S_P1.value = h1; } );
        Tween.Value(S_P2.value, BattleManager.ActivePlayer2.Health, (val) => { S_P2.value = val; }, 0.5f, 0, Tween.EaseInOut, completeCallback: () => { S_P2.value = h2; } );
    }

    public void UpdateUI()
    {
        bool b = BattleManager.Turn % 2 != 0;
        PlayerUIOverlay.SetActive(b);
        AttackUI.SetActive(b);
        PotionsUI.SetActive(b);
        DialogueUI.SetActive(!b);
        B_Attack.enabled = BattleManager.ActivePlayer1.AvialableMoves.Any(m => m.Count > 0);
    }

    public void ToggleSwitchPiece(bool b)
    {
        Tab_SwitchPiece.SetActive(b);
        if (!b) return;

        var pieces = Ref.BattleManager.player1Team.Where(p => !p.Value.Item2).ToList();

        SwitchPieces.ClearObjects();
        foreach (var p in pieces)
        {
            var piece = p.Key;
            var spg = Instantiate(OriginalPieceGraphic, OriginalPieceGraphic.transform.parent);
            spg.Create(piece);
            spg.gameObject.SetActive(true);
            
            SwitchPieces.Add(spg);
        }
    }
}
