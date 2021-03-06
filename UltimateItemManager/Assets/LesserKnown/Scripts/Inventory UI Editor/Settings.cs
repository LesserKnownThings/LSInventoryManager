using UnityEngine;
using UnityEngine.UI;


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

    public float stackTextSize = 0f;
    public Vector2 stackCellTopLeftOffset = Vector2.zero;
    public Vector2 stackCellBottomRightOffset = Vector2.zero;

    public bool addEventSystem = true;
    public bool destroyOnThrow = false;

    public Color bgColor;
    public Color slotBgColor;
    public Color stackSlotTextColor;

    public Sprite slotBgSprite;
    public Sprite defaultItemSprite;
    public Sprite bgSprite;
    public Sprite closeButtonSprite;
    public Sprite stackCellSprite;

    public Sprite cellOptionsSprite;
    public Sprite cellOptionsButtonsSprite;
    public Sprite destroyWindowButtonsSprite;
    public Sprite destroyWindowBGSprite;

    public KeyCode uiOpenInput;

    public delegate void ButtonCloseDelegate();
    public ButtonCloseDelegate buttonClose;

    public Settings()
    {

    }

    public float GetInventoryWidth()
    {
        int multiplier = 1;
        int cellMult = 0;

        if (cellNumber > 10)
        {
            multiplier = cellNumber / 10;
        }

        switch (multiplier)
        {
            case 1:
                cellMult = 4;
                break;
            case 2:
                cellMult = 5;
                break;
            case 3:
                cellMult = 6;
                break;
            case 4:
                cellMult = 7;
                break;
        }

        return (cellMult * horizontalCellSize) + leftPadding + rightPadding;
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

        stackTextSize = 0f;
        stackCellTopLeftOffset = Vector2.zero;
        stackCellBottomRightOffset = Vector2.zero;

        addEventSystem = true;

        bgColor = new Color();
        slotBgColor = new Color();
        stackSlotTextColor = new Color();

        slotBgSprite = null;
        defaultItemSprite = null;
        bgSprite = null;
        closeButtonSprite = null;
        stackCellSprite = null;
        cellOptionsSprite = null;
    }
}