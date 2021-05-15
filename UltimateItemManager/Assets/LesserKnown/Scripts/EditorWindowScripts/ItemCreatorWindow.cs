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
    #region Size Variables
    const float BOTTOM_BUTTONS_SIZE = 35f;
    const float BUTTONS_SIZE = 85f;
    const float TEXT_AREA_SIZE = 225f;
    #endregion

    public Texture2D defaultIcon;

    private static ItemCreatorWindow instance;
    public static ItemCreatorWindow Instance { get { return instance; } }

    private Dictionary<object, string> allowedRefs;
    public static List<Items> items = new List<Items>();
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
        foreach (var item in ItemCreator.GetData())
        {
            AddItem(item);
        }
    }

  

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        GUI.contentColor = Color.yellow;
        GUILayout.Label("RPG Creator", EditorStyles.boldLabel);
        GUI.contentColor = Color.white;
        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = Color.white;

       


        DrawItems();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("New Item", GUILayout.MinHeight(BOTTOM_BUTTONS_SIZE), GUILayout.MaxWidth(250)))
            AddItem();

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Save Data", GUILayout.MinHeight(BOTTOM_BUTTONS_SIZE)))
            SaveData();
        
        /*
        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("Compile", GUILayout.MinHeight(BOTTOM_BUTTONS_SIZE)))
            ItemCreator.GenerateAssetData();*/
        GUILayout.EndHorizontal();
    }

    
    private void SaveData()
    {
        if (cannotCompile)
        {
            Debug.LogError("Cannot save, you must fix all errors before saving");
            return;
        }

        ItemCreator.SaveData();
    }


    private bool CheckNameViolation(string data)
    {
        bool isNameViolation = false;

        isNameViolation = reg.IsMatch(data);

        return isNameViolation;
    }

    private void DrawItems()
    {
        if (items.Count == 0 || items == null)
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
                EditorGUILayout.LabelField("Name", GUILayout.MaxWidth(110f));
                items[i].name = EditorGUILayout.TextField(items[i].name, GUILayout.MaxWidth(200f));


                nameViolation[i][0] = !CheckNameViolation(items[i].name);      


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
        Texture2D itemIcon = ItemCreator.DeserializeTexture(items[index].iconData);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Icon", GUILayout.MaxWidth(25f));

        itemIcon = (Texture2D)EditorGUILayout.ObjectField(itemIcon, typeof(Texture2D), allowSceneObjects: false);

        EditorGUILayout.EndHorizontal();

         items[index].iconData = ItemCreator.SerializeTexture(itemIcon);
    }

    private void DrawStatButtons(int index)
    {
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Add Stat", GUILayout.MaxWidth(BUTTONS_SIZE)))
            AddVairable(index);

        GUI.backgroundColor = Color.red;
        if (items[index].stats.Count > 0 && GUILayout.Button("Remove Stat", GUILayout.MaxWidth(BUTTONS_SIZE)))
            RemoveVariable(index);
        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = Color.white;
    }
    
    private void DrawStats(int i)
    {
        for (int j = 0; j < items[i].stats.Count; j++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Stat Name", GUILayout.MaxWidth(75f));
            items[i].stats[j].statName = EditorGUILayout.TextField(items[i].stats[j].statName);

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
            EditorGUILayout.EndHorizontal();
        }

    }



    #region Private API
    private void AddItem()
    {
        Items currentItem = new Items();
        currentItem.name = $"Item";

        currentItem.iconData = ItemCreator.SerializeTexture(defaultIcon);


        currentItem.uid = Guid.NewGuid().ToString();
        items.Add(currentItem);
        hiddenItems.Add(true);
        nameInuse.Add(false);
        nameViolation.Add(new bool[] { false, false });        
        SaveData();
    }

    private void AddItem(Items item)
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

    private void CheckItemName(Items selectedItem, int index)
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

    [System.Serializable]
    public class Icon
    {
        public int width;
        public int height;
        public byte[] data;
        public TextureFormat format;
    }

    

    [System.Serializable]    
    public class Items
    {
        [HideInInspector]
        public string name;
        [HideInInspector]
        public string uid;
        [HideInInspector]
        public Icon iconData;
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

