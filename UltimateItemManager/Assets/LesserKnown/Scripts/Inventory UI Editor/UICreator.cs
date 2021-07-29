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

        GameObject eventSystem = new GameObject("Event System");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
        eventSystem.transform.SetParent(inventoryCanvas.transform);
        
        CreateUIPreview(inventoryCanvas, ref settings, ref inGameManager);
        CreateTransitionCell(inventoryCanvas.transform, ref settings, ref inGameManager);
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

        GameObject itemIcon = new GameObject("Item Icon");
        itemIcon.transform.SetParent(slot.transform);
        RectTransform rect = itemIcon.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;

        rect.offsetMin = new Vector2(10, 10);
        rect.offsetMax = new Vector2(-10, -10);

        Image icon = itemIcon.AddComponent<Image>();
        icon.sprite = settings.defaultItemSprite;

        cellData.cellItemIcon = icon;
        cellData.rect = slot.GetComponent<RectTransform>();
        cellData.Setup(new CellData(index, null, settings.defaultItemSprite, cellType));

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




