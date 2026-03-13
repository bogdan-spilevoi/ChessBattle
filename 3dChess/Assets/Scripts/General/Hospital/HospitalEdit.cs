using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HospitalEdit : MonoBehaviour
{
    public GameObject LayoutParent;
    public GameObject ListPieceParent;

    public PieceGraphicHospital OrgPieceGraphic;
    public List<PieceGraphicHospital> PieceGraphics = new();

    public List<HospitalListPieceUI> ListPiecesUI = new();
    public HospitalListPieceUI OrgListPiece;

    public List<HospitalSlotUI> Slots = new();
    public HospitalSlotUI OrgSlot;
    public int SlotsCount = 6;
    

    public TMP_Dropdown D_SortMethod;

    private void Awake()
    {
        PrepareSort();
    }

    private void Start()
    {
        RefreshListPiecesUI();
        PrepareSlots();
    }

    public void PrepareSlots()
    {
        var pgs = new List<(PieceGraphicHospital, HospitalSlotUI)>();
        for (int i = 0; i < SlotsCount; i++)
        {
            var ind = -i - 2;
            var newSlot = Instantiate(OrgSlot, LayoutParent.transform);
            newSlot.gameObject.SetActive(true);
            newSlot.Create(ind);
            Slots.Add(newSlot);

            if (GameRef.PlayerBehaviour.PiecesInventory.Any(p => p.Position == ind))
            {
                var pg = CreatePieceGraphic(newSlot.transform.position, GameRef.PlayerBehaviour.PiecesInventory.First(p => p.Position == ind));
                newSlot.Create(pg);
                pgs.Add((pg, newSlot));
            }
        }
        this.ActionAfterTime(0.1f, () =>
        {
            foreach (var pg in pgs)
            {
                pg.Item1.transform.SetAsLastSibling();
                pg.Item2.Create(pg.Item1);
            }
        });
          
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


    public PieceGraphicHospital CreatePieceGraphic(Vector3 pos, EntityData p)
    {
        var newPieceGraphic = Instantiate(OrgPieceGraphic, LayoutParent.transform);
        newPieceGraphic.gameObject.SetActive(true);
        newPieceGraphic.Create(p, p.Position);
        newPieceGraphic.GetComponent<RectTransform>().position = pos;
        PieceGraphics.Add(newPieceGraphic);
        return newPieceGraphic;
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

        foreach (var p in GameRef.PlayerBehaviour.PiecesInventory.OrderBy(kvp => kvp, comparer))
        {
            var newListPiece = Instantiate(OrgListPiece, ListPieceParent.transform);
            newListPiece.gameObject.SetActive(true);
            newListPiece.Create(p);
            ListPiecesUI.Add(newListPiece);
        }
        foreach (var p in PieceGraphics)
        {
            p.Create(p.thisEntity, p.position);
        }
    }

    public void RemovePieceGraphic(PieceGraphicHospital pieceGraphic)
    {
        PieceGraphics.Remove(pieceGraphic);
        Destroy(pieceGraphic.gameObject);
    }
    public HospitalListPieceUI GetPieceUIByPieceGraphic(PieceGraphicHospital pgh)
    {
        return ListPiecesUI.FirstOrDefault(p => p.thisEntity == pgh.thisEntity);
    }

    public bool TryPlaceOnSlot(PieceGraphicHospital pieceGraphic, out HospitalSlotUI slot)
    {
        slot = null;
        foreach (var d in Slots)
        {
            if (!d.Occupied &&
                RectTransformUtility.RectangleContainsScreenPoint(
            d.GetComponent<RectTransform>(),
            pieceGraphic.transform.position))
            {
                slot = d;
                return true;
            }
        }
        return false;
    }

    public void RemoveFromSlot(int slot)
    {
        slot =  -(slot + 2);
        Slots[slot].Clear();  
    }
}
