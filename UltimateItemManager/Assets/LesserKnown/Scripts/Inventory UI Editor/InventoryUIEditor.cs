using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
[CustomEditor(typeof(InventoryManager))]
[CanEditMultipleObjects]
public class InventoryUIEditor : Editor
{
    private const string GENERATED_ITEMS_FOLDER = "Generated/Items";
    private const string PREFAB_FOLDER = "Assets/LesserKnown/Resources/Generated/Prefab/";

    private const string CELL_AMOUNT_TOOLTIP = "The amount of cells in the inventory";
    private const string CELL_SIZE_TOOLTIP = "The size of a cell in the invetnory window. X is the width and Y is the height of the cell";
    private const string UPDATE_EDITOR_TOOLTIP = "Turn this on for the UI to update with every change";
    private const string BG_COLOR_TOOLTIP = "The background color of the inventory window";
    private const string SLOT_BG_COLOR_TOOLTIP = "The background color of the cells inside the window";
    private const string SPRITE_BG_TOOLTIP = "The sprite for the inventory window";
    private const string SPRITE_CELL_BG_TOOLTIP = "The sprite used for the cell inside the inventory window";
    private const string SPRITE_DEFAULT_ITEM_TOOLTIP = "The sprite used to render the default (empty slot) items inside the cells";

    private readonly int[] MAX_MIN_PADDING = { 0, 200 };
    private readonly int[] MAX_MIN_CELL_SIZE = { 0, 90 };
    private readonly int[] MAX_MIN_CELL_AMOUNT = { 0, 40 };

    private bool showPadding;
    private bool showSprites;
    private InventoryManager mainScript;
    private Settings localSettings;


    public override void OnInspectorGUI()
    {
        mainScript = (InventoryManager)target;
        GUI.color = Color.yellow;
        EditorGUILayout.LabelField("Settings");
        GUI.color = Color.white;

        mainScript.settings.cellNumber = EditorGUILayout.IntSlider(new GUIContent("Cell amount", CELL_AMOUNT_TOOLTIP), mainScript.settings.cellNumber, MAX_MIN_CELL_AMOUNT[0], MAX_MIN_CELL_AMOUNT[1]);

        Vector2 cellSize = new Vector2(mainScript.settings.horizontalCellSize, mainScript.settings.verticalCellSize);
        cellSize = EditorGUILayout.Vector2Field(new GUIContent("Cell size", CELL_SIZE_TOOLTIP), cellSize);
        mainScript.settings.horizontalCellSize = cellSize.x;
        mainScript.settings.verticalCellSize = cellSize.y;

        Vector2 spacing = new Vector2(mainScript.settings.horizontalSpacing, mainScript.settings.verticalSpacing);
        spacing = EditorGUILayout.Vector2Field("Cell Spacing", spacing);
        mainScript.settings.horizontalSpacing = spacing.x;
        mainScript.settings.verticalSpacing = spacing.y;

        mainScript.onValueChange = EditorGUILayout.Toggle(new GUIContent("Update editor", UPDATE_EDITOR_TOOLTIP), mainScript.onValueChange);

        if (mainScript.onValueChange && mainScript.settings != localSettings)
        {
            UICreator.CreatePreview(mainScript.settings);
            localSettings = mainScript.settings;
        }

        mainScript.settings.bgColor = EditorGUILayout.ColorField(new GUIContent("BG color", BG_COLOR_TOOLTIP), mainScript.settings.bgColor);
        mainScript.settings.slotBgColor = EditorGUILayout.ColorField(new GUIContent("Slot BG color", SLOT_BG_COLOR_TOOLTIP), mainScript.settings.slotBgColor);

        DisplaySprites();
        DisplayPadding();
        DisplayButtons();        
    }

    private void DisplaySprites()
    {
        showSprites = EditorGUILayout.Foldout(showSprites, "Sprites Settings");

        if(showSprites)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("BG Sprite", SPRITE_BG_TOOLTIP));
            mainScript.settings.bgSprite = (Sprite)EditorGUILayout.ObjectField(mainScript.settings.bgSprite, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Slot BG Sprite", SPRITE_CELL_BG_TOOLTIP));
            mainScript.settings.slotBgSprite = (Sprite)EditorGUILayout.ObjectField(mainScript.settings.slotBgSprite, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Default item Sprite", SPRITE_DEFAULT_ITEM_TOOLTIP));
            mainScript.settings.defaultItemSprite = (Sprite)EditorGUILayout.ObjectField(mainScript.settings.defaultItemSprite, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();

        }
    }

    private void DisplayPadding()
    {
        showPadding = EditorGUILayout.Foldout(showPadding, "Padding");

        if (showPadding)
        {
            mainScript.settings.leftPadding = EditorGUILayout.IntSlider("Left Padding", mainScript.settings.leftPadding, MAX_MIN_PADDING[0], MAX_MIN_PADDING[1]);
            mainScript.settings.rightPadding = EditorGUILayout.IntSlider("Right Padding", mainScript.settings.rightPadding, MAX_MIN_PADDING[0], MAX_MIN_PADDING[1]);
            mainScript.settings.topPadding = EditorGUILayout.IntSlider("Top Padding", mainScript.settings.topPadding, MAX_MIN_PADDING[0], MAX_MIN_PADDING[1]);
            mainScript.settings.bottomPadding = EditorGUILayout.IntSlider("Bottom Padding", mainScript.settings.bottomPadding, MAX_MIN_PADDING[0], MAX_MIN_PADDING[1]);
        }
    }

    private void DisplayButtons()
    {
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Add random item", GUILayout.MinHeight(25)))
        {
            foreach (var item in InventoryManager.items)
            {

            }
        }
        GUI.backgroundColor = Color.red;
        if(GUILayout.Button("Clear items", GUILayout.MinHeight(25)))
        {

        }
        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = Color.yellow;
        GUILayout.Space(5f);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create UI Preview", GUILayout.MinHeight(25)))
        {
            UICreator.CreatePreview(mainScript.settings);
        }
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Delete UI Preview", GUILayout.MinHeight(25)))
        {
            if(UICreator.inventoryCanvas == null)
            {
                Debug.LogWarning("Nothing to delete");
                return;
            }
            DestroyImmediate(UICreator.inventoryCanvas);
        }

        
        
        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("Reset Layout", GUILayout.MinHeight(25)))
        {
            mainScript.settings.ResetSettings();
        }

        GUI.backgroundColor = Color.cyan;
        if(GUILayout.Button("Create Prefab", GUILayout.MinHeight(25)))
        {
            CreatePrefab();
        }

        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;       
    }

    private static void CreatePrefab()
    {
        if(UICreator.inventoryCanvas == null)
        {
            Debug.LogError("No preview was created, please create a preview first!");
            return;
        }

        string localPath = $"{PREFAB_FOLDER}{UICreator.inventoryCanvas}.prefab";

        if (File.Exists(localPath))
        {
            File.Delete(localPath);
        }

        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);        

        PrefabUtility.SaveAsPrefabAssetAndConnect(UICreator.inventoryCanvas, localPath, InteractionMode.UserAction);
    }
}
#endif