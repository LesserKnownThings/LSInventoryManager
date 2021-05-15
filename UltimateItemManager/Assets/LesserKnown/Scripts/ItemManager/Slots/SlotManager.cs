using UnityEngine;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour
{
    public SlotData slotData;
    

}

public class SlotData
{
    public ItemCreatorWindow.Items slotItem = null;
    public int slotIndex = -1;
    public Image slotIcon;

    public SlotData(int slotIndex, Image slotIcon)
    {
        this.slotIndex = slotIndex;
        this.slotIcon = slotIcon;
    }
}

