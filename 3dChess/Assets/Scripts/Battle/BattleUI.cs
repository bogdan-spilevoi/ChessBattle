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
    public TMP_Text T_Name1, T_Name2, T_Lvl1, T_Lvl2;

    public BattleManager BattleManager;

    public BattleMoveUI OrgMove;
    public List<BattleMoveUI> MoveUI;
    public GameObject PlayerUIOverlay, AttackUI, DialogueUI;
    public Button B_Attack;

    public GameObject Tab_SwitchPiece;
    public List<SwitchPieceGraphic> SwitchPieces;
    public SwitchPieceGraphic OriginalPieceGraphic;
    

    public void Create(BattleManager battleManager)
    {
        BattleManager = battleManager;

        T_Name1.text = BattleManager.player1.Name == "" ? BattleManager.player1.GetType().ToString() : BattleManager.player1.Name;
        T_Name2.text = BattleManager.player2.Name == "" ? BattleManager.player2.GetType().ToString() : BattleManager.player2.Name;

        T_Lvl1.text = "lvl " +  BattleManager.player1.Level.ToString();
        T_Lvl2.text = "lvl" + BattleManager.player2.Level.ToString();

        S_P1.maxValue = BattleManager.player1.MaxHealth;
        S_P1.value = BattleManager.player1.Health;

        S_P2.maxValue = BattleManager.player2.MaxHealth;
        S_P2.value = BattleManager.player2.Health;

        foreach (var m in MoveUI)
        {
            Destroy(m.gameObject);
        }
        MoveUI.Clear();

        for(int i = 0; i < BattleManager.player1.Moves.Count; i++)
        {
            var m = BattleManager.player1.Moves[i];
            var g = Instantiate(OrgMove, OrgMove.transform.parent);
            g.gameObject.SetActive(true);
            g.Create(m, Move.MoveIndToLvlRequired(i) > BattleManager.player1.Level);
            MoveUI.Add(g);
            g.GetComponent<Button>().onClick.AddListener(() => { BattleManager.ProcessMove(m, true); });
        }
        UpdateUI();
    }

    public void UpdateHealth()
    {
        print("Animating health bar");
        int h1 = BattleManager.player1.Health, h2 = BattleManager.player2.Health;
        Tween.Value(S_P1.value, BattleManager.player1.Health, (val) => { S_P1.value = val; }, 0.5f, 0, Tween.EaseInOut, completeCallback: () => { S_P1.value = h1; } );
        Tween.Value(S_P2.value, BattleManager.player2.Health, (val) => { S_P2.value = val; }, 0.5f, 0, Tween.EaseInOut, completeCallback: () => { S_P2.value = h2; } );
    }

    public void UpdateUI()
    {
        bool b = BattleManager.Turn % 2 != 0;
        PlayerUIOverlay.SetActive(b);
        AttackUI.SetActive(b);
        DialogueUI.SetActive(!b);
        B_Attack.enabled = BattleManager.player1.AvialableMoves.Any(m => m.Count > 0);
    }

    public void ToggleSwitchPiece(bool b)
    {
        Tab_SwitchPiece.SetActive(b);
        if (!b) return;

        var pieces = Ref.BattleManager.player1Team.Where(p => !p.Value.Item2).ToList();

        foreach(var p in SwitchPieces)
        {
            Destroy(p.gameObject);
        }
        SwitchPieces.Clear();

        foreach (var p in pieces)
        {
            var piece = p.Key;
            var spg = Instantiate(OriginalPieceGraphic, OriginalPieceGraphic.transform.parent);
            spg.Create(piece.Name, piece.Level, piece.Health, piece.MaxHealth, Resources.Load<Sprite>($"Icons/{piece.PieceType}/{piece.Variant}"));
            spg.gameObject.SetActive(true);
            spg.GetComponent<Button>().onClick.AddListener(() =>
            {
                ToggleSwitchPiece(false);
                Ref.BattleManager.SwitchPiece(piece, true);
            });
            SwitchPieces.Add(spg);
        }
    }
}
