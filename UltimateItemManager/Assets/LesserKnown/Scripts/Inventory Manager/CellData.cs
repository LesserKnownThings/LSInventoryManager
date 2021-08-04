using System.Collections;
using UnityEngine;
using System;

[System.Serializable]
public class CellData
{
    public DynamicData itemData;
    public int index { get; private set; }
    public int stackAmount;

    [SerializeField]
    public Sprite defaultIcon;

    public CellType cellType;

    public delegate void IconUpdateDelegate();
    public IconUpdateDelegate dataUpdate;

    public bool isTransitCell
    {
        get { return cellType == CellType.TransitionCell; }
    }

    public bool isEmpty
    {
        get { return itemData == null; }
    }

    public bool HasSameData(DynamicData data)
    {
        return itemData.itemName.Equals(data.itemName);
    }

    public CellData(int index, DynamicData itemData, Sprite defaultIcon, CellType cellType)
    {
        this.index = index;
        this.itemData = itemData;
        this.defaultIcon = defaultIcon;
        this.cellType = cellType;
    }

    public CellData(int index, DynamicData itemData, Sprite defaultIcon, CellType cellType, int stackAmount)
    {
        this.index = index;
        this.itemData = itemData;
        this.defaultIcon = defaultIcon;
        this.cellType = cellType;
        this.stackAmount = stackAmount;
    }

    public Sprite GetSprite()
    {
        if(itemData == null)
        {
            return null;
        }
        return itemData.itemIcon;
    }

    public void AssignItem(DynamicData data, int newStack)
    {
        itemData = data;
        stackAmount = newStack;
        dataUpdate();
    }

    public void ClearCell()
    {
        itemData = null;
        stackAmount = 1;

        dataUpdate();
    }

    public void ReturnTransitState()
    {
        if(cellType == CellType.TransitionCell)
        {
            itemData = null;
            stackAmount = 1;
            dataUpdate();
        }
    }

    public void SwapCell(ref CellData selectedCell)
    {        
        CellData tempTransition = new CellData(index, itemData, defaultIcon, cellType, stackAmount);
        tempTransition.dataUpdate = dataUpdate;

        AssignItem(selectedCell.itemData, selectedCell.stackAmount);
        selectedCell.AssignItem(tempTransition.itemData, tempTransition.stackAmount);
    }
      
    public void ExchangeGoods(ref CellData currentCell, ref CellData previousCell)
    {
        if (HasSameData(currentCell.itemData) && itemData.isStackable)
        {
            int newStackAmount = stackAmount + currentCell.stackAmount;
             
            if(newStackAmount > currentCell.itemData.stackAmount)
            {
                previousCell.AssignItem(currentCell.itemData, currentCell.stackAmount);
                SwapCell(ref currentCell);
            }
            else
            {
                AssignDataToCell(ref currentCell, ref previousCell);
            }
        }
        else
        {
            previousCell.AssignItem(currentCell.itemData, currentCell.stackAmount);
            SwapCell(ref currentCell);
        }
    }

    

    private void AssignDataToCell(ref CellData currentCell, ref CellData previousCell)
    {
        currentCell.stackAmount += stackAmount;

        ClearCell();
        previousCell.ClearCell();
        currentCell.dataUpdate();
        previousCell.dataUpdate();
        dataUpdate();
    }
    
}

public enum CellType
{
    ItemCell,
    TransitionCell
}
