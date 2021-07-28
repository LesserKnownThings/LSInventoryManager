using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameInventoryManager : MonoBehaviour
{
    public InventoryDataManager dataManager = new InventoryDataManager();
    public GameObject inventoryUI;
    public Settings settings;

    private bool uiOpen;
    private bool isInTransition;

    private CellDataManager currentCell;
    private CellDataManager previousCell;

    private void Start()
    {
        inventoryUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(settings.uiOpenInput))
        {
            UIInput();
        }

        if (Input.GetMouseButtonDown(0))
        {

            if(currentCell != null)
            {
                previousCell = currentCell;
            }

            currentCell = RaycastManager.Instance.DetectCell();
            
            if(currentCell != null)
            {
                if(!isInTransition)
                {
                    if (!currentCell.cellData.isEmpty)
                    {
                        StartTransition(currentCell);
                    }
                }
                else
                {
                    EndTransition(currentCell, previousCell);
                }
                
            }

        }

    }

    private void StartTransition(CellDataManager selectedCell)
    {
        isInTransition = true;
        dataManager.transitionCell.cellData.SwapCell(ref selectedCell.cellData);

        dataManager.transitionCell.gameObject.SetActive(true);
    }

    private void EndTransition(CellDataManager currentCell, CellDataManager previousCell)
    {
        isInTransition = false;

        if(currentCell.cellData.isEmpty)
        {
            dataManager.transitionCell.cellData.SwapCell(ref currentCell.cellData);
        }else
        {
            dataManager.transitionCell.cellData.ExchangeGoods(ref currentCell.cellData, ref previousCell.cellData);
        }

        dataManager.transitionCell.cellData.ReturnTransitState();
        dataManager.transitionCell.gameObject.SetActive(false);  
    }


    public void UIInput()
    {
        uiOpen = !uiOpen;
        inventoryUI.SetActive(uiOpen);
    }
}
