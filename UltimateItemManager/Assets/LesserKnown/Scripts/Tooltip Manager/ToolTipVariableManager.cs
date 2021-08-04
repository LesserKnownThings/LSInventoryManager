using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTipVariableManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI variableName;
    [SerializeField]
    private TextMeshProUGUI variableValue;

    public void SetVariable(VariablesData variable)
    {
        variableName.text = variable.varName;
        variableValue.text = variable.varData;
    }
}

public class VariablesData
{
    public string varName;
    public string varData;

    public VariablesData(string varName, string varData)
    {
        this.varData = varData;
        this.varName = varName;
    }
}