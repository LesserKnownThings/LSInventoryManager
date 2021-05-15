using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(ItemManager))]
[CanEditMultipleObjects]
public class UICreatorCustomEditor:Editor
{
    private bool uISettingsON = false;
    private bool textSettingsON = false;
    private ItemManager iManager;

    public override void OnInspectorGUI()
    {
        iManager = (ItemManager)target;
        base.OnInspectorGUI();

        uISettingsON = EditorGUILayout.Foldout(uISettingsON, "UI Settings");
        textSettingsON = EditorGUILayout.Foldout(textSettingsON, "Text Settings");

        if(uISettingsON)        
            DisplayUISettings(iManager.uIData);


        if (textSettingsON)
            DisplayTextSettings(iManager.uIData);

        ClampData(iManager.uIData);

        if (!EditorApplication.isPlaying)
        {
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button("Create Preview"))
                iManager.uIData.CreateUI(iManager.transform);

            if (iManager.uIData.updatePreview)
                iManager.uIData.CreateUI(iManager.transform);

            GUI.backgroundColor = Color.yellow;
            if (iManager.uIData.currentUi != null && GUILayout.Button("Save Prefab"))
                SavePrefab();

            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();
        }
    }

    private void SavePrefab()
    {
        
    }

    private void DisplayUISettings(InventoryUIData data)
    {
        data.width = EditorGUILayout.IntField("Width", data.width);
        data.height = EditorGUILayout.IntField("Height", data.height);
        data.slotSize = EditorGUILayout.IntField("Slot Size", data.slotSize);
        data.leftPadding = EditorGUILayout.IntField("Left Padding", data.leftPadding);
        data.rightPadding = EditorGUILayout.IntField("Right Padding", data.rightPadding);
        data.topPadding = EditorGUILayout.IntField("Top Padding", data.topPadding);
        data.bottomPadding = EditorGUILayout.IntField("Bottom Padding", data.bottomPadding);
        data.xSpacing = EditorGUILayout.IntField("X Spacing", data.xSpacing);
        data.ySpacing = EditorGUILayout.IntField("Y Spacing", data.ySpacing);
    }

    private void DisplayTextSettings(InventoryUIData data)
    {
        data.textTopSpacing = EditorGUILayout.Slider("Text Space", data.textTopSpacing, 15f, 200f);
        data.textColor = EditorGUILayout.ColorField("Text Color", data.textColor);
    }

    private void ClampData(InventoryUIData data)
    {
        #region Even Numbers
        while (data.width % 2 != 0)
            data.width++;

        while (data.height % 2 != 0)
            data.height++;

        while (data.slotSize % 2 != 0)
            data.slotSize++;
        #endregion

        #region Possitive Numbers
        data.width = Mathf.Clamp(data.width, 0, int.MaxValue);
        data.height = Mathf.Clamp(data.height, 0, int.MaxValue);

        data.leftPadding = Mathf.Clamp(data.leftPadding, 0, int.MaxValue);
        data.rightPadding = Mathf.Clamp(data.rightPadding, 0, int.MaxValue);
        data.topPadding = Mathf.Clamp(data.topPadding, 0, int.MaxValue);
        data.bottomPadding = Mathf.Clamp(data.bottomPadding, 0, int.MaxValue);
        data.xSpacing = Mathf.Clamp(data.xSpacing, 0, int.MaxValue);
        data.ySpacing = Mathf.Clamp(data.ySpacing, 0, int.MaxValue);
        data.slotSize = Mathf.Clamp(data.slotSize, 0, int.MaxValue);

        data.textTopSpacing = Mathf.Clamp(data.textTopSpacing, 15f, 200f);
        #endregion
    }
}
#endif
