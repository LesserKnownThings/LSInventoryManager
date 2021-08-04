///*****************************************
///** CREATED BY LesserKnownThings        **
///** THIS SYSTEM IS STILL IN DEVELOPMENT **
///** System v1.0                         **
///*****************************************

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;



public enum WindowTypeEnum
{
    Items,
    Stats,
    Market,
    Crafting,
    Categories
}

public class ItemCreatorWindow : EditorWindow
{
    public GUISkin menuItemsGUISettings;
    public GUISkin itemsWindowSkin;
    public Texture2D deleteItemButton;
    public Texture2D addStatButton;
    public Texture2D removeStatButton;

    private const string STACKABLE_TOOLTIP = "Select this if the item can stack multiple times";
    private const string STACKABLE_AMOUNT_TOOLTIP = "The maximum amount that the item can stack";
    private const string OVERRIDE_TOOLTIP = "Toggle this if you want to override the data of the variable";
    private const string ABOUT_TEXT = "Create by Lesser Known Things\n\nYou can use this asset as you please no credit necessary.\n\nYou can use this as you please, the asset is completely free.";
    private const string REPO_PATH = "https://github.com/LesserKnownThings/LSInventoryManager";

    private const string IN_GAME_NAME_TOOLTIP = "The name that will be used to display the item name in game";

    #region Size Variables
    private const float BOTTOM_BUTTONS_SIZE = 35f;
    private const float BUTTONS_SIZE = 85f;
    private const float TEXT_AREA_SIZE = 185;
    private const float ICON_BUTTON_SIZE = 25;

    private const float MENU_ITEM_BUTTON_WIDTH = 150;
    private const float DELETE_BUTTON_SIZE = 50;
    #endregion

    private static ItemCreatorWindow instance;
    public static ItemCreatorWindow Instance { get { if (instance == null) { instance = (ItemCreatorWindow)EditorWindow.GetWindow(typeof(ItemCreatorWindow)); } return instance; } }

    private Dictionary<object, string> allowedRefs;
    public static List<CustomItem> items = new List<CustomItem>();
    public static List<ItemVariables> variables = new List<ItemVariables>();
    public static List<Category> categories = new List<Category>();

    /// <summary>
    /// Item same name
    /// Item name violation
    /// Variable same name
    /// Variable name violation
    /// </summary>
    private static bool[] violationsStatus = new bool[] { false, false, false, false };
    public static bool cannotCompile
    { 
        get 
        { 
            foreach (var violation in violationsStatus)
            {
                if (violation)
                {
                    return true;
                }
            }
            return false;
        } 
    }

    private bool cannotCompileEnum;
    

    private string[] refsNames;
    private Vector2 scrollPos;

    private Regex itemNameViolationRegex = new Regex(@"^([_]?[a-zA-Z])+([_]?[0-9]*[a-zA-Z]*[_]?)*$");
    private Regex variableNameViolationRegex = new Regex(@"(?:[\s]|^)(description|isStackable|stackAmount|itemIcon|itemName|inGameName|category)(?=[\s]|$)");

    private int toolbarInt = 0;
    private string[] menuToolbarValues = {"Items", "Variables", "Categories", "Market", "Crafting", "About" };

    public void Initialize()
    {
        allowedRefs = new Dictionary<object, string>();
        allowedRefs.Add(typeof(int).Name, "int");        
        allowedRefs.Add(typeof(float).Name, "float");
        allowedRefs.Add(typeof(string).Name, "string");        
        allowedRefs.Add(typeof(bool).Name, "bool");
    }


