#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEditor.Events;
using UnityEngine.Events;

public static class UICreator
{
    private const string DESTROY_WINDOW_TEXT = "Are you sure you want to destroy the item? The item will be gone forever!";

    private static GameObject InventoryCanvas;
    public static GameObject inventoryCanvas
    {
        get
        {
            if(InventoryCanvas == null)
            {
                InventoryCanvas = GameObject.Find("ItemManager");
            }
            return InventoryCanvas;
        }

        set
        {
            InventoryCanvas = value;
        }
    }

    public static List<CellDataManager> cells = new List<CellDataManager>();

    public static void CreatePreview(ref Settings settings, Transform parent)
    {
        cells.Clear();

        if (InventoryCanvas != null)
        {            
            UnityEngine.Object.DestroyImmediate(InventoryCanvas);
        }

        inventoryCanvas = CreateMainCanvas();
        inventoryCanvas.transform.SetParent(parent);
        InGameInventoryManager inGameManager = inventoryCanvas.AddComponent<InGameInventoryManager>();
        inGameManager.settings = settings;

       

        if (settings.addEventSystem)
        {
            GameObject eventSystem = new GameObject("Event System");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            eventSystem.transform.SetParent(inventoryCanvas.transform);
        }

        CreateUIPreview(inventoryCanvas, ref settings, ref inGameManager);
        CreateTransitionCell(inventoryCanvas.transform, ref settings, ref inGameManager);

        CreateDestroyWindow(inventoryCanvas.transform, ref inGameManager, ref settings);
        CreateCellOptionsWindow(inventoryCanvas.transform, ref inGameManager, ref settings);
    }    

    private static void CreateCellOptionsWindow(Transform parent, ref InGameInventoryManager inGameManager, ref Settings settings)
    {
        GameObject cellOptionsWindow = new GameObject("Cell Options Window");
        cellOptionsWindow.transform.SetParent(parent);
        inGameManager.cellOptionsWindow = cellOptionsWindow;
        RectTransform cellOptionsWindowRect = cellOptionsWindow.AddComponent<RectTransform>();
        cellOptionsWindowRect.pivot = new Vector2(0,1);
        inGameManager.cellOptionsRect = cellOptionsWindowRect;
        

        Image cellOptionsWindowBG = cellOptionsWindow.AddComponent<Image>();
        cellOptionsWindowBG.sprite = settings.cellOptionsSprite;

        GridLayoutGroup cellOptionsGrid = cellOptionsWindow.AddComponent<GridLayoutGroup>();
        cellOptionsGrid.padding.left = 5;
        cellOptionsGrid.padding.right = 5;
        cellOptionsGrid.padding.top = 5;
        cellOptionsGrid.padding.bottom = 5;
        cellOptionsGrid.spacing = new Vector2(0, 3);
        cellOptionsGrid.cellSize = new Vector2(100, 25);
        cellOptionsGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        cellOptionsGrid.constraintCount = 1;

        ContentSizeFitter cellOptionsSizeFitter = cellOptionsWindow.AddComponent<ContentSizeFitter>();
        cellOptionsSizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
        cellOptionsSizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;

        inGameManager.useCellButton = CreateButtonInLayout("Use Cell", "use", cellOptionsWindow.transform);
        inGameManager.useCellButton.GetComponent<Image>().sprite = settings.cellOptionsButtonsSprite;

        inGameManager.splitCellButton = CreateButtonInLayout("Split Cell", "split", cellOptionsWindow.transform);
        inGameManager.splitCellButton.GetComponent<Image>().sprite = settings.cellOptionsButtonsSprite;

        inGameManager.destroyCellButton = CreateButtonInLayout("Destroy Cell", "destroy", cellOptionsWindow.transform);
        inGameManager.destroyCellButton.GetComponent<Image>().sprite = settings.cellOptionsButtonsSprite;

        inGameManager.cancelButton = CreateButtonInLayout("Cancel Cell", "cancel", cellOptionsWindow.transform);
        inGameManager.cancelButton.GetComponent<Image>().sprite = settings.cellOptionsButtonsSprite;
    }

