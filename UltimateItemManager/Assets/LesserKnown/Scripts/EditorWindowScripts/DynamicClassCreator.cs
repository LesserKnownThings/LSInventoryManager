#if UNITY_EDITOR
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;



public static class DynamicClassCreator
{
    const string DYNAMIC_FOLDER_PATH = "Assets/LesserKnown/Resources/Generated/Dynamic/";
    const string ITEMS_FOLDER_PATH = "Assets/LesserKnown/Resources/Generated/Items";

    const string TEMPLATE_PATH = "Editor/DynamicClassTemplate";
    const string PARAM_TEMPLATE_PATH = "Editor/DynamicClassTemplateWithParam";
    const string generatedClassPath = "DynamicData.cs";

    const string NAME_VAR = "itemName";
    const string ICON_VAR = "itemIcon";


    private static string CreateDynamicClass(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);

        TemplateSystem template = new TemplateSystem(textAsset.text);

        for (int i = 0; i < ItemCreatorWindow.items.Count; i++)
            template.AddVariable(PrepareTemplateData());

        return template.ParseData();
    }

    

    private static TemplateItemData PrepareTemplateData()
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

    public static void Compile()
    {          

        string data = CreateDynamicClass(TEMPLATE_PATH);
        
       FileManager.RegisterClassToFile($"{DYNAMIC_FOLDER_PATH}{generatedClassPath}", data);

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

    private static void CreateScriptableObjects(ItemCreatorWindow.CustomItem item)
    {
        var obj = ScriptableObject.CreateInstance<DynamicData>();

        var fields = obj.GetType().GetFields();

        Sprite currentText = ItemCreator.GetItemIcon(item.itemIconDirectory);

        obj.itemIcon = currentText;
        obj.itemName = item.name;
        obj.isStackable = item.isStackable;
        obj.stackAmount = item.stackAmount;

        
        foreach (var field in fields)
        {
            int hasIndex = item.addedVariables.FindIndex(x => { return x.varName == field.Name; });

            if (hasIndex > -1)
            {     
                Type t = item.addedVariables[hasIndex].data.GetType();

                if (field.GetType() != t)
                {
                    item.addedVariables[hasIndex].data = Convert.ChangeType(item.addedVariables[hasIndex].data, item.addedVariables[hasIndex].dataType);
                }
                field.SetValue(obj, item.addedVariables[hasIndex].data);
            }
        }

        
        AssetDatabase.CreateAsset(obj, $"{ITEMS_FOLDER_PATH}/{item.name}.asset");        
    }

}


#endif