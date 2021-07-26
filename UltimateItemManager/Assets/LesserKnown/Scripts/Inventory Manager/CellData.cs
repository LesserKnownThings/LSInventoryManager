using System.Collections;
using UnityEngine;
using System;

[System.Serializable]
public class CellData
{
    public DynamicData itemData;
    public int index { get; private set; }
    
    public CellData(int index, DynamicData itemData)
    {
        this.index = index;
        this.itemData = itemData;
    }

    
}
