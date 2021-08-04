using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Text;
using System.Collections.Generic;

public class CellDataManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CellData cellData;
    public Image cellItemIcon;
    public TextMeshProUGUI stackAmountText;
    [HideInInspector]
    public GameObject stackAmountBG;
    
    [HideInInspector]
    public RectTransform rect;

    private DataToOmit dataToOmit = new DataToOmit();

    public void Setup(CellData cellData)
    {
        this.cellData = cellData;
        cellItemIcon.sprite = cellData.defaultIcon;
        this.cellData.dataUpdate += UpdateData;
    }

    private void Start()
    {
        if(cellData.dataUpdate == null)
        {
            cellData.dataUpdate += UpdateData;
        }
        

        if(cellData.isTransitCell)
        {
            gameObject.SetActive(false);
        }
    }


    private void Update()
    {
        if(cellData.cellType == CellType.TransitionCell && gameObject.activeSelf)
        {
            rect.position = Input.mousePosition;
        }
    }



    public void UpdateData()
    {
        if(cellData.isEmpty)
        {
            stackAmountBG.SetActive(false);
        }
        else
        {
            stackAmountBG.SetActive(true);
            if (cellData.itemData.isStackable)
            {
                stackAmountText.text = $"{cellData.stackAmount:0}";
            }
            else
            {
                stackAmountText.text = "1";
            }
        }

        if (cellData.GetSprite() == null)
        {
            cellItemIcon.sprite = cellData.defaultIcon;
        }
        else
        {
            cellItemIcon.sprite = cellData.GetSprite();
        }       
    }


    #region Tooltip 
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cellData.isEmpty)
        {
            return;
        }
        string data = ReworkData();
        List<VariablesData> varData = ReworkVariables();
        
        TooltipManager.Instance.ShowWindow(data, varData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideWindow();
    }

    private string ReworkData()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine(cellData.itemData.inGameName);
        sb.AppendLine("");
        sb.AppendLine(cellData.itemData.description);     

        return sb.ToString();
    }

    private List<VariablesData> ReworkVariables()
    {
        List<VariablesData> varData = new List<VariablesData>();

        var dataFields = cellData.itemData.GetType().GetFields();

        foreach (var fieldData in dataFields)
        {
            if (fieldData.FieldType.Equals(typeof(int)) || fieldData.FieldType.Equals(typeof(float)) || fieldData.FieldType.Equals(typeof(string)))
            {
                if (ContainsVariable(fieldData.Name))
                {
                    continue;
                }

                VariablesData cloneData;

                if (fieldData.GetType().Equals(typeof(float)))
                {
                    cloneData = new VariablesData(ReworkVariableName(fieldData.Name), $"{fieldData.GetValue(cellData.itemData):0.00}");
                }
                else
                {
                    cloneData = new VariablesData(ReworkVariableName(fieldData.Name), $"{fieldData.GetValue(cellData.itemData)}");
                }

                varData.Add(cloneData);
            }
        }

        return varData;
    }

    private bool ContainsVariable(string currentName)
    {
        var fields = dataToOmit.GetType().GetFields();

        foreach (var field in fields)
        {
            if (field.Name.Equals(currentName))
            {
                return true;
            }
        }

        return false;
    }

    private string ReworkVariableName(string variableName)
    {
        string data = string.Empty;

        for (int i = 0; i < variableName.Length; i++)
        {
            if(i == 0)
            {
                data += char.ToUpper(variableName[i]);
                continue;
            }

            if (char.IsUpper(variableName[i]))
            {
                data += $" {variableName[i]}";
                continue;
            }

            if (variableName[i].Equals('_'))
            {
                data += $" ";
                continue;
            }

            data += variableName[i];
        }

        return data;
    }

    internal class DataToOmit
    {
        public int stackAmount;
        public string itemName;
        public string inGameName;
        public string description;
    }
    #endregion
}
