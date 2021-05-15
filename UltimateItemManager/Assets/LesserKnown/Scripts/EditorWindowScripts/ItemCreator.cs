using System.IO;
using UnityEditor;
using Newtonsoft.Json;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
public static class ItemCreator
{
    const string folderPath = "Assets/LesserKnown/ItemManager/";
    const string scriptableObjectsFolder = "Assets/Resources/Items";
    const string ASSET_FOLDER_PATH = "Assets/LesserKnown/Resources/Generated/Items/ItemsData.asset";
    const string informationFilePath = "data.text";



    public static List<ItemCreatorWindow.Items> GetData()
    {
        List<ItemCreatorWindow.Items> items = FileManager.GetData<List<ItemCreatorWindow.Items>>(folderPath, informationFilePath);


        if (items == null || items.Count == 0)
            return new List<ItemCreatorWindow.Items>();

        List <ItemCreatorWindow.Items> cloneItems = items;     


        foreach (var item in items)
        {
            foreach (var stat in item.stats)
            {
                Type type = stat.dataType;
                stat.data = Convert.ChangeType(stat.data, type);
            }
        }

        return items;
    }


  
    public static void SaveData()
    {
        //FileManager.RegisterDataToFile(folderPath, informationFilePath, ItemCreatorWindow.items);
        GenerateAssetData();
    }
    

    public static T UpdateData<T>(object data, Type nextType)
    {
        object emptyData = Convert.ChangeType(0, nextType);

        bool differentData = nextType != data.GetType();

        if (differentData)
            return (T)emptyData;               

        return (T)data;
    }


    public static ItemCreatorWindow.Icon SerializeTexture(Texture2D texture)
    {
        ItemCreatorWindow.Icon icon = new ItemCreatorWindow.Icon();

        icon.data = texture.GetRawTextureData();
        icon.width = texture.width;
        icon.height = texture.height;
        icon.format = texture.format;
               
        return icon;
    }



    public static Texture2D DeserializeTexture(ItemCreatorWindow.Icon icon)
    {
        Texture2D texture = new Texture2D(icon.width, icon.height, icon.format, false);
        texture.LoadRawTextureData(icon.data);
        texture.Apply();
        return texture;
    }

    public static void GenerateAssetData()
    {
        ItemsAssets itemsAsset = ScriptableObject.CreateInstance<ItemsAssets>();
        itemsAsset.itemsData = ItemCreatorWindow.items;
        AssetDatabase.CreateAsset(itemsAsset, ASSET_FOLDER_PATH);
        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();
    }
}

[CreateAssetMenu(fileName = "Item", menuName = "Lesser/Dynamic")]
public class ItemsAssets:ScriptableObject
{
    [HideInInspector]
    public List<ItemCreatorWindow.Items> itemsData;
}




