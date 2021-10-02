using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AMath
{
    public static Vector3[] AddVector(Vector3[] array, Vector3 vectorToAdd)
    {
        Vector3[] newArray = new Vector3[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            newArray[i] = array[i] + vectorToAdd;
        }
        return newArray;
    }
}