    private static Button CreateButtonInLayout(string buttonName, string buttonData, Transform parent)
    {
        GameObject buttonGo = new GameObject(buttonName, typeof(Button));
        buttonGo.transform.SetParent(parent);
        Image buttonBg = buttonGo.AddComponent<Image>();
        buttonBg.color = Color.white;

        GameObject buttonTextGo = new GameObject("Button Text", typeof(TextMeshProUGUI));
        buttonTextGo.transform.SetParent(buttonGo.transform);
        TextMeshProUGUI buttonText = buttonTextGo.GetComponent<TextMeshProUGUI>();
        buttonText.text = buttonData;
        buttonText.color = Color.black;
        buttonText.horizontalAlignment = HorizontalAlignmentOptions.Center;
        buttonText.verticalAlignment = VerticalAlignmentOptions.Middle;
        buttonText.fontSize = 24;

        RectTransform buttonTextRect = buttonTextGo.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;

        return buttonGo.GetComponent<Button>();
    }

    private static void CreateDestroyWindow(Transform parent, ref InGameInventoryManager inGameManager, ref Settings settings)
    {
        GameObject destroyWindow = new GameObject("Destroy Window");
        inGameManager.destroyWindow = destroyWindow;
        destroyWindow.transform.SetParent(parent);
        Image destroyWindowBG = destroyWindow.AddComponent<Image>();
        destroyWindowBG.color = new Color(0, 0, 0, .5f);
        destroyWindowBG.sprite = settings.destroyWindowBGSprite;
        RectTransform destroyWindowRect = destroyWindow.GetComponent<RectTransform>();
        destroyWindowRect.sizeDelta = new Vector2(600,200);
        destroyWindowRect.localPosition = Vector2.zero;

        VerticalLayoutGroup destroyWindowLayout = destroyWindow.AddComponent<VerticalLayoutGroup>();
        destroyWindowLayout.padding.left = 20;
        destroyWindowLayout.padding.right = 20;
        destroyWindowLayout.childControlWidth = true;
        destroyWindowLayout.childControlHeight = true;

        GameObject destroyWindowTextHolder = new GameObject("Text");
        destroyWindowTextHolder.transform.SetParent(destroyWindow.transform);

        TextMeshProUGUI destroyWindowText = destroyWindowTextHolder.AddComponent<TextMeshProUGUI>();
        destroyWindowText.text = DESTROY_WINDOW_TEXT;
        destroyWindowText.color = Color.yellow;
        destroyWindowText.fontSize = 32;

        RectTransform destroyWindowTextHolderRect = destroyWindowTextHolder.GetComponent<RectTransform>();
        destroyWindowTextHolderRect.anchorMin = new Vector2(0, 1);
        destroyWindowTextHolderRect.anchorMax = new Vector2(1, 1);
        destroyWindowTextHolderRect.pivot = new Vector2(.5f, 1);       
        destroyWindowTextHolderRect.sizeDelta = new Vector2(0, 100);
       
        



        GameObject buttonHolder = new GameObject("Button Holder");
        buttonHolder.transform.SetParent(destroyWindow.transform);

        RectTransform buttonHolderRect = buttonHolder.AddComponent<RectTransform>();
        buttonHolderRect.anchorMin = Vector2.zero;
        buttonHolderRect.anchorMax = new Vector2(1, 0);
        buttonHolderRect.pivot = new Vector2(.5f, 0);
        buttonHolderRect.sizeDelta = new Vector2(0, 100);
        buttonHolderRect.localPosition = Vector2.zero;
        buttonHolderRect.anchoredPosition = Vector2.zero;
        LayoutElement buttonHolderLayoutElement = buttonHolder.AddComponent<LayoutElement>();
        buttonHolderLayoutElement.preferredHeight = 0;

        HorizontalLayoutGroup buttonHolderLayout = buttonHolder.AddComponent<HorizontalLayoutGroup>();

        buttonHolderLayout.childAlignment = TextAnchor.MiddleCenter;
        buttonHolderLayout.childControlHeight = true;
        buttonHolderLayout.childControlWidth = true;
        buttonHolderLayout.padding.left = 50;
        buttonHolderLayout.padding.right = 50;
        buttonHolderLayout.padding.top = 15;
        buttonHolderLayout.padding.bottom = 15;
        buttonHolderLayout.spacing = 30;

        inGameManager.cancelDestroyButton = CreateButtonInLayout("Cancel Button", "Cancel", buttonHolder.transform);
        inGameManager.cancelDestroyButton.GetComponent<Image>().sprite = settings.destroyWindowButtonsSprite;
        inGameManager.destroyItemButton = CreateButtonInLayout("Yes Button", "Yes", buttonHolder.transform);
        inGameManager.destroyItemButton.GetComponent<Image>().sprite = settings.destroyWindowButtonsSprite;
    }

