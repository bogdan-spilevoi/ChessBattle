using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectListUI : MonoBehaviour
{
    public EffectUI OriginalEffectUI;
    public List<EffectUI> AllEffectsUI;

    public void AddEffect(Effect e)
    {
        var newEffect = Instantiate(OriginalEffectUI, transform);
        AllEffectsUI.Add(newEffect);
        newEffect.gameObject.SetActive(true);
        newEffect.Create(e);
        newEffect.thisList = this;
    }

    public void RemoveEffect(Effect e)
    {
        var elem = AllEffectsUI.Find(ef => ef.thisEffect == e);
        if (elem != null)
        {
            Destroy(elem);
            AllEffectsUI.Remove(elem);
        }
    }

    public void UpdateEffects()
    {
        AllEffectsUI.ForEach(e => 
        { 
            e.UpdateEffectUI(); 
        });
        var toDelete = AllEffectsUI.Where(e => e.thisEffect.rounds <= 0).ToList();
        AllEffectsUI = AllEffectsUI.Where(e => e.thisEffect.rounds > 0).ToList();
        toDelete.ForEach(e => { Destroy(e.gameObject); });
        toDelete.Clear();
    }
}
