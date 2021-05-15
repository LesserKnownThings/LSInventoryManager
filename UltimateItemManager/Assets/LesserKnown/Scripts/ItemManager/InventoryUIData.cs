using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class InventoryUIData 
{
    #region Private Variables
    public Sprite inventoryBG;
    public Sprite itemSlotBG;
    public Sprite defaultItemIcon;
    public Sprite closeButton;
    #endregion

    
    [Space(10)]
    public bool updatePreview;
    public bool usePreview;

    [Space(10)]
    [HideInInspector]
    public int width = 2;
    [HideInInspector]
    public int height = 2;
    [HideInInspector]
    public int slotSize = 4;
    [Space(10)]
    [Header("Padding")]
    [HideInInspector]
    public int leftPadding = 0;
    [HideInInspector]
    public int rightPadding = 0;
    [HideInInspector]
    public int topPadding = 0;
    [HideInInspector]
    public int bottomPadding = 0;
    [HideInInspector]
    public int xSpacing = 0;
    [HideInInspector]
    public int ySpacing = 0;


    [HideInInspector]
    public float textTopSpacing = 15f;
    [HideInInspector]
    public Color textColor = Color.black;

    [HideInInspector]
    public GameObject currentUi;

    private InventoryUIData savedData;

    /// <summary>
    /// Default constructor
    /// </summary>
    public void LoadData()
    {
        if (inventoryBG == null)
            inventoryBG = Resources.Load<Sprite>("Resources/Editor/UI/bgSprite");
        if (itemSlotBG == null)
            itemSlotBG = Resources.Load<Sprite>("Resources/Editor/UI/itemSlotBG");
        if (defaultItemIcon == null)
            defaultItemIcon = Resources.Load<Sprite>("Resources/Editor/UI/defaultItemIcon");
        if (closeButton == null)
            closeButton = Resources.Load<Sprite>("Resources/Editor/UI/closeButton");
    }

    public void CreateUI(Transform canvasParent)
    {
        LoadData();

        
        currentUi = GameObject.FindGameObjectWithTag("Preview");

        if (savedData != null && !IsSame(savedData))
        {
            GameObject.DestroyImmediate(currentUi);
            Debug.LogWarning("Recreating UI");
        }

        if (currentUi == null)
        {
            GameObject go = CreateMainCanvas(canvasParent);

            CreateSaveData();

            GameObject mainHolder = CreateMainHolder(go.transform);

            GameObject slotHolder = CreateSlotHolder(mainHolder.transform);

            int gridSize = height * width;

                for(int i =0; i < gridSize; i++)
                {
                    GameObject slot = new GameObject($"Slot {i}");
                    slot.transform.SetParent(slotHolder.transform, true);
                    RectTransform slotRect = slot.AddComponent<RectTransform>();
                    slotRect.sizeDelta = new Vector2(slotSize, slotSize);
                    Image img = slot.AddComponent<Image>();
                    img.sprite = itemSlotBG;
                    //THIS IS TEMP FOR TESTING
                    img.color = Color.black;
                }
        }       

    }

    #region Private API
    private GameObject CreateMainCanvas(Transform canvasParent)
    {
        GameObject go = new GameObject("Inventory UI");
        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler canvasScaler = go.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.matchWidthOrHeight = 0.5f;

        GraphicRaycaster gRaycaster = go.AddComponent<GraphicRaycaster>();
        gRaycaster.ignoreReversedGraphics = true;
        gRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
        go.tag = "Preview";
        go.transform.SetParent(canvasParent, false);
        currentUi = go;

        return go;
    }

    private GameObject CreateMainHolder(Transform parent)
    {
        GameObject go = new GameObject("Main Holder");
        VerticalLayoutGroup vl = go.AddComponent<VerticalLayoutGroup>();
        vl.childControlHeight = true;
        vl.childControlWidth = true;

        go.AddComponent<Image>().sprite = inventoryBG;

        go.transform.SetParent(parent, false);
        RectTransform goRect = go.GetComponent<RectTransform>();

        ContentSizeFitter contentFitter = go.AddComponent<ContentSizeFitter>();
        contentFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        GameObject textHolder = new GameObject("Text Holder");
        textHolder.transform.SetParent(go.transform, false);
        textHolder.AddComponent<RectTransform>();

        LayoutElement textLayout = textHolder.AddComponent<LayoutElement>();
        textLayout.preferredHeight = textTopSpacing;

        TextMeshProUGUI tmProText = textHolder.AddComponent<TextMeshProUGUI>();
        tmProText.enableAutoSizing = true;
        tmProText.fontSizeMax = 15f;
        tmProText.fontSizeMax = 70f;
        tmProText.text = "Inventory";
        tmProText.alignment = TextAlignmentOptions.Center;
        tmProText.color = textColor;
        return go;
    }

    private GameObject CreateSlotHolder(Transform parent)
    {
        GameObject slotHolder = new GameObject("Slot Holder");
        slotHolder.transform.SetParent(parent, false);

        slotHolder.AddComponent<RectTransform>();

        RectTransform holderRect = slotHolder.GetComponent<RectTransform>();
        holderRect.pivot = new Vector2(0.5f, 1f);

        Vector2 rectSize = new Vector2();
        rectSize.x = width * slotSize + xSpacing * 4 + leftPadding + rightPadding;
        rectSize.y = height * slotSize + ySpacing * 4  + topPadding + bottomPadding;
        holderRect.sizeDelta = rectSize;

        GridLayoutGroup layoutGroup = slotHolder.AddComponent<GridLayoutGroup>();
        layoutGroup.padding.left = leftPadding;
        layoutGroup.padding.right = rightPadding;
        layoutGroup.padding.top = topPadding;
        layoutGroup.padding.bottom = bottomPadding;
        layoutGroup.cellSize = new Vector2(slotSize, slotSize);
        layoutGroup.spacing = new Vector2(xSpacing, ySpacing);
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;

        return slotHolder;
    }

    private bool IsSame(InventoryUIData data)
    {
        return
            width == data.width &&
            height == data.height &&
            leftPadding == data.leftPadding &&
            rightPadding == data.rightPadding &&
            topPadding == data.topPadding &&
            bottomPadding == data.bottomPadding &&
            xSpacing == data.xSpacing &&
            ySpacing == data.ySpacing &&
            slotSize == data.slotSize &&
            textTopSpacing == data.textTopSpacing &&
            textColor == data.textColor;
    }

    private void CreateSaveData()
    {     
        savedData = new InventoryUIData();
        savedData.slotSize = slotSize;
        savedData.width = width;
        savedData.height = height;
        savedData.leftPadding = leftPadding;
        savedData.rightPadding = rightPadding;
        savedData.topPadding = topPadding;
        savedData.bottomPadding = bottomPadding;
        savedData.ySpacing = ySpacing;
        savedData.xSpacing = xSpacing;
        savedData.textTopSpacing = textTopSpacing;
        savedData.textColor = textColor;
        
    }
    #endregion

}
