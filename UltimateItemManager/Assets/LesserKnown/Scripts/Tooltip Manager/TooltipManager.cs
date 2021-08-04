using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TooltipManager : MonoBehaviour
{
    #region Singleton
    private static TooltipManager instance;
    public static TooltipManager Instance
    {
        get { return instance; }
    }
    #endregion

    public GameObject tooltipBg;
    public TextMeshProUGUI mainText;

    [Space(10)]
    public GameObject varManagerGO;
    public Transform varManagerParent;

    private List<ToolTipVariableManager> variables = new List<ToolTipVariableManager>();

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        HideWindow();
    }

    

    public void ShowWindow(string data, List<VariablesData> receivingData)
    {        
        CreateVariables(receivingData);

        tooltipBg.SetActive(true);
        mainText.text = data;
    }

    public void HideWindow()
    {
        tooltipBg.SetActive(false);
        mainText.text = string.Empty;
        DestroyAnyVariables();
    }

    private void DestroyAnyVariables()
    {
        for (int i = 0; i < variables.Count; i++)
        {
            Destroy(variables[i].gameObject);
            variables.RemoveAt(i);
            i--;
        }
    }

    private void CreateVariables(List<VariablesData> receivingData)
    {
        for (int i = 0; i < receivingData.Count; i++)
        {
            GameObject clone = Instantiate(varManagerGO);
            clone.transform.SetParent(varManagerParent, false);
            ToolTipVariableManager cloneScript = clone.GetComponent<ToolTipVariableManager>();
            cloneScript.SetVariable(receivingData[i]);
            variables.Add(cloneScript);
        }
    }

}
