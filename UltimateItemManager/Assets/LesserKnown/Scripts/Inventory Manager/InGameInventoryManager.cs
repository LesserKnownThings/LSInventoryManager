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

    [Space(10)]
    public Button destroyItemButton;
    public Button cancelDestroyButton;
    public GameObject destroyWindow;
    private bool destroyWindowOpen = false;

    [Space(10)]
    public GameObject cellOptionsWindow;
    public RectTransform cellOptionsRect;
    public Button useCellButton;
    public Button splitCellButton;
    public Button destroyCellButton;
    public Button cancelButton;
    private bool cellOptionsWindowOpen;
    private bool isDestroyItem;

    private void Start()
    {
        if(!settings.addEventSystem)
        {
            GameObject eventSystem = inventoryUI.transform.Find("Event System").gameObject;

            if(eventSystem != null)
            {
                Destroy(eventSystem);
            }
        }

        inventoryUI.SetActive(false);
        cellOptionsWindow.SetActive(false);
        destroyWindow.SetActive(false);

        destroyItemButton.onClick.AddListener(DestroyItem);
        destroyItemButton.onClick.AddListener(() => DestroyItemFromCell());

        cancelDestroyButton.onClick.AddListener(() => DestroyWindowDisplay(false, false));

        destroyCellButton.onClick.AddListener(() => DestroyWindowDisplay(true, false));
        useCellButton.onClick.AddListener(UseItem);
        cancelButton.onClick.AddListener(CellOptionsWindowDisplay);
        splitCellButton.onClick.AddListener(SplitItem);
    }

    private void Update()
    {
        if (Input.GetKeyDown(settings.uiOpenInput))
        {
            UIInput();
        }

        if(cellOptionsWindowOpen)
        {
            CellOptionsWindowConditions();

            return;
        }

        if(destroyWindowOpen)
        {
            return;
        }

        CellActions();
    }



    #region Destroy Window Funcitons

    /// <summary>
    /// Turns on or of the destroy window display
    /// </summary>
    /// <param name="fromCell">True when the window is called from the cell functions and false when it's called when throwing an item</param>
    /// <param name="displayOnly">True when this function should act only as a UI open/close</param>
    private void DestroyWindowDisplay(bool fromCell, bool displayOnly)
    {
        destroyWindowOpen = !destroyWindowOpen;

        if (!displayOnly)
        {
            if (!fromCell)
            {
                if (destroyWindowOpen)
                {
                    dataManager.transitionCell.gameObject.SetActive(false);
                }
                else
                {
                    EndTransition(previousCell, previousCell);

                }
            }
            else
            {
                isDestroyItem = true;
                CellOptionsWindowDisplay();
            }
        }

        destroyWindow.SetActive(destroyWindowOpen);
    }
    
    private void DestroyItem()
    {
        if(isDestroyItem)
        { return; }

        EndTransition(null, previousCell);
        DestroyWindowDisplay(false, true);
    }

    private void DestroyItemFromCell()
    {
        if(!isDestroyItem)
        {
            return;
        }

        previousCell.cellData.ClearCell();
        previousCell = null;
        DestroyWindowDisplay(false, true);
        isDestroyItem = false;
    }

    #endregion

    #region Cell Options Functions
    /// <summary>
    /// This is the funciton for the cell use 
    /// Depending on your types you will have to change this
    /// </summary>
    private void UseItem()
    {
        switch(previousCell.cellData.itemData.category)
        {
           
        }
    }

    private void SplitItem()
    {
        Debug.LogWarning("Not implemented yet");
    }

    private void CellOptionsWindowConditions()
    {
        if (Input.GetMouseButtonDown(1))
        {
            currentCell = RaycastManager.Instance.DetectCell();

            if (currentCell == null || currentCell.cellData.isEmpty)
            {
                return;
            }
            else
            {
                if (currentCell.cellData != previousCell.cellData)
                {
                    previousCell = currentCell;
                    cellOptionsWindowOpen = false;
                    CellOptionsWindowDisplay();
                }
            }
        }
    }

    private void CellOptionsWindowDisplay()
    {
        cellOptionsWindowOpen = !cellOptionsWindowOpen;

        if (cellOptionsWindowOpen)
        {
            cellOptionsRect.transform.position = Input.mousePosition;

            if (previousCell.cellData.itemData.isStackable && previousCell.cellData.stackAmount > 1)
            {
                splitCellButton.gameObject.SetActive(true);
            }
            else
            {
                splitCellButton.gameObject.SetActive(false);
            }
        }

        cellOptionsWindow.SetActive(cellOptionsWindowOpen);
    }
    #endregion

    private void CellActions()
    {  
        if (Input.GetMouseButtonDown(0))
        {

            if (currentCell != null)
            {
                previousCell = currentCell;
            }

            currentCell = RaycastManager.Instance.DetectCell();

            if (currentCell != null)
            {
                if (!isInTransition)
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
            else
            {
                if (isInTransition && settings.destroyOnThrow)
                {
                    DestroyWindowDisplay(false, false);
                }else if(!settings.destroyOnThrow)
                {
                    Debug.LogWarning("This will throw the item and spawn a prefab");
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (isInTransition)
            {
                EndTransition(currentCell, currentCell);
            }
            else
            {
                previousCell = RaycastManager.Instance.DetectCell();

                if(previousCell == null || previousCell.cellData.isEmpty)
                {
                    return;
                }

                CellOptionsWindowDisplay();
            }
        }
    }

   

    #region Cell Transition
    private void StartTransition(CellDataManager selectedCell)
    {
        isInTransition = true;
        dataManager.transitionCell.cellData.SwapCell(ref selectedCell.cellData);

        dataManager.transitionCell.gameObject.SetActive(true);
    }

    private void EndTransition(CellDataManager currentCell, CellDataManager previousCell)
    {
        isInTransition = false;

        if (currentCell != null)
        {

            if (currentCell.cellData.isEmpty)
            {
                dataManager.transitionCell.cellData.SwapCell(ref currentCell.cellData);
            }
            else
            {
                dataManager.transitionCell.cellData.ExchangeGoods(ref currentCell.cellData, ref previousCell.cellData);
            }
        }
        

        dataManager.transitionCell.cellData.ReturnTransitState();
        dataManager.transitionCell.gameObject.SetActive(false);  
    }

    #endregion


    public void UIInput()
    {
        uiOpen = !uiOpen;
        inventoryUI.SetActive(uiOpen);

        if(!uiOpen)
        {
            CloseAllUI();
        }
    }

    private void CloseAllUI()
    {
        cellOptionsWindow.SetActive(false);
        destroyWindow.SetActive(false);

        cellOptionsWindowOpen = false;
        destroyWindowOpen = false;
    }
}