    [MenuItem("LesserKnown/Items")]
    public static void Init()
    {
        instance = (ItemCreatorWindow)EditorWindow.GetWindow(typeof(ItemCreatorWindow));

        instance.Initialize();
        instance.Show();

        instance.refsNames = new string[instance.allowedRefs.Count];
        int index = 0;


        foreach (var key in instance.allowedRefs)
        {
            instance.refsNames[index] = key.Value;
            index++;
        }

        variables = ItemCreator.GetData<List<ItemVariables>>(DataTypeEnum.Variables);

        if(variables == null)
        {
            variables = new List<ItemVariables>();
        }

        items = ItemCreator.GetData<List<CustomItem>>(DataTypeEnum.Items);       

        if(items == null)
        {
            items = new List<CustomItem>();
        }

        categories = ItemCreator.GetData<List<Category>>(DataTypeEnum.Categories);

        if(categories == null)
        {
            categories = new List<Category>();
        }

    }
    

    
    private void OnEnable()
    {
        List<CustomItem> virtualItems = ItemCreator.GetData<List<CustomItem>>(DataTypeEnum.Items);

        if (virtualItems != null)
        {
            foreach (var item in virtualItems)
            {
                AddItem(item);
            }
        }

        List<ItemVariables> virtualVariables = ItemCreator.GetData<List<ItemVariables>>(DataTypeEnum.Variables);

        if (virtualVariables != null)
        {
            foreach (var variable in virtualVariables)
            {
                AddVariable(variable);
            }
        }

        List<Category> virtualCategories = ItemCreator.GetData<List<Category>>(DataTypeEnum.Categories);

        if (virtualCategories != null)
        {
            foreach (var category in virtualCategories)
            {
                AddCategory(category);
            }
        }
    }  

    void OnGUI()
    {
        DrawHeader();

        ///ITEMS AND SUB-MENUS
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical("box");
        DrawMenuButtons();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();

        switch (toolbarInt)
        {
            case 0:
                DrawItems();
                break;
            case 1:
                DrawVariables();
                break;
            case 2:
                DrawCategories();
                break;
            case 5:
                DrawAboutPage();
                break;            
            default:
                DrawInProgress();
                break;
        }

        

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();       
    }