    private static GameObject CreateMainCanvas()
    {
        GameObject clone =  new GameObject("ItemManager");        
        Canvas cloneCanvas = clone.AddComponent<Canvas>();
        cloneCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        clone.AddComponent<GraphicRaycaster>();
        CanvasScaler mainScaler = clone.AddComponent<CanvasScaler>();
        mainScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        mainScaler.referenceResolution = new Vector2(1920, 1080);
        mainScaler.matchWidthOrHeight = 0.5f;

        return clone;
    }

    private static void CreateTransitionCell(Transform parent, ref Settings settings, ref InGameInventoryManager inGameManager)
    {
        CreateSlot(-1, parent, ref settings, CellType.TransitionCell, ref inGameManager);
    }

    private static void CreateUIPreview(GameObject parent,ref Settings settings, ref InGameInventoryManager inGameManager)
    {
        GameObject preview = new GameObject("Preview");
        inGameManager.inventoryUI = preview;
        preview.transform.SetParent(parent.transform);

        ContentSizeFitter sizeFitter = preview.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        RectTransform rect = preview.GetComponent<RectTransform>();

        if(rect == null)
        {
            rect = preview.AddComponent<RectTransform>();
        }

        rect.localPosition = Vector2.zero;


        Image background = preview.AddComponent<Image>();
        background.sprite = settings.bgSprite;
        background.color = settings.bgColor;

        VerticalLayoutGroup verticalLayout = preview.AddComponent<VerticalLayoutGroup>();
        verticalLayout.childControlHeight = true;
        verticalLayout.childControlWidth = true;

        CreateClosingButton(preview.transform, settings, ref inGameManager);
        CreateSlotHolder(preview.transform, ref settings, ref inGameManager);
    }

    private static void CreateSlotHolder(Transform parent, ref Settings settings, ref InGameInventoryManager inGameManager)
    {
        GameObject preview = new GameObject("Item Holder");
        preview.transform.SetParent(parent);

        GridLayoutGroup layout = preview.AddComponent<GridLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.padding.left = settings.leftPadding;
        layout.padding.right = settings.rightPadding;
        layout.padding.top = settings.topPadding;
        layout.padding.bottom = settings.bottomPadding;
        layout.spacing = new Vector2(settings.horizontalSpacing, settings.verticalSpacing);
        layout.cellSize = new Vector2(settings.horizontalCellSize, settings.verticalCellSize);

        

        for (int i = 0; i < settings.cellNumber; i++)
        {
            CreateSlot(i, preview.transform, ref settings, CellType.ItemCell, ref inGameManager);
        }
    }

    private static void CreateClosingButton(Transform parent, Settings settings, ref InGameInventoryManager inGameManager)
    {
        GameObject topViewHolder = new GameObject("Top View");
        topViewHolder.transform.SetParent(parent);

        HorizontalLayoutGroup topViewLayout = topViewHolder.AddComponent<HorizontalLayoutGroup>();
        topViewLayout.childControlHeight = true;
        topViewLayout.childControlWidth = true;

        GameObject textInfo = new GameObject("Text Info");
        textInfo.transform.SetParent(topViewHolder.transform);
        TextMeshProUGUI textInfoText = textInfo.AddComponent<TextMeshProUGUI>();
        textInfoText.alignment = TextAlignmentOptions.Center;
        textInfoText.alignment = TextAlignmentOptions.CenterGeoAligned;
        textInfoText.fontSize = 50f;
        textInfoText.fontStyle = FontStyles.Bold;
        textInfoText.color = Color.yellow;
        textInfoText.text = "Inventory";
        LayoutElement textInfoLayoutElement = textInfo.AddComponent<LayoutElement>();


        textInfoLayoutElement.preferredWidth = settings.GetInventoryWidth() - 75f;
        

        GameObject closingButton = new GameObject("Close Button");
        LayoutElement closingButtonLayoutElement = closingButton.AddComponent<LayoutElement>();
        closingButtonLayoutElement.preferredHeight = 95f;
        closingButtonLayoutElement.preferredWidth = 95f;
        closingButton.transform.SetParent(topViewHolder.transform);
        RectTransform buttonRect = closingButton.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(75, 75);
        Image buttonImage = closingButton.AddComponent<Image>();
        Button buttonRef = closingButton.AddComponent<Button>();

        UnityAction action = new UnityAction(inGameManager.UIInput);
        UnityEventTools.AddPersistentListener(buttonRef.onClick, action);

        if (settings.closeButtonSprite != null)
        {
            buttonImage.sprite = settings.closeButtonSprite;
        }
    }

