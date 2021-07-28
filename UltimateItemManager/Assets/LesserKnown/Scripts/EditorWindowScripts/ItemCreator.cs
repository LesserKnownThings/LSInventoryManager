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
    const string informationFilePath = "data.text";



    public static List<ItemCreatorWindow.CustomItem> GetData()
    {
        List<ItemCreatorWindow.CustomItem> items = FileManager.GetData<List<ItemCreatorWindow.CustomItem>>(folderPath, informationFilePath);

        if (items == null)
        {
            return new List<ItemCreatorWindow.CustomItem>();
        }

        return items;
    }


  
    public static void SaveData()
    {
        FileManager.RegisterDataToFile(folderPath, informationFilePath, ItemCreatorWindow.items);
    }
    

    public static T UpdateData<T>(object data, Type nextType)
    {
        object emptyData = Convert.ChangeType(0, nextType);

        bool differentData = nextType != data.GetType();

        if (differentData)
            return (T)emptyData;               

        return (T)data;
    }

    public static Sprite GetItemIcon(string path)
    {
        return (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
    }


    public static ItemCreatorWindow.SerializedTexture SerializeTexture(Texture2D texture)
    {
        ItemCreatorWindow.SerializedTexture icon = new ItemCreatorWindow.SerializedTexture();

        if(texture == null)
        {
            return null;
        }
        
        icon.data = texture.GetRawTextureData();
        icon.width = texture.width;
        icon.height = texture.height;
        icon.format = texture.format;
               
        return icon;
    }



    public static Texture2D DeserializeTexture(ItemCreatorWindow.SerializedTexture serializedTexture)
    {
        if(serializedTexture == null)
        {
            return null;
        }

        Texture2D texture = new Texture2D(serializedTexture.width, serializedTexture.height, serializedTexture.format, false);
        texture.LoadRawTextureData(serializedTexture.data);
        texture.Apply();
        return texture;
    }

  
}





