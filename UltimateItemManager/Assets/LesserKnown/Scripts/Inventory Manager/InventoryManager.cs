using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class InventoryManager : MonoBehaviour
{
    private const string GENERATED_ITEMS_FOLDER = "Generated/Items";
    private const string PREFAB_FOLDER = "Generated/Prefab/ItemManager (UnityEngine.GameObject)";

    public static List<DynamicData> items = new List<DynamicData>();
    public List<CellDataManager> cells = new List<CellDataManager>();

    private GameObject inventoryUI;

    public bool onValueChange;
    public Settings settings = new Settings();

    public void Awake()
    {
        items = Resources.LoadAll(GENERATED_ITEMS_FOLDER, typeof(DynamicData)).Cast<DynamicData>().ToList();

#if UNITY_EDITOR
        if(UICreator.inventoryCanvas != null)
        {
            Destroy(UICreator.inventoryCanvas);
        }
#endif

        inventoryUI = (GameObject)Resources.Load(PREFAB_FOLDER, typeof(GameObject));

#if UNITY_EDITOR
        if(inventoryUI == null)
        {
            if(UICreator.inventoryCanvas == null)
            {
                UICreator.CreatePreview(settings);
                Debug.LogWarning("No preview created, creting one. Please create a prefab instead.");
            }
        }


#endif

    }

    


}



