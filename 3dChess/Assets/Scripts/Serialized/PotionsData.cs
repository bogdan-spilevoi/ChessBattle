using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PotionData
{
    public string Name;
    public int Position;

    public PotionData(string name, int position)
    {
        Name = name;
        Position = position;
    }

    public PotionData Copy()
    {
        return new PotionData(Name, Position);
    }

    public override string ToString()
    {
        return Name + "_" + Position;
    }
}
