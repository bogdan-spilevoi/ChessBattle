using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Rng32
{
    private uint state;
    public Rng32(uint seed) => state = seed == 0 ? 0xA341316C : seed;

    public uint NextU()
    {
        uint x = state;
        x ^= x << 13;
        x ^= x >> 17;
        x ^= x << 5;
        state = x;
        return x;
    }

    public int RangeInt(int minInclusive, int maxExclusive)
    {
        if (maxExclusive <= minInclusive)
            return minInclusive;
        uint span = (uint)(maxExclusive - minInclusive);
        return (int)(NextU() % span) + minInclusive;
    }

    public bool Percent(int percent0to100)
    {
        int roll = RangeInt(0, 100);
        return roll < percent0to100;
    }
}