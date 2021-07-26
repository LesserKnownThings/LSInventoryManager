using UnityEngine;

[System.Serializable]
public class Settings
{
    public int leftPadding = 0;
    public int rightPadding = 0;
    public int topPadding = 0;
    public int bottomPadding = 0;
    public float horizontalSpacing = 0f;
    public float verticalSpacing = 0f;

    public float horizontalCellSize = 0f;
    public float verticalCellSize = 0f;
    public int cellNumber = 0;

    public Color bgColor;
    public Color slotBgColor;

    public Sprite slotBgSprite;
    public Sprite defaultItemSprite;
    public Sprite bgSprite;

    public Settings()
    {

    }

    public void ResetSettings()
    {
        leftPadding = 0;
        rightPadding = 0;
        topPadding = 0;
        bottomPadding = 0;
        horizontalSpacing = 0f;
        verticalSpacing = 0f;

        horizontalCellSize = 0f;
        verticalCellSize = 0f;
        cellNumber = 0;

        bgColor = new Color();
        slotBgColor = new Color();
    }
}