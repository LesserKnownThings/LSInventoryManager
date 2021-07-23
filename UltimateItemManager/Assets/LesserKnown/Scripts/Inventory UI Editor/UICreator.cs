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
    private static GameObject canvasParent;

    public static void CreatePreview(Settings settings)
    {
        GameObject existingCanvas = GameObject.Find("ItemManager");

        if (existingCanvas != null)
        {            
            UnityEngine.Object.DestroyImmediate(existingCanvas);
        }

        canvasParent = CreateMainCanvas();
        CreateUIPreview(canvasParent, settings);
    }

    private static GameObject CreateMainCanvas()
    {
        GameObject clone =  new GameObject("ItemManager");
        ContentSizeFitter sizeFitter = clone.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
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

        RectTransform rect = preview.GetComponent<RectTransform>();

        if(rect == null)
        {
            rect = preview.AddComponent<RectTransform>();
        }

        rect.localPosition = Vector2.zero;


        Image background = preview.AddComponent<Image>();

        GridLayoutGroup layout = preview.AddComponent<GridLayoutGroup>();
        layout.padding.left = settings.leftPadding;
        layout.padding.right = settings.rightPadding;
        layout.padding.top = settings.topPadding;
        layout.padding.bottom = settings.bottomPadding;
        layout.spacing = new Vector2(settings.horizontalSpacing, settings.verticalSpacing);
        layout.cellSize = new Vector2(settings.horizontalCellSize, settings.verticalCellSize);
        background.color = Color.gray;
    }

}







