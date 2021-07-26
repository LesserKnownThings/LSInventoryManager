using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public static class UICreator
{
    public static GameObject inventoryCanvas;
    public static List<CellDataManager> cells = new List<CellDataManager>();

    public static void CreatePreview(Settings settings)
    {
        cells.Clear();

        GameObject existingCanvas = GameObject.Find("ItemManager");

        if (existingCanvas != null)
        {            
            UnityEngine.Object.DestroyImmediate(existingCanvas);
        }

        inventoryCanvas = CreateMainCanvas();
        CreateUIPreview(inventoryCanvas, settings);
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
    private static void CreateUIPreview(GameObject parent, Settings settings)
    {
        GameObject preview = new GameObject("Preview");
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

        GridLayoutGroup layout = preview.AddComponent<GridLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.padding.left = settings.leftPadding;
        layout.padding.right = settings.rightPadding;
        layout.padding.top = settings.topPadding;
        layout.padding.bottom = settings.bottomPadding;
        layout.spacing = new Vector2(settings.horizontalSpacing, settings.verticalSpacing);
        layout.cellSize = new Vector2(settings.horizontalCellSize, settings.verticalCellSize);

        background.color = settings.bgColor;

        for (int i = 0; i < settings.cellNumber; i++)
        {
            CreateSlots(i, preview.transform, ref settings);
        }
    }

    private static void CreateSlots(int index, Transform parent, ref Settings settings)
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

        rect.offsetMin = new Vector2(5, 5);
        rect.offsetMax = new Vector2(-5, -5);

        Image icon = itemIcon.AddComponent<Image>();
        icon.sprite = settings.defaultItemSprite;

        cellData.cellItemIcon = icon;
        cellData.cellData = new CellData(index, null);
        cells.Add(cellData);
    }

}







