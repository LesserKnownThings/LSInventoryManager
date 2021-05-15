using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

public static class FileManager
{
    const string templateStructFolderPath = "Assets/LesserKnown/Generated/Dynamic";
    const string templateStructFilePath = "/itemStrcutTemplate.data";


    public static void RegisterDataToFile(string folderPath, string filePath, object data)
    {
        string fullPath = Prerequisites(folderPath, filePath);

        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        using(FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            AddText(fs, jsonData);          
        }

    }

    public static void RegisterClassToFile(string fullPath, string data)
    {
        string trimmedData = Regex.Replace(data, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);


        using (StreamWriter sw = File.CreateText(fullPath))
        {
            sw.Write(trimmedData);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void AddText(FileStream fs, string value)
    {
        byte[] info = new UTF8Encoding(true).GetBytes(value);
        fs.Write(info, 0, info.Length);
    }

    public static  T GetData<T>(string folderPath, string filePath)
    {
        Prerequisites(folderPath, filePath);

        string fullPath = $"{folderPath}{filePath}";
        

        string data = string.Empty;

        using (FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Read))
        {
            data = GetData(fs, fullPath);
        }

        return JsonConvert.DeserializeObject<T>(data);
    }

    private static  string GetData(FileStream fs, string fullPath)
    {
        string data = string.Empty;
        byte[] b = File.ReadAllBytes(fullPath);
        UTF8Encoding temp = new UTF8Encoding(true);

        while (fs.Read(b, 0, b.Length) > 0)
            data += temp.GetString(b);

        return data;
    }

    private static string Prerequisites(string folderPath, string filePath)
    {
        bool folderExists = Directory.Exists(folderPath);

        if (!folderExists)
            Directory.CreateDirectory(folderPath);

        string fullPath = $"{folderPath}{filePath}";  

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();       

        return fullPath;
    }

}

[CreateAssetMenu(fileName ="some", menuName = "Lesser/Item")]
public class Asd:ScriptableObject
{

}
