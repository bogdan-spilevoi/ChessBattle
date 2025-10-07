using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LayoutEdit : MonoBehaviour
{
    public PlayerBehaviour player;
    public GameObject LayoutParent;
    public GameObject ListPieceParent;

    public List<Image> Squares = new();
    public List<PieceGraphic> PieceGraphics = new();
    public List<ListPieceUI> ListPiecesUI = new();

    public Image OrgSquare;
    public PieceGraphic OrgPieceGraphic;
    public ListPieceUI OrgListPiece;

    public TMP_Text T_Limit;
    public TMP_Text T_Warning;

    public GameObject RemovePieceGraphicHelper;
    public TMP_Dropdown D_SortMethod;

    public int Limit = 8;

    public Color C1, C2;

    private void Start()
    {
        player.LayoutEdit = this;

        
        PrepareSort();

        GenerateSquares();       
        Invoke(nameof(PlacePieces), 0.5f);       
    }

    public void PrepareSort()
    {      
        D_SortMethod.value = PlayerPrefs.GetInt("sortMethod");
        D_SortMethod.RefreshShownValue();
        D_SortMethod.onValueChanged.AddListener(OnSortMethodChange);
    }

    public void OnSortMethodChange(int option)
    {
      
        PlayerPrefs.SetInt("sortMethod", option);
        RefreshListPiecesUI(option);
    }

    public void GenerateSquares()
    {
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                var newSquare = Instantiate(OrgSquare, LayoutParent.transform);
                Squares.Add(newSquare);
                newSquare.gameObject.SetActive(true);
                newSquare.color = (i + j) % 2 == 1 ? C1 : C2;
            }
        }
    }

    public int PiecesOnBoard { get { return PieceGraphics.Where(p => p.position != -1).Count(); } }

    public void PlacePieces()
    {
        foreach(var p in player.PiecesInventory)
        {
            var newListPiece = Instantiate(OrgListPiece, ListPieceParent.transform);
            newListPiece.gameObject.SetActive(true);
            newListPiece.Create(p);
            ListPiecesUI.Add(newListPiece);

            if (p.Position == -1) continue;
            if(p.Health <= 0)
            {
                p.Position = -1;
                p.Health = 0;
                continue;
            }

            var newPieceGraphic = Instantiate(OrgPieceGraphic, LayoutParent.transform);
            newPieceGraphic.gameObject.SetActive(true);
            newPieceGraphic.Create(p, p.Position);
            newPieceGraphic.LayoutEdit = this;

            newPieceGraphic.GetComponent<RectTransform>().localPosition = Squares[p.Position].GetComponent<RectTransform>().localPosition;            
            PieceGraphics.Add(newPieceGraphic);
        }
        RefreshListPiecesUI();
        UpdateLimit();
        player.SaveManager.SaveGame();
    }

    public void RefreshListPiecesUI(int sortMethod = -1)
    {
        if (sortMethod == -1)
            sortMethod = PlayerPrefs.GetInt("sortMethod");

        foreach (var p in ListPiecesUI)
        {
            Destroy(p.gameObject);
        }
        ListPiecesUI.Clear();

        IComparer<EntityData> comparer = sortMethod switch
        {
            0 => Comparer<EntityData>.Create((a, b) => b.Exp.CompareTo(a.Exp)),
            1 => Comparer<EntityData>.Create((a, b) => (a.MaxHealth - a.Health).CompareTo(b.MaxHealth - b.Health)),
            2 => Comparer<EntityData>.Create((a, b) => b.PieceType.CompareTo(a.PieceType)),
            3 => Comparer<EntityData>.Create((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal)),
            _ => Comparer<EntityData>.Create((a, b) => b.Exp.CompareTo(a.Exp)),
        };

        foreach (var p in player.PiecesInventory.OrderBy(kvp => kvp, comparer))
        {
            var newListPiece = Instantiate(OrgListPiece, ListPieceParent.transform);
            newListPiece.gameObject.SetActive(true);
            newListPiece.Create(p);
            ListPiecesUI.Add(newListPiece);
        }
        foreach(var p in PieceGraphics)
        {
            p.Create(p.thisEntity, p.position);
        }
    }

    public void UpdateLimit()
    {
        T_Limit.text = PiecesOnBoard + "/" + Limit;
    }

    public void RemovePieceGraphic(PieceGraphic pg)
    {
        PieceGraphics.Remove(pg);
        Destroy(pg.gameObject);
    }

    public PieceGraphic CreatePieceGraphic(Vector3 pos, EntityData p)
    {
        var newPieceGraphic = Instantiate(OrgPieceGraphic, LayoutParent.transform);
        newPieceGraphic.gameObject.SetActive(true);
        newPieceGraphic.Create(p, p.Position);
        newPieceGraphic.LayoutEdit = this;
        newPieceGraphic.GetComponent<RectTransform>().position = pos;       
        PieceGraphics.Add(newPieceGraphic);
        return newPieceGraphic;
    }

    public int GetTileIndexFromPosition(Vector3 pos)
    {
        if (IsToTheRightOfLayout(pos)) return -2;

        for (int i = 0; i < 4 * 8; i++)
        {
            if (IsInsideSquare(pos, Squares[i].GetComponent<RectTransform>().localPosition, 80))
                return i;
        }
        return -1;
    }

    bool IsInsideSquare(Vector2 point, Vector2 center, float sideLength)
    {
        float halfSide = sideLength / 2f;

        return
            point.x >= center.x - halfSide &&
            point.x <= center.x + halfSide &&
            point.y >= center.y - halfSide &&
            point.y <= center.y + halfSide;
    }

    bool IsToTheRightOfLayout(Vector2 point)
    {
        return point.x > LayoutParent.GetComponent<RectTransform>().sizeDelta.x / 2;
    }

    public void UpdateAllListPieces()
    {
        ListPiecesUI.ForEach(p => { p.UpdateOutline(); });
    }

    public void CloseTab(CanvasGroup Tab_Layout)
    {
        if(PieceGraphics.Where(p => p.thisEntity.PieceType == EntityData.Type.King).Count() == 0)
        {
            ShowWarning("Loadout has no Kings!");
            return;
        }
        if (PieceGraphics.Where(p => p.thisEntity.PieceType == EntityData.Type.King).Count() > 1)
        {
            ShowWarning("Loadout has more than one King!");
            return;
        }

        Tab_Layout.alpha = 0;
        Tab_Layout.interactable = false;
        Tab_Layout.blocksRaycasts = false;
        player.SaveManager.SaveGame();
    }


    private Coroutine warningCor;
    public void ShowWarning(string text)
    {
        if(warningCor != null)
            StopCoroutine(warningCor);
        warningCor = StartCoroutine(ShowWarningCor(text, 2));
    }

    private IEnumerator ShowWarningCor(string text, float time)
    {
        T_Warning.text = text;
        yield return new WaitForSecondsRealtime(time);
        T_Warning.text = "";
    }
}
