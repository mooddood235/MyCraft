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
    public static List<Vector3> AddVector(List<Vector3> list, Vector3 vectorToAdd)
    {
        List<Vector3> newList = new List<Vector3>();

        for (int i = 0; i < list.Count; i++)
        {
            newList.Add(list[i] + vectorToAdd);
        }
        return newList;
    }

    public static void MutatorAddVector(List<Vector3> list, Vector3 vectorToAdd)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list.Add(list[i] + vectorToAdd);
        }
    }
}
