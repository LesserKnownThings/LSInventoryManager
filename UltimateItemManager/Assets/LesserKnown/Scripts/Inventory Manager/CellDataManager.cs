using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CellDataManager : MonoBehaviour
{
    public CellData cellData;
    public Image cellItemIcon;
    
    [HideInInspector]
    public RectTransform rect;


    public void Setup(CellData cellData)
    {
        this.cellData = cellData;
        cellItemIcon.sprite = cellData.defaultIcon;
        this.cellData.iconUpdate += ChangeIcon;
    }

    private void Start()
    {
        if(cellData.iconUpdate == null)
        {
            cellData.iconUpdate += ChangeIcon;
        }
        

        if(cellData.isTransitCell)
        {
            gameObject.SetActive(false);
        }

        Debug.Log(cellData.defaultIcon);
    }


    private void Update()
    {
        if(cellData.cellType == CellType.TransitionCell && gameObject.activeSelf)
        {
            rect.position = Input.mousePosition;
        }
    }



    public void ChangeIcon()
    {
        if (cellData.GetSprite() == null)
        {
            cellItemIcon.sprite = cellData.defaultIcon;
        }
        else
        {
            cellItemIcon.sprite = cellData.GetSprite();
        }
    }
   
}
