using Pixelplacement;
using Pixelplacement.TweenSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public Slider S_P1, S_P2;
    public Piece Me, Opp;
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
    
    private List<TweenBase> TweenBaseList = new();
    

    public void Create(BattleManager battleManager)
    {
        TweenBaseList.ForEach(t => t.Stop());
        TweenBaseList.Clear();

        BattleManager = battleManager;

        Me = BattleManager.GetActivePlayer(ChessManager.Side);
        Opp = BattleManager.GetActivePlayer(!ChessManager.Side);

        T_Name1.text = Me.Name == "" ? Me.GetType().ToString() : Me.Name;
        T_Name2.text = Opp.Name == "" ? Opp.GetType().ToString() : Opp.Name;

        T_Lvl1.text = "lvl " +  Me.Level.ToString();
        T_Lvl2.text = "lvl " + Opp.Level.ToString();

        S_P1.maxValue = Me.MaxHealth;
        S_P1.value = Me.Health;
        S_P1.fillRect.GetComponent<Image>().color = GameColors.GetColorBySide(ChessManager.Side);

        S_P2.maxValue = Opp.MaxHealth;
        S_P2.value = Opp.Health;
        S_P2.fillRect.GetComponent<Image>().color = GameColors.GetColorBySide(!ChessManager.Side);


        MovesUI.ClearObjects();
        for (int i = 0; i < Me.Moves.Count; i++)
        {
            var m = Me.Moves[i];
            var g = Instantiate(OrgMove, OrgMove.transform.parent);
            g.gameObject.SetActive(true);
            g.Create(m, Move.MoveIndToLvlRequired(i) > Me.Level);
            MovesUI.Add(g);         
        }

        foreach(var potion in Ref.ChessManager.MyData.Potions)
        {
            PotionSlotsUI[potion.Position].Create(potion);
        }
        T_Health1.text = $"{Me.Health}/{Me.MaxHealth}";
        T_Health2.text = $"{Opp.Health}/{Opp.MaxHealth}";
        UpdateUI();
    }

    public void UpdateHealth()
    {
        int h1 = Me.Health, h2 = Opp.Health;

        T_Health1.text = $"{h1}/{Me.MaxHealth}";
        T_Health2.text = $"{h2}/{Opp.MaxHealth}";
        
        var t1 = Tween.Value(S_P1.value, Me.Health, (val) => { S_P1.value = val; }, 0.5f, 0, Tween.EaseInOut, completeCallback: () => { S_P1.value = h1; } );
        var t2 = Tween.Value(S_P2.value, Opp.Health, (val) => { S_P2.value = val; }, 0.5f, 0, Tween.EaseInOut, completeCallback: () => { S_P2.value = h2; } );
        TweenBaseList.Add(t1);
        TweenBaseList.Add(t2);
    }

    public void ToggleOverlay(bool b)
    {
        PlayerUIOverlay.SetActive(b);
    }

    public void UpdateUI()
    {
        bool b = BattleManager.Turn % 2 != (ChessManager.Side ? 0 : 1);
        PlayerUIOverlay.SetActive(b);
        AttackUI.SetActive(b);
        PotionsUI.SetActive(b);
        DialogueUI.SetActive(!b);
        //B_Attack.enabled = Me.AvialableMoves.Any(m => m.Count > 0);
    }

    public void ToggleSwitchPiece(bool b)
    {
        Tab_SwitchPiece.SetActive(b);
        if (!b) return;

        var pieces = Ref.BattleManager.GetTeam(ChessManager.Side).Where(p => !p.Value.Item2).ToList();

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
