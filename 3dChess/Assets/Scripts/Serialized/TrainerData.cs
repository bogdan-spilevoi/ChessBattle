using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerData
{
    public string Name;
    public List<EntityData> Inventory;
    public bool Defeated;

    public TrainerData() { }

    public TrainerData(Trainer t)
    {
        Inventory = t.Inventory;
        Defeated = t.Defeated;
        Name = t.Name;
    }
}
