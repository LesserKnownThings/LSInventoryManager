using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Globalization;

public class TemplateSystem
{
    const string FOREVERY_LOOP = "FOREVERY";
    const string FOREVERY_LOOP_END = "ENDFOREVERY";
    const string STRUCT_LOOP = "STRUCT";
    const string STURCT_LOOP_END = "ENDSTRUCT";

    private string templateHolder;

  


    private List<TemplateItemData> itemsData = new List<TemplateItemData>();
    private bool inLoop;

    public TemplateSystem(string template)
    {
        templateHolder = template;
    }

    public void AddVariable(TemplateItemData templateItemData)
    {
        itemsData.Add(templateItemData);
    }

    public string ParseData()
    {
        List<string> lines = new List<string>(templateHolder.Split('\n'));
        StringBuilder strcutSb = new StringBuilder();
        StringBuilder realSb = new StringBuilder();

        string structs = string.Empty;

        for (int i = 0; i < lines.Count; i++)
        {
            string current = lines[i];

            if (current.Contains(FOREVERY_LOOP))
            {
                //Get the rest of the template
                while (!current.Contains(FOREVERY_LOOP_END))
                {
                    i++;
                    current = lines[i];

                    if (!current.Contains(FOREVERY_LOOP_END))
                        strcutSb.AppendLine(lines[i]);
                    else if(current.Contains(FOREVERY_LOOP_END))
                    {
                        string content = strcutSb.ToString();

                        if (content.Length > 0)
                            structs = BuildStructs(content);                    

                        realSb.AppendLine(structs);
                        string trimmed = realSb.ToString().TrimEnd('\n');
                        realSb = new StringBuilder(trimmed);                    
                    }
                }
            }
            else
                realSb.AppendLine(current.Trim('\n'));           
        }

        


        return realSb.ToString();
    }

    private string BuildStructs(string content)
    {
        List<string> lines = new List<string>(content.Split('\n'));

        StringBuilder sb = new StringBuilder();

        foreach (var data in itemsData)
        {            
            StringBuilder variabelSubString = new StringBuilder();
            for (int i = 0; i < lines.Count; i++)
            {
                string current = lines[i];

                if (!current.Contains(STRUCT_LOOP) && !current.Contains("structName"))
                    sb.AppendLine(current);

                if (current.Contains(STRUCT_LOOP))
                {
                    while(!current.Contains(STURCT_LOOP_END))
                    {
                        i++;
                        current = lines[i];

                        if (!current.Contains(STURCT_LOOP_END))
                            variabelSubString.AppendLine(current);
                    }

                    foreach (var variables in data.itemVariables)
                    {
                        sb.AppendLine(ParseData(variabelSubString.ToString(), variables));
                    }
                }

                int parseStart = current.IndexOf(">:");

                if (parseStart < 0)
                    continue;

                parseStart += 2;
                int endParse = current.IndexOf(":<");

                if (endParse < 0)
                    throw new Exception("A parse has ended before it begun");

                if (current.Contains("structName") && !current.Contains("varName"))
                    sb.AppendLine(ParseLine(current, parseStart, endParse, data, true));
                else if (current.Contains("structName") && current.Contains("varName"))
                    sb.AppendLine(ParseLine(current, parseStart, endParse, data, false));                   

            }
        }

        return sb.ToString();
    }

   
    private string ParseLine(string line, int parseStart, int endParse, TemplateItemData tempData, bool onlyStruct)
    {
        string subString = line.Substring(parseStart - 2, endParse - parseStart + 4);

        
        if (onlyStruct)
        {            
            return line.Replace(subString, $"{tempData.itemName.ToUpper()}Class");
        }
        else
        {
            return line.Replace(subString, $"{tempData.itemName.ToUpper()}Class {tempData.itemName}");
        }

    }

    private string ParseData(string content, KeyValuePair<string, object> varData)
    {
        string[] data = ParseObject(varData.Value);

        return content.Replace("[0]", data[0]).Replace("Name", varData.Key).Replace("[1]", data[1]).Trim('\n');
    }

    /// <summary>
    /// Returns an array of 2 strings
    /// [0] is the data type
    /// [1] is the data value
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private string[] ParseObject(object data)
    {
        string[] dataStr = new string[2];

        string objDataStr = data.ToString();

        if (data is float)
        {
            dataStr[0] = "float";
            objDataStr += "f";
        }
        else if (data is string)
        {
            dataStr[0] = "string";
            objDataStr = $"\"{objDataStr}\"";
        }
        else if (data is bool)
        {
            dataStr[0] = "bool";
            objDataStr = objDataStr.ToLower();
        }
        else
            dataStr[0] = "int";
        
        dataStr[1] = objDataStr;
        return dataStr;
    }

    /// <summary>
    /// Maybe will do in later version
    /// </summary>
    private void RecordStructTemplate()
    {

    }

}

public struct TemplateItemData
{
    public string itemName;
    public Dictionary<string, object> itemVariables;

    public TemplateItemData(string itemName, Dictionary<string, object> itemVariables)
    {
        this.itemName = itemName;
        this.itemVariables = itemVariables;
    }
}