    private void DrawCategories()
    {
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("Compile & Save Enum", GUILayout.MaxWidth(150f), GUILayout.MinHeight(30f)))
        {
            SaveData();
            CompileEnumData();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        DrawAddCategory();
        DrawRemoveCategory(true, -1);
        EditorGUILayout.EndHorizontal();
        
        for (int i = 0; i < categories.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (i > 0)
            {
                categories[i].data = EditorGUILayout.TextField(categories[i].data);
            }
            else
            {
                EditorGUILayout.LabelField(categories[i].data);
            }
            

            if (HasNameViolation(categories[i].data, false) || HasSameName(i, DataTypeEnum.Categories))
            {
                cannotCompileEnum = true;
                EditorGUILayout.HelpBox("Name Violation!", MessageType.Error);
            }
            else
            {
                cannotCompileEnum = false;
            }

            if (i > 0)
            {
                DrawRemoveCategory(false, i);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawAddCategory()
    {
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.yellow;

        if (addStatButton != null)
        {
            if (GUILayout.Button(addStatButton, GUILayout.Width(ICON_BUTTON_SIZE), GUILayout.Height(ICON_BUTTON_SIZE)))
            {
                AddCategory();
            }
        }
        else
        {
            if (GUILayout.Button("Add Stat", GUILayout.MaxWidth(BUTTONS_SIZE)))
            {
                AddCategory();
            }
        }


        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = Color.white;
    }

    private void DrawRemoveCategory(bool isLast, int index)
    {
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.red;

        if (addStatButton != null)
        {
            if (GUILayout.Button(removeStatButton, GUILayout.Width(ICON_BUTTON_SIZE), GUILayout.Height(ICON_BUTTON_SIZE)))
            {
                if(isLast)
                {
                    RemoveCategory();
                }
                else
                {
                    RemoveCateogryAt(index);
                }
            }
        }
        else
        {
            if (GUILayout.Button("Remove", GUILayout.MaxWidth(BUTTONS_SIZE)))
            {
                if (isLast)
                {
                    RemoveCategory();
                }
                else
                {
                    RemoveCateogryAt(index);
                }
            }
        }


        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = Color.white;
    }

    private void AddCategory()
    {
        Category category = new Category(GUID.Generate().ToString(), $"Category{categories.Count}");
        categories.Add(category);
        SaveData();
    }

    private void AddCategory(Category category)
    {
        categories.Add(category);
    }

    private void RemoveCategory()
    {
        if(categories.Count == 1)
        {
            Debug.LogWarning("Not allowed to delete default");
            return;
        }
        categories.RemoveAt(categories.Count - 1);
        SaveData();
    }

    private void RemoveCateogryAt(int index)
    {
        categories.RemoveAt(index);
        SaveData();
    }

    private void DrawInProgress()
    {
        EditorGUILayout.LabelField("In Progress", EditorStyles.boldLabel);
    }

    private void DrawAboutPage()
    {
        GUI.color = Color.yellow;
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("About", EditorStyles.boldLabel);

        GUI.color = Color.white;
        GUIStyle style = new GUIStyle();
        style.fontSize = 15;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        EditorGUILayout.LabelField(ABOUT_TEXT, style, GUILayout.Width(400), GUILayout.Height(100));
        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("GitHub Repository", style);
        GUI.color = Color.black;
        if(GUILayout.Button(REPO_PATH, style))
        {
            Application.OpenURL(REPO_PATH);
        }

        GUI.color = Color.white;
        EditorGUILayout.EndVertical();
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal();

        GUI.contentColor = Color.yellow;
        GUILayout.Label("RPG Creator", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Save Data", GUILayout.MaxWidth(100f), GUILayout.MinHeight(30f)))
        {
            SaveData();
        }

        GUI.backgroundColor = Color.white;

        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("Compile Scripts", GUILayout.MaxWidth(100f), GUILayout.MinHeight(30f)))
        {
            CompileData();
        }

        GUI.backgroundColor = Color.white;
        GUI.contentColor = Color.white;        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawItemButtons()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New Item", GUILayout.MinHeight(BOTTOM_BUTTONS_SIZE)))
        {
            AddItem();
        }       
        GUILayout.EndHorizontal();
    }

    private void DrawMenuButtons()
    {
        if (menuItemsGUISettings == null)
        {
            return;
        }
        GUI.skin = menuItemsGUISettings;

        toolbarInt = GUILayout.SelectionGrid(toolbarInt, menuToolbarValues, 1, GUILayout.Width(MENU_ITEM_BUTTON_WIDTH));
    
        GUI.skin = null;
    }

    private bool HasNameViolation(string data, bool isVariable)
    {
        bool notNameViolation = false;
         

        notNameViolation = itemNameViolationRegex.IsMatch(data);

        if(isVariable)
        {
            bool hasVariableName = false;
            hasVariableName = variableNameViolationRegex.IsMatch(data);

            return !notNameViolation || hasVariableName;
        }

        return !notNameViolation;
    }

    private void DrawItems()
    {

        if(itemsWindowSkin != null)
        {
            GUI.skin = itemsWindowSkin;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        for (int i = 0; i < items.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");


            items[i].displayHidden = EditorGUILayout.Foldout(items[i].displayHidden, items[i].name);

            
            if (HasSameName(i, DataTypeEnum.Items))
            {
                EditorGUILayout.HelpBox("Name in use!", MessageType.Error);
                violationsStatus[0] = true;
            }
            else
            {
                violationsStatus[0] = false;
            }

            if (HasNameViolation(items[i].name, false))
            {
                EditorGUILayout.HelpBox("Naming violation", MessageType.Error);
                violationsStatus[1] = true;
            }
            else
            {
                violationsStatus[1] = false;
            }

            //Draw items only when they're toggled
            if (!items[i].displayHidden)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                items[i].name = EditorGUILayout.TextField(items[i].name, GUILayout.ExpandWidth(false));
                GUI.color = Color.yellow;
                EditorGUILayout.LabelField(new GUIContent("In Game Name", IN_GAME_NAME_TOOLTIP), EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                items[i].inGameName = EditorGUILayout.TextField(items[i].inGameName, GUILayout.ExpandWidth(false));
                GUI.color = Color.white;

                items[i].category = (ItemCategoriesEnum)EditorGUILayout.EnumPopup("Category", items[i].category);

                EditorGUILayout.Space(10);

                items[i].isStackable = EditorGUILayout.Toggle(new GUIContent("Stackable", STACKABLE_TOOLTIP), items[i].isStackable, GUILayout.ExpandWidth(false));

                if (items[i].isStackable)
                {
                    items[i].stackAmount = EditorGUILayout.IntField(new GUIContent("Stack Amount", STACKABLE_AMOUNT_TOOLTIP), items[i].stackAmount, GUILayout.ExpandWidth(false));
                }
                else
                {
                    items[i].stackAmount = 1;
                }

                
                

                EditorGUILayout.Space(10);
                EditorGUILayout.EndVertical();

                

                ///Draw Icon select
                DrawIconSelection(i);
                if (itemsWindowSkin != null)
                {
                    GUI.skin = itemsWindowSkin;
                }

                EditorGUILayout.EndHorizontal();

                items[i].showDescription = EditorGUILayout.Foldout(items[i].showDescription, "Description");

                if (items[i].showDescription)
                {
                    EditorStyles.textArea.wordWrap = true;

                    Vector2 currentScrollView = new Vector2(items[i].textScrollX, items[i].textScrollY);
                    currentScrollView = EditorGUILayout.BeginScrollView(currentScrollView, GUILayout.Height(150));
                    items[i].textScrollX = currentScrollView.x;
                    items[i].textScrollY = currentScrollView.y;

                    items[i].description = EditorGUILayout.TextArea(items[i].description, EditorStyles.textArea, GUILayout.ExpandHeight(true));
                    EditorGUILayout.EndScrollView();
                }

                GUI.color = Color.yellow;
                EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
                GUI.color = Color.white;


                DrawItemVariables(i);
            }

            DeleteItem(i);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(30);
        }

        EditorGUILayout.EndScrollView();


        DrawItemButtons(); //Saving data and Adding new items

        GUI.skin = null;
    }

  

    #region Item Variables
    private void DrawItemVariables(int index)
    {
        DrawItemVarButtons(index);

        for (int i = 0; i < items[index].addedVariables.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(items[index].addedVariables[i].varName, EditorStyles.boldLabel, GUILayout.ExpandWidth(false), GUILayout.Width(125));

            var itemVariables = items[index].addedVariables;
            var currentItem = itemVariables[i];

            currentItem.overrideData = EditorGUILayout.Toggle(new GUIContent("Override Data", OVERRIDE_TOOLTIP), currentItem.overrideData, GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Value", EditorStyles.boldLabel, GUILayout.Width(75));

            int sameIndex = variables.FindIndex(x => { return x.uid == currentItem.uid || x.varName == currentItem.varName; });

            ///Draw all types variable types
            switch (currentItem.index)
            {
                case 0:     
                    if (currentItem.overrideData)
                    {
                        int intData = ItemCreator.UpdateData<int>(itemVariables[i].data, itemVariables[i].dataType, typeof(int));
                        currentItem.data = EditorGUILayout.IntField(intData, GUILayout.ExpandWidth(false));
                    }
                    else
                    {                        
                        EditorGUILayout.LabelField(currentItem.data.ToString(), GUILayout.ExpandWidth(false));
                    }

                    currentItem.dataType = typeof(int);
                    break;

                case 1:      
                    if (currentItem.overrideData)
                    {
                        float floatData = ItemCreator.UpdateData<float>(currentItem.data, itemVariables[i].dataType, typeof(float));
                        currentItem.data = EditorGUILayout.FloatField(floatData, GUILayout.ExpandWidth(false));
                    }
                    else
                    {
                        EditorGUILayout.LabelField(currentItem.data.ToString(), GUILayout.ExpandWidth(false));
                    }

                    currentItem.dataType = typeof(float);
                    break;

                case 2:
                    string stringData = ItemCreator.UpdateData<string>(currentItem.data, itemVariables[i].dataType, typeof(string));

                    string dataString = stringData;

                    if (currentItem.overrideData)
                    {
                        if (dataString.Length < 25)
                        {
                            currentItem.data = EditorGUILayout.TextField(stringData);
                        }
                        else
                        {
                            GUILayout.BeginVertical();
                            EditorStyles.textField.wordWrap = true;
                            currentItem.data = EditorGUILayout.TextArea(stringData, GUILayout.MaxWidth(TEXT_AREA_SIZE), GUILayout.MinWidth(TEXT_AREA_SIZE));
                            GUILayout.EndVertical();
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField(currentItem.data.ToString(), EditorStyles.wordWrappedLabel, GUILayout.MaxWidth(TEXT_AREA_SIZE));
                    }

                    currentItem.dataType = typeof(string);
                    break;

                case 3:                   

                    if (currentItem.overrideData)
                    {
                        bool booldata = ItemCreator.UpdateData<bool>(currentItem.data, itemVariables[i].dataType, typeof(bool));
                        currentItem.data = EditorGUILayout.Toggle(booldata, GUILayout.ExpandWidth(false));
                    }
                    else
                    {
                        EditorGUILayout.LabelField(currentItem.data.ToString(), GUILayout.ExpandWidth(false));
                    }

                    currentItem.dataType = typeof(bool);
                    break;

            }

            if (!currentItem.overrideData)
            {
                currentItem.data = variables[sameIndex].data;
            }

            itemVariables[i] = currentItem;            

            DrawDeleteItemStatButton(index, i, false);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();            
        }


        
    }   

    private void DrawItemVarButtons(int index)
    {
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.yellow;

        if (!cannotCompile)
        {


            if (addStatButton != null)
            {
                if (GUILayout.Button(addStatButton, GUILayout.Width(ICON_BUTTON_SIZE), GUILayout.Height(ICON_BUTTON_SIZE)))
                {
                    AddItemVariable(index);
                }
            }
            else
            {
                if (GUILayout.Button("Add Variable"))
                {
                    AddItemVariable(index);
                }
            }

            DrawDeleteItemStatButton(index, -1, true);

        }


        EditorGUILayout.EndHorizontal();
    }

    private void DrawDeleteItemStatButton(int index, int varIndex, bool removeLast)
    {
        GUI.backgroundColor = Color.red;
        if (removeStatButton != null)
        {
            if (GUILayout.Button(removeStatButton, GUILayout.Width(ICON_BUTTON_SIZE), GUILayout.Height(ICON_BUTTON_SIZE)))
            {
                if (removeLast)
                {
                    RemoveLastItemVariable(index);
                }
                else
                {
                    RemoveItemVariableAt(index, varIndex);
                }
            }
        }
        else
        {
            if (GUILayout.Button("Remove Variable"))
            {
                if (removeLast)
                {
                    RemoveLastItemVariable(index);
                }
                else
                {
                    RemoveItemVariableAt(index, varIndex);
                }
            }
        }
        GUI.backgroundColor = Color.white;
    }

    Rect buttonRect;
    private void AddItemVariable(int index)
    {
        
        PopupWindow.Show(buttonRect, new VarPickerPopup(index));

        
    }

    private void RemoveLastItemVariable(int index)
    {
        if(items[index].addedVariables.Count == 0)
        {
            Debug.LogWarning("Item has no variables!");
            return;
        }

        items[index].addedVariables.RemoveAt(items[index].addedVariables.Count - 1);

        SaveData();
    }

    private void RemoveItemVariableAt(int index, int varIndex)
    {
        items[index].addedVariables.RemoveAt(varIndex);

        SaveData();
    }

    #endregion


    private void DeleteItem(int index)
    {
        GUI.backgroundColor = Color.red;
        if (deleteItemButton == null)
        {
            if (GUILayout.Button("Delete Item", GUILayout.MaxWidth(BUTTONS_SIZE)))
            {
                DeleteItemCleanup(index);
            }
        }
        else
        {
            if (GUILayout.Button(deleteItemButton, GUILayout.Width(DELETE_BUTTON_SIZE), GUILayout.Height(DELETE_BUTTON_SIZE)))
            {
                DeleteItemCleanup(index);
            }
        }

        GUI.backgroundColor = Color.white;
    }

    private void DeleteItemCleanup(int index)
    {
        items.RemoveAt(index);
        SaveData();
    }

    /// <summary>
    /// Draws the selection window for the item icon
    /// </summary>
    /// <param name="index"></param>
    private void DrawIconSelection(int index)
    {
        GUI.skin = null;
        Sprite currentSprite = ItemCreator.GetItemIcon(items[index].itemIconDirectory);

        currentSprite = (Sprite)EditorGUILayout.ObjectField("Item Icon",currentSprite, typeof(Sprite), false);

        if(!AssetDatabase.GetAssetPath(currentSprite).Equals(items[index].itemIconDirectory))
        {
            items[index].itemIconDirectory = AssetDatabase.GetAssetPath(currentSprite);
            SaveData();
        }        
    }

    private void DrawAddButton()
    {
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.yellow;

        if(addStatButton != null)
        {
            if(GUILayout.Button(addStatButton, GUILayout.Width(ICON_BUTTON_SIZE), GUILayout.Height(ICON_BUTTON_SIZE)))
            {
                AddVariable();
            }
        }
        else
        {
            if (GUILayout.Button("Add Stat", GUILayout.MaxWidth(BUTTONS_SIZE)))
            {
                AddVariable();
            }
        }
        
       
        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = Color.white;
    }

    private void DrawVariables()
    {
        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.yellow;
        EditorGUILayout.LabelField("Variables", EditorStyles.boldLabel);
        GUI.color = Color.white;

        DrawAddButton();
        DrawRemoveStat(-1, true);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < variables.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Stat Name", GUILayout.MaxWidth(75f));
            variables[i].varName = EditorGUILayout.TextField(variables[i].varName, GUILayout.ExpandWidth(false));

            EditorGUILayout.LabelField("Value", GUILayout.MaxWidth(40f));

            ///Draw all types variable types
            
            switch (variables[i].index)
            {
                case 0:
                    int intData = ItemCreator.UpdateData<int>(variables[i].data, variables[i].dataType, typeof(int));
                    variables[i].data = EditorGUILayout.IntField(intData);
                    variables[i].dataType = typeof(int);
                    break;

                case 1:
                    float floatData = ItemCreator.UpdateData<float>(variables[i].data, variables[i].dataType, typeof(float));
                    variables[i].data = EditorGUILayout.FloatField(floatData);
                    variables[i].dataType = typeof(float);
                    break;

                case 2:
                    string stringData = ItemCreator.UpdateData<string>(variables[i].data, variables[i].dataType, typeof(string));

                    string dataString = stringData;

                    if (dataString.Length < 25)
                        variables[i].data = EditorGUILayout.TextField(stringData);
                    else
                    {
                        GUILayout.BeginVertical();
                        EditorStyles.textField.wordWrap = true;
                        variables[i].data = EditorGUILayout.TextArea(stringData, GUILayout.MaxWidth(TEXT_AREA_SIZE), GUILayout.MinWidth(TEXT_AREA_SIZE));
                        GUILayout.EndVertical();
                    }

                    variables[i].dataType = typeof(string);
                    break;

                case 3:
                    bool booldata = ItemCreator.UpdateData<bool>(variables[i].data, variables[i].dataType, typeof(bool));
                    variables[i].data = EditorGUILayout.Toggle(booldata);
                    variables[i].dataType = typeof(bool);
                    break;                   

            }
            

            variables[i].index = EditorGUILayout.Popup(variables[i].index, refsNames);            

            if (HasSameName(i, DataTypeEnum.Variables))
            {
                EditorGUILayout.HelpBox("Name in use!", MessageType.Error);
                violationsStatus[2] = true;
            }
            else
            {
                violationsStatus[2] = false;
            }

            if (HasNameViolation(variables[i].varName, true))
            {
                EditorGUILayout.HelpBox("Naming violation", MessageType.Error);
                violationsStatus[3] = true;
            }
            else
            {
                violationsStatus[3] = false;
            }

            DrawRemoveStat(i, false);

            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// Will delete a stat
    /// </summary>
    /// <param name="index">the stat that you want to remove </param>
    /// <param name="removeLast">if set to yes the last item will be deleted and the index will be ignored</param>
    private void DrawRemoveStat(int index, bool removeLast)
    {        
        GUI.backgroundColor = Color.red;
        if (removeStatButton == null)
        {
            if (GUILayout.Button("Remove Stat", GUILayout.MaxWidth(BUTTONS_SIZE)))
            {                
                RemoveVariable(index, removeLast);
            }
        }
        else
        {
            if (GUILayout.Button(removeStatButton, GUILayout.Width(ICON_BUTTON_SIZE), GUILayout.Height(ICON_BUTTON_SIZE)))
            {
                RemoveVariable(index, removeLast);
            }
        }
        GUI.backgroundColor = Color.white;
    }

    #region Public API
    public static CustomItem GetItem(int index)
    {
        if(index >= items.Count || index < 0)
        {
            Debug.LogError("Item does not exist, or index is out of range");
            return null;
        }
        return items[index];
    }

    public static CustomItem GetItem(string itemName)
    {
        int index = items.FindIndex(x => { return itemName == x.name; });

        if(index == -1)
        {
            Debug.LogError("Item does not exist, or index is out of range");
            return null;
        }

        return items[index];
    }
    #endregion

    #region Private API
    private void AddItem()
    {
        CustomItem currentItem = new CustomItem();
        currentItem.name = $"Item";

        currentItem.itemIconDirectory = "";
        currentItem.uid = Guid.NewGuid().ToString();
        items.Add(currentItem);

        SaveData();
    }

    private void AddItem(CustomItem item)
    {
        items.Add(item);
    }

    private void AddVariable()
    {
        ItemVariables itemVariable = new ItemVariables();
        itemVariable.index = variables.Count - 1;
        itemVariable.dataType = typeof(int);
        itemVariable.data = 0;
        itemVariable.uid = Guid.NewGuid().ToString();
        variables.Add(itemVariable);        
        SaveData();
    }

    private void AddVariable(ItemVariables variable)
    {
        variables.Add(variable);
    }

    private void RemoveVariable(int index, bool lastVar)
    {
        if (lastVar)
        {
            if (variables.Count == 0)
            {
                Debug.LogError("The variables list is empty!");
                return;
            }

            int lastIndex = variables.Count - 1;

            for (int i = 0; i < items.Count; i++)
            {
                int hasVarIndex = items[i].addedVariables.FindIndex(x => { return x.uid == variables[lastIndex].uid || x.varName == variables[lastIndex].varName; });

                if(hasVarIndex > -1)
                {
                    items[i].addedVariables.RemoveAt(hasVarIndex);
                    --i;
                }

            }

            variables.RemoveAt(lastIndex);
        }
        else
        {

            for (int i = 0; i < items.Count; i++)
            {
                int hasVarIndex = items[i].addedVariables.FindIndex(x => { return x.uid == variables[index].uid || x.varName == variables[index].varName; });

                if (hasVarIndex > -1)
                {
                    items[i].addedVariables.RemoveAt(hasVarIndex);
                    --i;
                }

            }

            variables.RemoveAt(index);
        }        

        SaveData();
    }

    private bool HasSameName(int index, DataTypeEnum dataType)
    {
        int sameNameIndex = -1;
        
        if(dataType == DataTypeEnum.Items)
        {
            sameNameIndex = items.FindIndex(x => { return x.uid != items[index].uid && x.name == items[index].name; });
        }
        else if (dataType == DataTypeEnum.Variables)
        {
            sameNameIndex = variables.FindIndex(x => { return x.uid != variables[index].uid && x.varName == variables[index].varName; });
        }else if(dataType == DataTypeEnum.Categories)
        {
            sameNameIndex = categories.FindIndex(x => { return x.uid != categories[index].uid && x.data.Equals(categories[index].data); });
        }


        return sameNameIndex > -1;
    }

    public void SaveVariableChnage()
    {
        SaveData();
    }

    private void SaveData()
    {
        violationsStatus = new bool[] { false, false, false, false };

        if (cannotCompile)
        {
            Debug.LogError("Cannot save data, you must fix all errors before saving");
            return;
        }
        ItemCreator.SaveItemData();
        ItemCreator.SaveVarData();
        ItemCreator.SaveCategoryData();
    }

    private void CompileData()
    {
        if (cannotCompile)
        {
            Debug.LogError("Cannot compile, you must fix all errors before compiling");
            return;
        }

        DynamicClassCreator.CompileClass();
    }

    private void CompileEnumData()
    {
        if (cannotCompileEnum)
        {
            Debug.LogError("Cannot compile the current enum, you must fix all errors before compiling");
            return;
        }

        DynamicClassCreator.CompileEnum();
    }

    #endregion

    #region Internal Classes  

    [System.Serializable]
    public class ItemVariables
    {
        public int index = 0;
        public string varName = string.Empty;
        public object data;
        public Type dataType;
        public string uid;
        public bool overrideData;
        
        public T GetData<T>()
        {
            return (T)data;
        }
    }

    [System.Serializable]
    public class Category
    {
        public string uid;
        public string data;

        public Category(string uid, string data)
        {
            this.uid = uid;
            this.data = data;
        }
    }

    public class SerializedTexture
    {
        public byte[] data { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public TextureFormat format { get; set; }
    }
    

    [System.Serializable]    
    public class CustomItem
    {
        [HideInInspector]
        public string name;
        [HideInInspector]
        public string uid;
        [HideInInspector]
        public string itemIconDirectory;
        [HideInInspector]
        public bool isStackable;
        [HideInInspector]
        public int stackAmount;
        [HideInInspector]
        public string description = "";
        [HideInInspector]
        public string inGameName = "";
        [HideInInspector]
        public ItemCategoriesEnum category;
        [HideInInspector]
        public List<ItemVariables> addedVariables = new List<ItemVariables>();

        [HideInInspector]
        public float textScrollX = 0f;
        public float textScrollY = 0f;
        [HideInInspector]
        public bool showDescription;
        [HideInInspector]
        public bool displayHidden;
    }

    [System.Serializable]
    public class DynamicIndex
    {
        public object data;
        public Type type;
    }
    #endregion

}
#endif

