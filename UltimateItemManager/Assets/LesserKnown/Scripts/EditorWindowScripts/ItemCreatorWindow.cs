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
    public Texture2D deleteItemButton;
    public Texture2D addStatButton;
    public Texture2D removeStatButton;

    private const string STACKABLE_TOOLTIP = "Select this if the item can stack multiple times";
    private const string STACKABLE_AMOUNT_TOOLTIP = "The maximum amount that the item can stack";
    private const string OVERRIDE_TOOLTIP = "Toggle this if you want to override the data of the variable";
    private const string ABOUT_TEXT = "Create by Lesser Known Things\n\nYou can use this asset as you please no credit necessary.\n\nYou can use this as you please, the asset is completely free.";
    private const string REPO_PATH = "https://github.com/LesserKnownThings/LSInventoryManager";

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
    public static bool cannotCompile = false;

    private List<bool> hiddenItems = new List<bool>();
    private string[] refsNames;
    private Vector2 scrollPos;

    private Regex reg = new Regex(@"^([_]?[a-zA-Z])+([_]?[0-9]*[a-zA-Z]*[_]?)*$");
    private MatchCollection matches;

    private WindowTypeEnum selectedWindowType = WindowTypeEnum.Items;

    private int toolbarInt = 0;
    private string[] menuToolbarValues = {"Items", "Variables", "Market", "Crafting", "Categories", "About" };

    private List<int> variablesIndex = new List<int>();
    private List<string> variableNames = new List<string>();

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


        for (int i = 0; i < items.Count; i++)
        {
            instance.hiddenItems.Add(false);
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
            case 5:
                DrawAboutPage();
                break;
            default:
                DrawInProgress();
                break;
        }

        EditorGUILayout.EndVertical();


        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();       
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

    private bool HasNameViolation(string data)
    {
        bool notNameViolation = false;

        notNameViolation = reg.IsMatch(data);

        return !notNameViolation;
    }

    private void DrawItems()
    {

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        for (int i = 0; i < items.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");


            hiddenItems[i] = EditorGUILayout.Foldout(hiddenItems[i], items[i].name);

            

            List<int> inUseNames = new List<int>();

            if (HasSameName(i, DataTypeEnum.Items))
            {
                EditorGUILayout.HelpBox("Name in use!", MessageType.Error);
                cannotCompile = true;
            }
            else
            {
                cannotCompile = false;
            }

            if (HasNameViolation(items[i].name))
            {
                EditorGUILayout.HelpBox("Naming violation", MessageType.Error);
                cannotCompile = true;
            }
            else
            {
                cannotCompile = false;
            }

            //Draw items only when they're toggled
            if (!hiddenItems[i])
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                items[i].name = EditorGUILayout.TextField(items[i].name, GUILayout.ExpandWidth(false));

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


                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

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

            ///Draw all types variable types
            switch (currentItem.index)
            {
                case 0:     
                    if (currentItem.overrideData)
                    {
                        int intData = ItemCreator.UpdateData<int>(itemVariables[i].data, typeof(int));
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
                        float floatData = ItemCreator.UpdateData<float>(currentItem.data, typeof(float));
                        currentItem.data = EditorGUILayout.FloatField(floatData, GUILayout.ExpandWidth(false));
                    }
                    else
                    {
                        EditorGUILayout.LabelField(currentItem.data.ToString(), GUILayout.ExpandWidth(false));
                    }

                    currentItem.dataType = typeof(float);
                    break;

                case 2:
                    string stringData = ItemCreator.UpdateData<string>(currentItem.data, typeof(string));

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
                        bool booldata = ItemCreator.UpdateData<bool>(currentItem.data, typeof(bool));
                        currentItem.data = EditorGUILayout.Toggle(booldata, GUILayout.ExpandWidth(false));
                    }
                    else
                    {
                        EditorGUILayout.LabelField(currentItem.data.ToString(), GUILayout.ExpandWidth(false));
                    }

                    currentItem.dataType = typeof(bool);
                    break;

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

        if(addStatButton != null)
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

        cannotCompile = false;
        SaveData();
    }

    private void RemoveItemVariableAt(int index, int varIndex)
    {
        items[index].addedVariables.RemoveAt(varIndex);

        cannotCompile = false;
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
        hiddenItems.RemoveAt(index);
        items.RemoveAt(index);
        cannotCompile = false;
        SaveData();
    }

    /// <summary>
    /// Draws the selection window for the item icon
    /// </summary>
    /// <param name="index"></param>
    private void DrawIconSelection(int index)
    {
        Sprite currentSprite = ItemCreator.GetItemIcon(items[index].itemIconDirectory);

        currentSprite = (Sprite)EditorGUILayout.ObjectField("Item Icon",currentSprite, typeof(Sprite), false, GUILayout.ExpandWidth(false));

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
                    int intData = ItemCreator.UpdateData<int>(variables[i].data, typeof(int));
                    variables[i].data = EditorGUILayout.IntField(intData);
                    variables[i].dataType = typeof(int);
                    break;

                case 1:
                    float floatData = ItemCreator.UpdateData<float>(variables[i].data, typeof(float));
                    variables[i].data = EditorGUILayout.FloatField(floatData);
                    variables[i].dataType = typeof(float);
                    break;

                case 2:
                    string stringData = ItemCreator.UpdateData<string>(variables[i].data, typeof(string));

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
                    bool booldata = ItemCreator.UpdateData<bool>(variables[i].data, typeof(bool));
                    variables[i].data = EditorGUILayout.Toggle(booldata);
                    variables[i].dataType = typeof(bool);
                    break;                   

            }
            
            variables[i].index = EditorGUILayout.Popup(variables[i].index, refsNames);            

            if (HasSameName(i, DataTypeEnum.Variables))
            {
                EditorGUILayout.HelpBox("Name in use!", MessageType.Error);
                cannotCompile = true;
            }
            else
            {
                cannotCompile = false;
            }

            if (HasNameViolation(variables[i].varName))
            {
                EditorGUILayout.HelpBox("Naming violation", MessageType.Error);
                cannotCompile = true;
            }
            else
            {
                cannotCompile = false;
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
        hiddenItems.Add(true);

        SaveData();
    }

    private void AddItem(CustomItem item)
    {
        items.Add(item);
        hiddenItems.Add(true);
    }

    private void AddVariable()
    {
        ItemVariables itemVariable = new ItemVariables();
        itemVariable.index = variables.Count - 1;
        itemVariable.dataType = typeof(int);
        itemVariable.data = 0;
        itemVariable.uid = Guid.NewGuid().ToString();
        variablesIndex.Add(variables.Count);
        variableNames.Add(itemVariable.varName);
        variables.Add(itemVariable);        
        SaveData();
    }

    private void AddVariable(ItemVariables variable)
    {
        variables.Add(variable);
        variablesIndex.Add(variablesIndex.Count);
        variableNames.Add(variable.varName);
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

        cannotCompile = false;
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
        }


        return sameNameIndex > -1;
    }

    public void SaveVariableChnage()
    {
        cannotCompile = false;
        SaveData();
    }

    private void SaveData()
    {
        if (cannotCompile)
        {
            Debug.LogError("Cannot save data, you must fix all errors before saving");
            return;
        }
        ItemCreator.SaveItemData();
        ItemCreator.SaveVarData();
    }

    private void CompileData()
    {
        if (cannotCompile)
        {
            Debug.LogError("Cannot compile, you must fix all errors before compiling");
            return;
        }
        DynamicClassCreator.Compile();
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
        public List<ItemVariables> addedVariables = new List<ItemVariables>();
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

