using System.Collections;
using UnityEngine;
using System;

[System.Serializable]
public class CellData
{
    public DynamicData itemData;
    public int index { get; private set; }

    [SerializeField]
    public Sprite defaultIcon;

    public CellType cellType;

    public delegate void IconUpdateDelegate();
    public IconUpdateDelegate iconUpdate;

    public bool isTransitCell
    {
        get { return cellType == CellType.TransitionCell; }
    }

    public bool isEmpty
    {
        get { return itemData == null; }
    }

    public CellData(int index, DynamicData itemData, Sprite defaultIcon, CellType cellType)
    {
        this.index = index;
        this.itemData = itemData;
        this.defaultIcon = defaultIcon;
        this.cellType = cellType;
    }

    public Sprite GetSprite()
    {
        if(itemData == null)
        {
            return null;
        }
        return itemData.itemIcon;
    }

    public void AssignItem(DynamicData data)
    {
        itemData = data;

        iconUpdate();
    }

    public void ClearCell()
    {
        itemData = null;
    }

    public void ReturnTransitState()
    {
        if(cellType == CellType.TransitionCell)
        {
            itemData = null;
            iconUpdate();
        }
    }

    public void SwapCell(ref CellData otherCell)
    {
        CellData tempTransition = new CellData(index, itemData, defaultIcon, cellType);
        tempTransition.iconUpdate = iconUpdate;

        AssignItem(otherCell.itemData);
        otherCell.AssignItem(tempTransition.itemData);

    }
      
    public void ExchangeGoods(ref CellData currentCell, ref CellData otherCell)
    {
        otherCell.AssignItem(currentCell.itemData);
        SwapCell(ref currentCell);
    }
    
}

public enum CellType
{
    ItemCell,
    TransitionCell
}
