using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewPiece : MonoBehaviour
{
    [Header("Logistics")]
    public Camera Basic;
    public Camera Piece;
    public GameObject Tab_Basic, Tab_Piece;
    private EntityData thisEntity;
    public bool State;

    [Header("Info")]
    public TMP_Text T_Name;
    public TMP_Text T_Level, T_Type;
    public Slider S_Health;
    public Slider S_Exp;
    public Slider S_Attack, S_Defence, S_Speed, S_Luck;

    public MoveUI OriginalMoveUI;   
    public GameObject MovesParent;
    public List<MoveUI> CurrentMoves;

    [Header("Models")]
    public List<GameObject> Pieces;
    public GameObject CurrentPiece;
    public Vector3 PodiumPos;

    [Header("Rotation")]
    public int Direction;

    [Header("Change Name")]
    public GameObject Tab_ChangeName;
    public TMP_InputField In_Name;

    private void FixedUpdate()
    {
        if (!State) return;

        if(Direction != 0)
        {
            CurrentPiece.transform.Rotate(0, 0, Direction);
        }
    }

    public void SetDirection(int dir) => Direction = dir;

    public void OpenViewPiece(EntityData e)
    {
        Movement.IsPaused = true;
        thisEntity = e;
        Basic.gameObject.SetActive(false);
        Tab_Basic.gameObject.SetActive(false);

        Piece.gameObject.SetActive(true);
        Tab_Piece.gameObject.SetActive(true);

        T_Name.text = e.Name;
        T_Type.text = e.PieceType.ToString();
        T_Level.text = "lvl " + e.Level;

        if(CurrentPiece != null) 
            Destroy(CurrentPiece);

        CurrentPiece = Instantiate(Pieces[(int)e.PieceType], transform);
        CurrentPiece.transform.localPosition = PodiumPos;
        CurrentPiece.GetComponent<MeshRenderer>().material = Resources.Load<Material>($"Materials/{thisEntity.Variant}");
        CurrentPiece.SetActive(true);
        State = true;

        foreach(var m in CurrentMoves)
        {
            Destroy(m.gameObject);
        }
        CurrentMoves.Clear();

        for(int i = 0; i < e.Moves.Count; i++)
        {
            var m = e.Moves[i];
            var newMoveUI = Instantiate(OriginalMoveUI, MovesParent.transform);
            CurrentMoves.Add(newMoveUI);
            newMoveUI.gameObject.SetActive(true);
            newMoveUI.Create(m, Move.MoveIndToLvlRequired(i) > e.Level, Move.MoveIndToLvlRequired(i)) ;           
        }

        S_Health.maxValue = e.MaxHealth;
        S_Health.value = e.Health;
        S_Health.fillRect.GetComponent<Image>().color = S_Health.value == 0 ? Color.red : Color.green;

        var expBounds = e.GetExpTresholdBounds();
        S_Exp.minValue = expBounds.Item1;
        S_Exp.maxValue = expBounds.Item2;
        S_Exp.value = e.Exp;

        S_Attack.maxValue = 10;
        S_Defence.maxValue = 1000;
        S_Speed.maxValue = 10;
        S_Luck.maxValue = 10;

        S_Attack.value = e.Attack;
        S_Defence.value = e.MaxHealth;
        S_Speed.value = e.Speed;
        S_Luck.value = e.Luck;
    }

    public void CloseViewPiece()
    {
        Movement.IsPaused = false;
        Basic.gameObject.SetActive(true);
        Tab_Basic.gameObject.SetActive(true);

        Piece.gameObject.SetActive(false);
        Tab_Piece.gameObject.SetActive(false);
        State = false;
    }

    public void OpenChangeName()
    {
        In_Name.text = thisEntity.Name;
        Tab_ChangeName.SetActive(true);
    }

    public void CloseChangeName()
    {
        thisEntity.Name = In_Name.text;
        Tab_ChangeName.SetActive(false);
        T_Name.text = thisEntity.Name;
        FindAnyObjectByType<PlayerBehaviour>().SaveManager.SaveGame();
        FindObjectOfType<LayoutEdit>().RefreshListPiecesUI();
        
    }
}

