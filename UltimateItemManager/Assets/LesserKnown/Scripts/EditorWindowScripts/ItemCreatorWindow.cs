///*****************************************
///** CREATED BY LesserKnownThings        **
///** THIS SYSTEM IS STILL IN DEVELOPMENT **
///** System v1.0                         **
///*****************************************

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
public class ItemCreatorWindow : EditorWindow
{
    public GUISkin menuItemsGUISettings;

    private const string STACKABLE_TOOLTIP = "Select this if the item can stack multiple times";
    private const string STACKABLE_AMOUNT_TOOLTIP = "The maximum amount that the item can stack";

    #region Size Variables
    const float BOTTOM_BUTTONS_SIZE = 35f;
    const float BUTTONS_SIZE = 85f;
    const float TEXT_AREA_SIZE = 225f;

    const float MENU_BUTTON_WIDTH = 150;
    const float MENU_BUTTON_HEIGHT = 75;
    #endregion

    private static ItemCreatorWindow instance;
    public static ItemCreatorWindow Instance { get { return instance; } }

    private Dictionary<object, string> allowedRefs;
    public static List<CustomItem> items = new List<CustomItem>();
    public static bool cannotCompile = false;

    private List<bool> hiddenItems = new List<bool>();
    private List<bool> nameInuse = new List<bool>();
    private List<bool[]> nameViolation = new List<bool[]>();
    private string[] refsNames;
    private Vector2 scrollPos;

    private Regex reg = new Regex(@"^([_]?[a-zA-Z])+([_]?[0-9]*[a-zA-Z]*[_]?)*$");
    private MatchCollection matches;

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

        items = ItemCreator.GetData();

