using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DynamicClassCreator
{
    const string folderPath = "Assets/LesserKnown/Resources/Generated/Dynamic/";
    const string templatePath = "Editor/DynamicClassTemplate";
    const string generatedClassPath = "DynamicData.cs";

    
    private static string CreateDynamicClass()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(templatePath);

        TemplateSystem template = new TemplateSystem(textAsset.text);

        for (int i = 0; i < ItemCreatorWindow.items.Count; i++)
            template.AddVariable(PrepareTemplateData(ItemCreatorWindow.items[i]));       
       

        return template.ParseData();
    }

    private static TemplateItemData PrepareTemplateData(ItemCreatorWindow.Items item)
    {

        Dictionary<string, object> templateDict = new Dictionary<string, object>();

        foreach (var stat in item.stats)
        {
            templateDict.Add(stat.statName, stat.data);
        }

        TemplateItemData templateData = 
            new TemplateItemData
            (
                item.name,
                templateDict
                );

        return templateData;
    }

    public static void Compile()
    {          

        string data = CreateDynamicClass();
        FileManager.RegisterClassToFile($"{folderPath}{generatedClassPath}", data);
    }
}

