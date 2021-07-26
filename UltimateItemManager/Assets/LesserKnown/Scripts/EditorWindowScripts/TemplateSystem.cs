using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Globalization;
using System.Reflection;

public class TemplateSystem
{
    const string FOREVERY_LOOP = "FOREVERY";
    const string FOREVERY_LOOP_END = "ENDFOREVERY";

    const string FOREVERY_PARAM_LOOP = "FOREVERY Param";
    const string FOREVERY_PARAM_LOOP_END = "ENDFOREVERY Param";

    const string FOREVERY_ASSIGN_LOOP = "FOREVERY Assign";
    const string FOREVERY_ASSIGN_LOOP_END = "ENDFOREVERY Assign";

    const string FOREVERY_CLASS_LOOP = "FOREVERY Class";
    const string FOREVERY_CLASS_LOOP_END = "ENDFOREVERY Class";

    const string FILL = ">:FILL_PARAM:<";

    const string NONE = "";

    private string templateHolder;

   


    private List<TemplateItemData> itemsData = new List<TemplateItemData>();
    Dictionary<string, object> totalVariables = new Dictionary<string, object>();
    

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
        SetTotalVariables();

        List<string> lines = new List<string>(templateHolder.Split('\n'));

        StringBuilder strcutSb = new StringBuilder();

        StringBuilder realSb = new StringBuilder();

        
        for(int i=0; i < lines.Count;i++)
        {
            if (lines[i].Contains(FOREVERY_LOOP))
            {
                while (true)
                {
                    if (lines[i].Contains(FOREVERY_LOOP_END))
                    {
                        realSb.AppendLine(ReworkLoopString(FOREVERY_LOOP, strcutSb, false));
                        strcutSb.Clear();
                        break;
                    }
                    ++i;
                    strcutSb.AppendLine(lines[i]);
                }
            }
            else
                realSb.AppendLine(lines[i]);
            
        }


        return realSb.ToString();
    }



    private void SetTotalVariables()
    {
        foreach (var data in itemsData)
        {
            foreach (var varData in data.itemVariables)
            {
                if(!totalVariables.ContainsKey(varData.Key))
                {
                    totalVariables.Add(varData.Key, varData.Value);
                }
            }
        }
    }

    private string ReworkLoopString(string loopType,StringBuilder strcutSb, bool emptyParam)
    {
        StringBuilder parsedData = new StringBuilder();

        List<string> tempData = new List<string>();

        foreach (var data in itemsData)
        {
            int index = -1;
                       

            foreach (var keyValuePairData in totalVariables)
            {
                index++;

                if (!tempData.Contains(keyValuePairData.Key))
                {
                    tempData.Add(keyValuePairData.Key);

                    if (index < totalVariables.Count() - 1 && !loopType.Equals(FOREVERY_LOOP) && !loopType.Equals(FOREVERY_ASSIGN_LOOP))
                    {
                        parsedData.Append($"{ParseData(strcutSb.ToString(), keyValuePairData, loopType)},");
                    }
                    else
                    {
                        parsedData.Append(ParseData(strcutSb.ToString(), keyValuePairData, loopType));
                    }
                }
            }
        }       
        
        
        tempData.Clear();



        return parsedData.ToString();
    }

    private string ReworkFillClass( StringBuilder strcutSb, TemplateItemData data)
    {
        StringBuilder parsedData = new StringBuilder();
        string sbString = strcutSb.ToString();
        StringBuilder paramSb = new StringBuilder();

        parsedData.Append(sbString.Replace(">:className:<", data.itemName));

        int index = -1;

        foreach (var keyValuePairData in totalVariables)
        {
            index++;

            if(data.itemVariables.ContainsKey(keyValuePairData.Key))
            {
                KeyValuePair<string, object> currentKeyValuePairData = new KeyValuePair<string, object>(keyValuePairData.Key, data.itemVariables[keyValuePairData.Key]);

                if (index == totalVariables.Count() - 1)
                {
                    paramSb.AppendLine(ParseData("[1]", currentKeyValuePairData, NONE));
                }
                else 
                {
                    paramSb.AppendLine($"{ParseData("[1]", currentKeyValuePairData, NONE)},");
                }
            }
            else
            {
                string paramName = keyValuePairData.Key;
                object obj = keyValuePairData.Value;

                if (index == totalVariables.Count() - 1)
                {
                    paramSb.AppendLine(ParseVariable("[1]", paramName, obj));
                }
                else
                {
                    paramSb.AppendLine($"{ParseVariable("[1]", paramName, obj)},");
                }                    
            }
        }

        string parsedDataStr = parsedData.ToString();
        parsedDataStr = parsedDataStr.Replace(FILL, paramSb.ToString());        

        return parsedDataStr;
    }


    private string ReworkClassLoopString(StringBuilder strcutSb)
    {
        StringBuilder parsedData = new StringBuilder();
        StringBuilder realSb = new StringBuilder();

        parsedData.Append(strcutSb.ToString().Replace($">:{FOREVERY_CLASS_LOOP_END}:<", ""));


        foreach (var data in itemsData)
        {
            realSb.AppendLine(ReworkFillClass(parsedData, data));
        }

        return realSb.ToString();
    }
     

    private string ParseData(string content, KeyValuePair<string, object> varData, string loopType)
    {
        string dataTrim = content;

        switch (loopType)
        {
            case FOREVERY_LOOP:
                dataTrim = dataTrim.Replace($">:{FOREVERY_LOOP_END}:<", "");
                break;
            case FOREVERY_ASSIGN_LOOP:
                dataTrim = dataTrim.Replace($">:{FOREVERY_ASSIGN_LOOP_END}:<", "");
                break;
            case FOREVERY_PARAM_LOOP:
                dataTrim = dataTrim.Replace($">:{FOREVERY_PARAM_LOOP_END}:<", "");
                break;
        }

        string[] data = ParseObject(varData.Value, false);

        string returnData = string.Empty;

        
            returnData = dataTrim.Replace("[0]", data[0]).Replace("Name", varData.Key).Replace("[1]", data[1]).Trim('\n');
        

        return returnData;
    }

    private string ParseVariable(string content, string varName, object varData)
    {
        string returnData = string.Empty;
        string dataTrim = content;

        dataTrim = dataTrim.Replace($">:{FOREVERY_PARAM_LOOP_END}:<", "");

        string[] data = ParseObject(varData, true);

        Type intType = typeof(int);
            Type floatType = typeof(float);
            Type boolType = typeof(bool);

            if (varData.GetType().Equals(intType))
            {
                returnData = dataTrim.Replace("[0]", data[0]).Replace("Name", varName).Replace("[1]", "0");
            }
            else if (varData.GetType().Equals(floatType))
            {
                returnData = dataTrim.Replace("[0]", data[0]).Replace("Name", varName).Replace("[1]", "0f");
            }
            else if (varData.GetType().Equals(boolType))
            {
                returnData = dataTrim.Replace("[0]", data[0]).Replace("Name", varName).Replace("[1]", "false");
            }
            else
            {
                returnData = dataTrim.Replace("[0]", data[0]).Replace("Name", varName).Replace("[1]", data[1]).Trim('\n');
            }

        
        return returnData;
    }

    /// <summary>
    /// Returns an array of 2 strings
    /// [0] is the data type
    /// [1] is the data value
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private string[] ParseObject(object data, bool hasDefaultValue)
    {
        string[] dataStr = new string[2];

        string objDataStr = data.ToString();

        if(hasDefaultValue)
        {
            objDataStr = string.Empty;
        }


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
