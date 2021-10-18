using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NMath
{
    public static int Sign(float value)
    {
        if (value > 0) return 1;
        if (value < 0) return -1;
        return 0;
    }
}
