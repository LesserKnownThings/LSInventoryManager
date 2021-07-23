using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(InventoryManager))]
[CanEditMultipleObjects]
public class InventoryUIEditor : Editor
{
    private readonly int[] MAX_MIN_PADDING = { 0, 200 };
    private readonly int[] MAX_MIN_CELL_SIZE = { 0, 90 };
    private readonly int[] MAX_MIN_CELL_AMOUNT = { 0, 40 };

    private bool showPadding;
    private InventoryManager mainScript;



    public override void OnInspectorGUI()
    {
        mainScript = (InventoryManager)target;
        GUI.color = Color.yellow;
        EditorGUILayout.LabelField("Settings");
        GUI.color = Color.white;

        mainScript.settings.cellNumber = EditorGUILayout.IntSlider("Cell Amount", mainScript.settings.cellNumber, MAX_MIN_CELL_AMOUNT[0], MAX_MIN_CELL_AMOUNT[1]);

        Vector2 cellSize = new Vector2(mainScript.settings.horizontalCellSize, mainScript.settings.verticalCellSize);
        cellSize = EditorGUILayout.Vector2Field("Cell Size", cellSize);
        mainScript.settings.horizontalCellSize = cellSize.x;
        mainScript.settings.verticalCellSize = cellSize.y;

        Vector2 spacing = new Vector2(mainScript.settings.horizontalSpacing, mainScript.settings.verticalSpacing);
        spacing = EditorGUILayout.Vector2Field("Cell Spacing", spacing);
        mainScript.settings.horizontalSpacing = spacing.x;
        mainScript.settings.verticalSpacing = spacing.y;

        mainScript.onValueChange = EditorGUILayout.Toggle("Update in Editor", mainScript.onValueChange);

        if (mainScript.onValueChange)
        {
            UICreator.CreatePreview(mainScript.settings);
        }

        DisplayPadding();
        DisplayButtons();
    }

    private void DisplayPadding()
    {
        showPadding = EditorGUILayout.Foldout(showPadding, "Padding");

        if (showPadding)
        {
            mainScript.settings.leftPadding = EditorGUILayout.IntSlider("Left Padding", mainScript.settings.leftPadding, MAX_MIN_PADDING[0], MAX_MIN_PADDING[1]);
            mainScript.settings.rightPadding = EditorGUILayout.IntSlider("Right Padding", mainScript.settings.rightPadding, MAX_MIN_PADDING[0], MAX_MIN_PADDING[1]);
            mainScript.settings.topPadding = EditorGUILayout.IntSlider("Top Padding", mainScript.settings.topPadding, MAX_MIN_PADDING[0], MAX_MIN_PADDING[1]);
            mainScript.settings.bottomPadding = EditorGUILayout.IntSlider("Bottom Padding", mainScript.settings.bottomPadding, MAX_MIN_PADDING[0], MAX_MIN_PADDING[1]);
        }
    }

    private void DisplayButtons()
    {
        GUI.backgroundColor = Color.yellow;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create UI Preview"))
        {
            UICreator.CreatePreview(mainScript.settings);
        }
        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("Reset Layout"))
        {
            mainScript.settings.ResetSettings();
        }
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
    }
}
#endif