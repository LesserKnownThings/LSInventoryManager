using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;


public class CellDataManager : MonoBehaviour
{
    public CellData cellData;
    public Image cellItemIcon;

    public void Setup(CellData cellData)
    {
        this.cellData = cellData;       
    }

}