    private static void CreateSlot(int index, Transform parent, ref Settings settings, CellType cellType, ref InGameInventoryManager inGameManager)
    {
        GameObject slot = new GameObject($"slot {index}");
        Image slotImage = slot.AddComponent<Image>();
        slotImage.sprite = settings.slotBgSprite;
        slotImage.color = settings.slotBgColor;
        slot.transform.SetParent(parent);
        CellDataManager cellData = slot.AddComponent<CellDataManager>();

        if (cellType == CellType.TransitionCell)
        {
            slot.GetComponent<RectTransform>().sizeDelta = new Vector2(settings.horizontalCellSize - 10, settings.verticalCellSize - 10);
        }


        GameObject itemIcon = new GameObject("Item Icon");
        itemIcon.transform.SetParent(slot.transform);
        RectTransform rect = itemIcon.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;

       

        rect.offsetMin = new Vector2(10, 10);
        rect.offsetMax = new Vector2(-10, -10);

        Image icon = itemIcon.AddComponent<Image>();
        icon.sprite = settings.defaultItemSprite;

        GameObject stackBG = new GameObject("Stack");
        Image stackImg = stackBG.AddComponent<Image>();
        stackImg.sprite = settings.stackCellSprite;
        RectTransform stackRect = stackBG.GetComponent<RectTransform>();
        stackRect.anchorMin = new Vector2(0, 0);
        stackRect.anchorMax = new Vector2(1, 1);
        stackRect.pivot = new Vector2(1, 0);
        stackBG.transform.SetParent(itemIcon.transform);

        stackRect.offsetMin = new Vector2(settings.stackCellTopLeftOffset.y, settings.stackCellBottomRightOffset.x);
        stackRect.offsetMax = new Vector2(-settings.stackCellBottomRightOffset.y, -settings.stackCellTopLeftOffset.x);

        

        GameObject stackTextGO = new GameObject("Stack Text");
        stackTextGO.transform.SetParent(stackBG.transform);
        TextMeshProUGUI stackText = stackTextGO.AddComponent<TextMeshProUGUI>();
        stackText.text = "1";        
        stackText.enableAutoSizing = true;
        stackText.fontSizeMax = settings.stackTextSize;
        stackText.fontSizeMin = 5;
        stackText.fontStyle = FontStyles.Bold;
        stackText.color = settings.stackSlotTextColor;
        stackText.alignment = TextAlignmentOptions.CenterGeoAligned;

        RectTransform stackTextRect = stackTextGO.GetComponent<RectTransform>();
        stackTextRect.anchorMin = new Vector2(0, 0);
        stackTextRect.anchorMax = new Vector2(1, 1);
        stackTextRect.offsetMin = Vector2.zero;
        stackTextRect.offsetMax = Vector2.zero;

        stackBG.SetActive(false);

        cellData.cellItemIcon = icon;
        cellData.rect = slot.GetComponent<RectTransform>();
        cellData.Setup(new CellData(index, null, settings.defaultItemSprite, cellType));
        cellData.stackAmountText = stackText;
        cellData.stackAmountBG = stackBG;

        if (cellType != CellType.TransitionCell)
        {
            cells.Add(cellData);
            inGameManager.dataManager.cells.Add(cellData);            
        }
        else
        {
            inGameManager.dataManager.transitionCell = cellData;
        }
    }


}


#endif