        if (items != null)
            for (int i = 0; i < items.Count; i++)
            {
                instance.hiddenItems.Add(false);
                instance.nameInuse.Add(false);
                instance.nameViolation.Add(new bool[] { false, false });
            }

    }
    

    
    private void OnEnable()
    {
        List<CustomItem> virtualItems = ItemCreator.GetData();

        if (virtualItems == null)
        {
            return;
        }
        else
        {
            foreach (var item in virtualItems)
            {
                AddItem(item);
            }
        }
    }

  

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();      

        GUI.contentColor = Color.yellow;
        GUILayout.Label("RPG Creator", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("Compile Scripts", GUILayout.MaxWidth(100f), GUILayout.MinHeight(30f)))
        {
            CompileData();
        }
        GUI.backgroundColor = Color.white;
        GUI.contentColor = Color.white;
        EditorGUILayout.EndHorizontal();

        ///ITEMS AND SUB-MENUS
        EditorGUILayout.BeginHorizontal();       

      //  DisplayMenuButtons();

        EditorGUILayout.BeginVertical();

        DrawItems();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New Item", GUILayout.MinHeight(BOTTOM_BUTTONS_SIZE)))
        {
            AddItem();
        }

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Save Data", GUILayout.MinHeight(BOTTOM_BUTTONS_SIZE)))
        {
            SaveData();
        }

        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();


        EditorGUILayout.EndHorizontal();
    }

    private void DisplayMenuButtons()
    {
        if (menuItemsGUISettings == null)
        {
            return;
        }
        //This is for all the menu buttons
        EditorGUILayout.BeginVertical();

        GUI.skin = menuItemsGUISettings;

        if (GUILayout.Button("Items"))
        {

        }

        if (GUILayout.Button("Classes"))
        {

        }
        GUI.skin = null;
        EditorGUILayout.EndVertical();
    }


    private void SaveData()
    {
        if (cannotCompile)
        {
            Debug.LogError("Cannot compile, you must fix all errors before compiling");
            return;
        }
        ItemCreator.SaveData();
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


    private bool CheckNameViolation(string data)
    {
        bool isNameViolation = false;

        isNameViolation = reg.IsMatch(data);

        return isNameViolation;
    }

    private void DrawItems()
    {
        if (items.Count == 0)
            return;        
        
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        for (int i = 0; i < items.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");

            hiddenItems[i] = EditorGUILayout.Foldout(hiddenItems[i], items[i].name);

            CheckItemName(items[i], i);

            if (nameInuse[i])
                EditorGUILayout.HelpBox("Name in use!", MessageType.Error);

            if(nameViolation[i][0])
                EditorGUILayout.HelpBox("Naming violation", MessageType.Error);
            if(nameViolation[i][1])
                EditorGUILayout.HelpBox("Naming violation in stats", MessageType.Error);

            //Draw items only when they're toggled
            if (!hiddenItems[i])
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name", GUILayout.ExpandWidth(false));
                items[i].name = EditorGUILayout.TextField(items[i].name, GUILayout.ExpandWidth(false));


                nameViolation[i][0] = !CheckNameViolation(items[i].name);

                EditorGUILayout.BeginVertical();

                items[i].isStackable = EditorGUILayout.Toggle(new GUIContent("Stackable", STACKABLE_TOOLTIP), items[i].isStackable, GUILayout.ExpandWidth(false));

                if(items[i].isStackable)
                {
                    items[i].stackAmount = EditorGUILayout.IntField(new GUIContent("Stack Amount", STACKABLE_AMOUNT_TOOLTIP), items[i].stackAmount, GUILayout.ExpandWidth(false));
                }
                else
                {
                    items[i].stackAmount = 1;
                }

                EditorGUILayout.EndVertical();

                ///Draw Icon select
                DrawIconSelection(i);

                

                EditorGUILayout.EndHorizontal();

                DrawStatButtons(i);

                EditorGUILayout.BeginVertical();

                DrawStats(i);

                EditorGUILayout.EndVertical();
                
                EditorGUILayout.Space(15f);
            }

            EditorGUILayout.Space(20f);
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Delete Item", GUILayout.MaxWidth(BUTTONS_SIZE)))
            {
                hiddenItems.RemoveAt(i);
                nameInuse.RemoveAt(i);
                nameViolation.RemoveAt(i);
                items.RemoveAt(i);
                cannotCompile = false;
                SaveData();
            }          

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.EndScrollView();
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

    private void DrawStatButtons(int index)
    {
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Add Stat", GUILayout.MaxWidth(BUTTONS_SIZE)))
        {
            AddVairable(index);
        }
       
        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = Color.white;
    }
    
    private void DrawStats(int i)
    {
        for (int j = 0; j < items[i].stats.Count; j++)
        {   
            EditorGUILayout.BeginHorizontal();            

            EditorGUILayout.LabelField("Stat Name", GUILayout.MaxWidth(75f));
            items[i].stats[j].statName = EditorGUILayout.TextField(items[i].stats[j].statName, GUILayout.ExpandWidth(false));

            EditorGUILayout.LabelField("Data", GUILayout.MaxWidth(40f));

            nameViolation[i][1] = !CheckNameViolation(items[i].stats[j].statName);

            ///Draw all types variable types
            switch (items[i].stats[j].index)
            {
                case 0:
                    int intData = ItemCreator.UpdateData<int>(items[i].stats[j].data, typeof(int));
                    items[i].stats[j].data = EditorGUILayout.IntField(intData);
                    items[i].stats[j].dataType = typeof(int);
                    break;

                case 1:
                    float floatData = ItemCreator.UpdateData<float>(items[i].stats[j].data, typeof(float));
                    items[i].stats[j].data = EditorGUILayout.FloatField(floatData);
                    items[i].stats[j].dataType = typeof(float);
                    break;

                case 2:
                    string stringData = ItemCreator.UpdateData<string>(items[i].stats[j].data, typeof(string));

                    string dataString = stringData;

                    if (dataString.Length < 25)
                        items[i].stats[j].data = EditorGUILayout.TextField(stringData);
                    else
                    {
                        GUILayout.BeginVertical();
                        EditorStyles.textField.wordWrap = true;
                        items[i].stats[j].data = EditorGUILayout.TextArea(stringData, GUILayout.MaxWidth(TEXT_AREA_SIZE), GUILayout.MinWidth(TEXT_AREA_SIZE));
                        GUILayout.EndVertical();
                    }

                    items[i].stats[j].dataType = typeof(string);
                    break;

                case 3:
                    bool booldata = ItemCreator.UpdateData<bool>(items[i].stats[j].data, typeof(bool));
                    items[i].stats[j].data = EditorGUILayout.Toggle(booldata);
                    items[i].stats[j].dataType = typeof(bool);
                    break;

            }


            items[i].stats[j].index = EditorGUILayout.Popup(items[i].stats[j].index, refsNames);

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Remove Stat", GUILayout.MaxWidth(BUTTONS_SIZE)))
                RemoveVariable(i);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }

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
        nameInuse.Add(false);
        nameViolation.Add(new bool[] { false, false });

        SaveData();
    }

    private void AddItem(CustomItem item)
    {
        items.Add(item);
        hiddenItems.Add(true);
        nameInuse.Add(false);
        nameViolation.Add(new bool[] { false, false });
    }

    private void AddVairable(int index)
    {
        items[index].stats.Add(new ItemVariables());
        items[index].stats[items[index].stats.Count - 1].index = 0;
        items[index].stats[items[index].stats.Count - 1].dataType = typeof(int);
        items[index].stats[items[index].stats.Count - 1].data = 0;

        SaveData();
    }

    private void RemoveVariable(int index)
    {
        items[index].stats.RemoveAt(items[index].stats.Count - 1);
        cannotCompile = false;

        SaveData();
    }

    private void CheckItemName(CustomItem selectedItem, int index)
    {
        int sameNameIndex = items.FindIndex(x => { return x.name == selectedItem.name && x.uid != selectedItem.uid; });

        if (sameNameIndex > -1)
        {
            nameInuse[index] = true;
            cannotCompile = true;
        }
        else
        {
            cannotCompile = false;
            nameInuse[index] = false;
        }

        for (int i = 0; i < nameViolation.Count; i++)
        {
            if(nameViolation[i][0] || nameViolation[i][1])
            {
                cannotCompile = true;
            }
        }
    }
    #endregion

    #region Internal Classes

    [System.Serializable]
    public class ItemVariables
    {
        public int index = 0;
        public string statName = string.Empty;
        public object data;
        public Type dataType;
        
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
        public List<ItemVariables> stats = new List<ItemVariables>();       
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

