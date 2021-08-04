#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;



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
    private const string STACK_SLOT_TEXT_COLOR = "The text color of the stack cell";
    private const string SLOT_BG_COLOR_TOOLTIP = "The background color of the cells inside the inventory window";
    private const string SPRITE_BG_TOOLTIP = "The sprite for the inventory window";
    private const string SPRITE_CELL_BG_TOOLTIP = "The sprite used for the cell inside the inventory window";
    private const string SPRITE_DEFAULT_ITEM_TOOLTIP = "The sprite used to render the default (empty slot) items inside the cells";
    private const string SPRITE_STACK_CELL_TOOLTIP = "The sprite used to render the stack cell background";
    private const string CLOSE_BUTTON_SPRITE = "The sprite used to render the close button for the inventory";
    private const string UI_INPUT_STRING = "The input used to open/close the UI. You will have to set this one up with the old Unity Input System";
    private const string ADD_EVENTSYSTEM_STRING = "Toggle this to add an event system if you know one will not be in the scene";
    private const string STACK_CELL_TOPLEFT_OFFSET = "Change the offset of the stack cell on the top and left level";
    private const string STACK_CELL_BOTTOMRIGHT_OFFSET = "Change the offset of the stack cell on the bottom and right level";
    private const string DESTROY_ONTHROW_TOOLTIP = "Toggle this if you want to destroy items when you throw them from the inventory";
    private const string CELL_OPTIONS_TOOLTIP = "The sprite used to render the background of the cell options window";
    private const string DESTROY_WINDOW_SPRITE_TOOLTIP = "The sprite used to render the background of the destroy items window";
    private const string DESTROY_WINDOW_BUTTONS_SPRITE = "The sprite used to render the buttons inside the destroy window";
    private const string CELL_OPTIONS_WINDOW_BUTTONS_SPRITE = "The sprite used to render the buttons inside the cell options window";
    private const string REWORK_UI_TOOLTIP = "Activate this if you want to rework the UI. This is yet in alpha and if you rework the UI the in game manager might stop working or might not work properly. Activate this if you are willing to rework the UI Manager";
    private const string MANUAL_REWORK_TOOLTIP = "If you want to rework the Inventory UI manually you can make changes to the prefab inside the Resources/Generated/Prefab";

    private readonly int[] MAX_MIN_PADDING = { 0, 200 };
    private readonly int[] MAX_MIN_CELL_SIZE = { 0, 90 };
    private readonly int[] MAX_MIN_CELL_AMOUNT = { 0, 42 };

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

        mainScript.alphaReworkUI = EditorGUILayout.Toggle(new GUIContent("Alpha UI Rework", REWORK_UI_TOOLTIP), mainScript.alphaReworkUI);

        if(!mainScript.alphaReworkUI)
        {
            GUI.color = Color.yellow;
            EditorGUILayout.LabelField(MANUAL_REWORK_TOOLTIP, EditorStyles.wordWrappedLabel);
            return;
        }

        mainScript.settings.cellNumber = EditorGUILayout.IntSlider(new GUIContent("Cell amount", CELL_AMOUNT_TOOLTIP), mainScript.settings.cellNumber, MAX_MIN_CELL_AMOUNT[0], MAX_MIN_CELL_AMOUNT[1]);

        EditorGUILayout.Space(5);
        DisplaySizes();

        //NEED TO WORK ON THIS IT"S NOT WORKING PROPERLY
        //mainScript.onValueChange = EditorGUILayout.Toggle(new GUIContent("Update editor", UPDATE_EDITOR_TOOLTIP), mainScript.onValueChange);
        mainScript.settings.addEventSystem = EditorGUILayout.Toggle(new GUIContent("Add Event System", ADD_EVENTSYSTEM_STRING), mainScript.settings.addEventSystem);
        mainScript.settings.destroyOnThrow = EditorGUILayout.Toggle(new GUIContent("Destroy on throw", DESTROY_ONTHROW_TOOLTIP), mainScript.settings.destroyOnThrow);

        if (mainScript.onValueChange && mainScript.settings != localSettings)
        {
            UICreator.CreatePreview(ref mainScript.settings, mainScript.transform);
            localSettings = mainScript.settings;           
        }

        mainScript.settings.bgColor = EditorGUILayout.ColorField(new GUIContent("BG color", BG_COLOR_TOOLTIP), mainScript.settings.bgColor);
        mainScript.settings.slotBgColor = EditorGUILayout.ColorField(new GUIContent("Slot BG color", SLOT_BG_COLOR_TOOLTIP), mainScript.settings.slotBgColor);
        mainScript.settings.stackSlotTextColor = EditorGUILayout.ColorField(new GUIContent("Stack text color", STACK_SLOT_TEXT_COLOR), mainScript.settings.stackSlotTextColor);
        EditorGUILayout.Space(10);
        mainScript.settings.uiOpenInput = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("UI Input", UI_INPUT_STRING), mainScript.settings.uiOpenInput);
        
        EditorGUILayout.Space(10);

        
        DisplaySprites();
        DisplayPadding();
        DisplayButtons();        
    }

    private void DisplaySizes()
    {
        Vector2 cellSize = new Vector2(mainScript.settings.horizontalCellSize, mainScript.settings.verticalCellSize);
        cellSize = EditorGUILayout.Vector2Field(new GUIContent("Cell size", CELL_SIZE_TOOLTIP), cellSize);
        mainScript.settings.horizontalCellSize = cellSize.x;
        mainScript.settings.verticalCellSize = cellSize.y;

        Vector2 spacing = new Vector2(mainScript.settings.horizontalSpacing, mainScript.settings.verticalSpacing);
        spacing = EditorGUILayout.Vector2Field("Cell Spacing", spacing);
        mainScript.settings.horizontalSpacing = spacing.x;
        mainScript.settings.verticalSpacing = spacing.y;

        mainScript.settings.stackTextSize = EditorGUILayout.FloatField("Stack Text Size", mainScript.settings.stackTextSize);

        mainScript.settings.stackCellTopLeftOffset = EditorGUILayout.Vector2Field(new GUIContent("Stack TL offset", STACK_CELL_TOPLEFT_OFFSET), mainScript.settings.stackCellTopLeftOffset);
        mainScript.settings.stackCellBottomRightOffset = EditorGUILayout.Vector2Field(new GUIContent("Stack BR offset", STACK_CELL_BOTTOMRIGHT_OFFSET), mainScript.settings.stackCellBottomRightOffset);
    }

    private void DisplaySprites()
    {
        showSprites = EditorGUILayout.Foldout(showSprites, "Sprites Settings");

        if(showSprites)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("BG Sprite", SPRITE_BG_TOOLTIP), GUILayout.Width(135));
            mainScript.settings.bgSprite = (Sprite)EditorGUILayout.ObjectField(mainScript.settings.bgSprite, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Slot BG Sprite", SPRITE_CELL_BG_TOOLTIP), GUILayout.Width(135));
            mainScript.settings.slotBgSprite = (Sprite)EditorGUILayout.ObjectField(mainScript.settings.slotBgSprite, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Default item Sprite", SPRITE_DEFAULT_ITEM_TOOLTIP), GUILayout.Width(135));
            mainScript.settings.defaultItemSprite = (Sprite)EditorGUILayout.ObjectField(mainScript.settings.defaultItemSprite, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Close button Sprite", CLOSE_BUTTON_SPRITE), GUILayout.Width(135));
            mainScript.settings.closeButtonSprite = (Sprite)EditorGUILayout.ObjectField(mainScript.settings.closeButtonSprite, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Stack cell Sprite", SPRITE_STACK_CELL_TOOLTIP), GUILayout.Width(135));
            mainScript.settings.stackCellSprite = (Sprite)EditorGUILayout.ObjectField(mainScript.settings.stackCellSprite, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Cell options window", CELL_OPTIONS_TOOLTIP), GUILayout.Width(135));
            mainScript.settings.cellOptionsSprite = (Sprite)EditorGUILayout.ObjectField(mainScript.settings.cellOptionsSprite, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Destroy window Sprite", DESTROY_WINDOW_SPRITE_TOOLTIP), GUILayout.Width(135));
            mainScript.settings.destroyWindowBGSprite = (Sprite)EditorGUILayout.ObjectField(mainScript.settings.destroyWindowBGSprite, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Button Sprites");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Destroy buttons Sprite", DESTROY_WINDOW_BUTTONS_SPRITE), GUILayout.Width(135));
            mainScript.settings.destroyWindowButtonsSprite = (Sprite)EditorGUILayout.ObjectField(mainScript.settings.destroyWindowButtonsSprite, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Options buttons Sprite", CELL_OPTIONS_WINDOW_BUTTONS_SPRITE), GUILayout.Width(135));
            mainScript.settings.cellOptionsButtonsSprite = (Sprite)EditorGUILayout.ObjectField(mainScript.settings.cellOptionsButtonsSprite, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

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
            DynamicData[] folderItems = Resources.LoadAll(GENERATED_ITEMS_FOLDER).Cast<DynamicData>().ToArray();

            int emptyIndex = UICreator.cells.FindIndex(x => { return x.cellData.isEmpty; });

            if(emptyIndex > -1)
            {
                int randomItem = Random.Range(0, folderItems.Length);
                UICreator.cells[emptyIndex].cellData.AssignItem(folderItems[randomItem], 1);
            }
            else
            {
                Debug.LogError("No more empty cells!");
            }
        }
        GUI.backgroundColor = Color.red;
        if(GUILayout.Button("Clear items", GUILayout.MinHeight(25)))
        {
            foreach (var cell in UICreator.cells)
            {
                cell.cellData.ClearCell();
            }
        }
        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = Color.yellow;
        GUILayout.Space(5f);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create UI Preview", GUILayout.MinHeight(25)))
        {
            UICreator.CreatePreview(ref mainScript.settings, mainScript.transform);
            PrefabUtility.RecordPrefabInstancePropertyModifications(UICreator.inventoryCanvas);
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