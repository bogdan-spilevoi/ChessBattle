using Newtonsoft.Json;
using Pixelplacement;
using Pixelplacement.TweenSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class PlayerBehaviour : MonoBehaviour
{
    public SaveManager SaveManager;
    public Camera Camera;
    public PieceFoundData pieceFoundData;
    public List<EntityData> PiecesInventory = new();
    [HideInInspector]
    public Trainer TrainerInRange;
    public UI UI;
    [HideInInspector]
    public LayoutEdit LayoutEdit;

    public Action<EntityData> OnGetPiece;
    public BoxBehaviour BoxBehaviour;

    private Vector3 InitialCameraPos;
    private Quaternion InitialCameraRotation;

    private void Start()
    {
        InitialCameraPos = Camera.transform.localPosition;
        InitialCameraRotation = Camera.transform.rotation;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !Movement.IsPaused)
        {
            SaveManager.SaveGame();
        }

        if (TrainerInRange != null)
        {
            UI.ShowBattleTrainerButton(TrainerInRange.Name);
            if(Input.GetKeyDown(KeyCode.Space) && !Movement.IsPaused)
            {
                SpeakWithNpc(TrainerInRange);
                GameRef.TrainerSpeak.Create(TrainerInRange,
                    () =>
                    {
                        PlayerPrefs.SetString("trainer", JsonConvert.SerializeObject(new TrainerData(TrainerInRange), Formatting.Indented));
                        SceneManager.LoadScene("Chess");
                    });               
            }            
        }
        else
        {
            UI.HideBattleTrainerBUtton();
        }
    }

    public void ChangePiecePosition(int oldPos, int newPos)
    {
        var found = PiecesInventory.Find(e => e.Position == oldPos);
        found.Position = newPos;
    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.TryGetComponent(out Trainer t))
        {
            TrainerInRange = t;
        }
        if(other.CompareTag("test"))
        {
            BoxBehaviour.PrepareBox();
        }
    }

    public EntityData GetPiece(EntityData e)
    {
        string p = e.Variant + "/" + e.PieceType;
        if(!pieceFoundData.PiecesFound.Contains(p))
            pieceFoundData.PiecesFound.Add(p);

        PiecesInventory.Add(e);
        SaveManager.SaveGame();
        LayoutEdit.RefreshListPiecesUI();
        OnGetPiece?.Invoke(e);
        return e;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Trainer t))
        {
            TrainerInRange = null;
        }
    }

    public void ChangePlayerPos(Vector3 pos)
    {
        GetComponent<CharacterController>().enabled = false;
        transform.position = pos;
        GetComponent<CharacterController>().enabled = true;
    }

    public void SpeakWithNpc(Trainer trainer)
    {
        var camerPos = (transform.position + trainer.transform.position) / 2 + new Vector3(0, 4, -5);
        Quaternion targetRotation = InitialCameraRotation * Quaternion.Euler(-15f, 0f, 0f);

        Tween.Position(Camera.transform, camerPos, 0.5f, 0, Tween.EaseOut);
        Tween.Rotation(Camera.transform, targetRotation, 0.5f, 0, Tween.EaseOut);
    }

    public void ResetCamera()
    {
        Tween.LocalPosition(Camera.transform, InitialCameraPos, 0.5f, 0, Tween.EaseOut);
        Tween.Rotation(Camera.transform, InitialCameraRotation, 0.5f, 0, Tween.EaseOut);
    }
}
