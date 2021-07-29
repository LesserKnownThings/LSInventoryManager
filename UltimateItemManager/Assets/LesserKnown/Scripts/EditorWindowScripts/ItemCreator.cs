#if UNITY_EDITOR
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
    const string ITEMS_DATA_PATH = "items.text";
    const string VARS_DATA_PATH = "variables.text";



    public static T  GetData<T>(DataTypeEnum dataType)
    {
        T data = default;
        if (dataType == DataTypeEnum.Items)
        {
            data = FileManager.GetData<T>(folderPath, ITEMS_DATA_PATH);
        }else if (dataType == DataTypeEnum.Variables)
        {
            data = FileManager.GetData<T>(folderPath, VARS_DATA_PATH);
        }

        return data;
    }

    


  
    public static void SaveItemData()
    {
        FileManager.RegisterDataToFile(folderPath, ITEMS_DATA_PATH, ItemCreatorWindow.items);
    }
    
    public static void SaveVarData()
    {
        FileManager.RegisterDataToFile(folderPath, VARS_DATA_PATH, ItemCreatorWindow.variables);
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

public enum DataTypeEnum
{
    Items,
    Variables
}



#endif