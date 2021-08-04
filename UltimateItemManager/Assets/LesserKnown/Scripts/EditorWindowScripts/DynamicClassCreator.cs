#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;



public static class DynamicClassCreator
{
    private const string DYNAMIC_FOLDER_PATH = "Assets/LesserKnown/Resources/Generated/Dynamic/";
    private const string ITEMS_FOLDER_PATH = "Assets/LesserKnown/Resources/Generated/Items";

    private const string CLASS_TEMPLATE_PATH = "Editor/DynamicClassTemplate";
    private const string ENUM_TEMPLATE_PATH = "Editor/DynamicEnumTemplate";

    private const string GENERATED_CLASS_PATH = "DynamicData.cs";
    private const string GENERATED_ENUM_PATH = "ItemCategoriesEnum.cs";

    private static string CreateDynamicData(string path, bool isClass)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);

        TemplateSystem template = new TemplateSystem(textAsset.text);

        if (isClass)
        {
            for (int i = 0; i < ItemCreatorWindow.items.Count; i++)
            {
                template.AddVariable(PrepareClassTemplate());
            }
        }

        return template.ParseData(isClass);
    }

  
    private static TemplateItemData PrepareClassTemplate()
    {        
        Dictionary<string, object> templateDict = new Dictionary<string, object>();
        
        
        foreach (var variable in ItemCreatorWindow.variables)
        {
            templateDict.Add(variable.varName, variable.data);
        }

        TemplateItemData templateData = 
            new TemplateItemData
            (
                templateDict
                );
        
        return templateData;
    }

    

    public static void CompileClass()
    {

        string data = CreateDynamicData(CLASS_TEMPLATE_PATH, true);

        FileManager.RegisterClassToFile($"{DYNAMIC_FOLDER_PATH}{GENERATED_CLASS_PATH}", data);

        AssetDatabase.Refresh();

        if (Directory.Exists(ITEMS_FOLDER_PATH))
        {
            Directory.Delete(ITEMS_FOLDER_PATH, true);
            Directory.CreateDirectory(ITEMS_FOLDER_PATH);
        }

        for (int i = 0; i < ItemCreatorWindow.items.Count; i++)
        {
            CreateScriptableObjects(ItemCreatorWindow.items[i]);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void CompileEnum()
    {
        string data = CreateDynamicData(ENUM_TEMPLATE_PATH, false);
        FileManager.RegisterClassToFile($"{DYNAMIC_FOLDER_PATH}{GENERATED_ENUM_PATH}", data);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void CreateScriptableObjects(ItemCreatorWindow.CustomItem item)
    {
        var obj = ScriptableObject.CreateInstance<DynamicData>();

        var fields = obj.GetType().GetFields();

        Sprite currentText = ItemCreator.GetItemIcon(item.itemIconDirectory);

        obj.itemIcon = currentText;
        obj.itemName = item.name;
        obj.isStackable = item.isStackable;
        obj.stackAmount = item.stackAmount;
        obj.inGameName = item.inGameName;
        obj.description = item.description;
        obj.category = item.category;

        
        foreach (var field in fields)
        {
            int hasIndex = item.addedVariables.FindIndex(x => { return x.varName == field.Name; });

            if (hasIndex > -1)
            {
                item.addedVariables[hasIndex].data = Convert.ChangeType(item.addedVariables[hasIndex].data, field.FieldType);
                
                field.SetValue(obj, item.addedVariables[hasIndex].data);
            }
        }

        
        AssetDatabase.CreateAsset(obj, $"{ITEMS_FOLDER_PATH}/{item.name}.asset");        
    }

}


#endif