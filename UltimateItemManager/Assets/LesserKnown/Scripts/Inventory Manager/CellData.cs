using System.Collections;
using UnityEngine;
using System;

public class CellData : MonoBehaviour
{
    public int index { get; private set; }
    public object itemData;
    public Type dataType;

    public CellData(int index, Type dataType)
    {
        this.index = index;
        this.dataType = dataType;
    }

    
}
