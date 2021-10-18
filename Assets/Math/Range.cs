using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range
{
    public readonly float min;
    public readonly float max;

    public bool Within(float value)
    {
        return value >= min && value <= max;
    }

    public bool Within(Range range)
    {
        return range.min >= min && range.min <= max && range.max >= min && range.max <= max;
    }

    public Range(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
    
    
}
