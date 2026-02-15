using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainerSpeak : MonoBehaviour
{
    public static bool Active = false;
    [Header("UI")]
    public CanvasGroup TrainerSpeakUI;
    public TMP_Text T_TrainerName, T_Dialogue;
    public Image I_TrainerPic;
    public Button B_Battle, B_Flee;

    [Header("Others")]
    public Trainer thisTrainer;
    public Action OnBattle;

    public void Create(Trainer trainer, Action OnBattle, bool optional = true)
    {
        Active = true;
        Movement.IsPaused = true;
        TrainerSpeakUI.Activate();

        thisTrainer = trainer;
        thisTrainer.Speak();
        this.OnBattle = OnBattle;

        T_TrainerName.text = trainer.Name;
        T_Dialogue.text = trainer.Defeated ? trainer.DefeatedText : trainer.ChallengeText;
        SetupIcon();

        B_Flee.gameObject.SetActive(optional);
        B_Battle.gameObject.SetActive(!trainer.Defeated);

        B_Flee.onClick.RemoveAllListeners();
        B_Battle.onClick.RemoveAllListeners();

        B_Flee.onClick.AddListener(() =>
        {
            TrainerSpeakUI.Deactivate();
            thisTrainer.EndSpeak();
            GameRef.PlayerBehaviour.ResetCamera();
            Movement.IsPaused = false;
            Active = false;
        });
        B_Battle.onClick.AddListener(() => {
            Active = false;
            Movement.IsPaused = false;
            OnBattle();
        });
    }

    private void SetupIcon()
    {
        var initialHeight = I_TrainerPic.GetComponent<RectTransform>().sizeDelta.y;
        var sprt = Resources.Load<Sprite>($"Icons/Trainers/{thisTrainer.Name}");
        I_TrainerPic.sprite = sprt != null ? sprt : null;
        I_TrainerPic.FitSpecific(true, I_TrainerPic.GetComponent<RectTransform>().sizeDelta.x);

        I_TrainerPic.GetComponent<RectTransform>().localPosition = new Vector2(
            I_TrainerPic.GetComponent<RectTransform>().localPosition.x,
            I_TrainerPic.GetComponent<RectTransform>().localPosition.y + (I_TrainerPic.GetComponent<RectTransform>().sizeDelta.y - initialHeight) / 2);
    }
}
