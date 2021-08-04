using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TemplateSystem
{
    const string FOREVERY_LOOP = "FOREVERY";
    const string FOREVERY_LOOP_END = "ENDFOREVERY";

    const string FOREVERY_PARAM_LOOP = "FOREVERY Param";
    const string FOREVERY_PARAM_LOOP_END = "ENDFOREVERY Param";

    const string FOREVERY_ASSIGN_LOOP = "FOREVERY Assign";
    const string FOREVERY_ASSIGN_LOOP_END = "ENDFOREVERY Assign";

    private string templateHolder;

   


    private List<TemplateItemData> itemsData = new List<TemplateItemData>();
    private Dictionary<string, object> totalVariables = new Dictionary<string, object>();
    

    public TemplateSystem(string template)
    {
        templateHolder = template;
    }

    public void AddVariable(TemplateItemData templateItemData)
    {
        itemsData.Add(templateItemData);
    }

    public string ParseData(bool isClass)
    {
        if (isClass)
        {
            SetTotalVariables();
        }

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
                        if (isClass)
                        {
                            realSb.AppendLine(ReworkLoopString(FOREVERY_LOOP, strcutSb, false));
                        }
                        else
                        {
                            realSb.AppendLine(ReworkEnumLoopString(FOREVERY_LOOP, strcutSb));
                        }

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
            foreach (var varData in data.variable)
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

    private string ReworkEnumLoopString(string loopType, StringBuilder strcutSb)
    {
        StringBuilder parsedData = new StringBuilder();

        for (int i = 0; i < ItemCreatorWindow.categories.Count; i++)
        {         

            if (i < ItemCreatorWindow.categories.Count - 1 && i != 0)
            {
                parsedData.AppendLine($"{ItemCreatorWindow.categories[i].data}={i},");
            }
            else
            {
                if (i == 0 && ItemCreatorWindow.categories.Count > 1)
                {
                    parsedData.AppendLine($"{ItemCreatorWindow.categories[i].data}={i},");
                }
                else
                {
                    parsedData.AppendLine($"{ItemCreatorWindow.categories[i].data}={i}");
                }
            }
        }

        string returnData = parsedData.ToString().Replace(FOREVERY_LOOP_END, "");

        return returnData;
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
}

public struct TemplateItemData
{
    public Dictionary<string, object> variable;

    public TemplateItemData(Dictionary<string, object> variable)
    {
        this.variable = variable;
    }
}
