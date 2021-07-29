#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


public class VarPickerPopup : PopupWindowContent
{

    private const string WINDOW_NAME = "Variable Picker";
    private EditorWindow currentWindow;

    private Vector2 scrollPos;
    private int index = -1;

    private List<CustomVariable> availableVariables = new List<CustomVariable>();

    public VarPickerPopup(int index)
    {
        this.index = index;
        SetAvailableVariableList();
    }

    private void SetAvailableVariableList()
    {
        List<ItemCreatorWindow.ItemVariables> cloneVars = new List<ItemCreatorWindow.ItemVariables>(ItemCreatorWindow.items[index].addedVariables);
        List<ItemCreatorWindow.ItemVariables> reworkedList = new List<ItemCreatorWindow.ItemVariables>(ItemCreatorWindow.variables);

        for (int i = 0; i < cloneVars.Count; i++)
        {
            int currentIndex = reworkedList.FindIndex(x => { return x.uid == cloneVars[i].uid || x.varName == cloneVars[i].varName; });

            if(currentIndex > -1)
            {
                reworkedList.RemoveAt(currentIndex);
                --i;
            }
        }

        for (int i = 0; i < reworkedList.Count; i++)
        {
            availableVariables.Add(new CustomVariable(reworkedList[i]));
        }

    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, 150);
    }

    public override void OnGUI(Rect rect)
    {
        if (index == -1)
        {
            return;
        }

        GUILayout.Label(WINDOW_NAME, EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.Space(10);
        DrawVariableToggles();

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Add variables & Save"))
        {
            SaveData();   
        }        


        EditorGUILayout.EndVertical();
    }

    private void DrawVariableToggles()
    {
        for (int i = 0; i < availableVariables.Count; i++)
        {
            availableVariables[i].isTaken = EditorGUILayout.Toggle(availableVariables[i].itemVar.varName, availableVariables[i].isTaken);
        }
    }

    private void SaveData()
    {
        foreach (var variable in availableVariables)
        {
            if(variable.isTaken)
            {
                ItemCreatorWindow.items[index].addedVariables.Add(variable.itemVar);
            }
        }

        ItemCreatorWindow.Instance.SaveVariableChnage();

        editorWindow.Close();
    }

    public override void OnOpen()
    {

    }

    public override void OnClose()
    {

    }

    internal class CustomVariable
    {
        public ItemCreatorWindow.ItemVariables itemVar { get; private set; }
        public bool isTaken = false;

        public CustomVariable(ItemCreatorWindow.ItemVariables itemVar)
        {
            this.itemVar = itemVar;
            isTaken = false;
        }
    }


}

#endif

