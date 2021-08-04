using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
public class InventoryManager : MonoBehaviour
{
    #region Constants
    private const string GENERATED_ITEMS_FOLDER = "Generated/Items";
    private const string PREFAB_FOLDER = "Generated/Prefab/ItemManager (UnityEngine.GameObject)";
    #endregion

    #region UI Creator Settings
    public bool onValueChange;
    public Settings settings = new Settings();
    #endregion

    public bool alphaReworkUI;

    public static List<DynamicData> items = new List<DynamicData>();

    private InGameInventoryManager inGameInvManager;

    public void Awake()
    {
        items = Resources.LoadAll(GENERATED_ITEMS_FOLDER, typeof(DynamicData)).Cast<DynamicData>().ToList();
        GenerateUI();        
    }

  
   
    private void GenerateUI()
    {
        GameObject prefab = (GameObject)Resources.Load(PREFAB_FOLDER, typeof(GameObject));
        GameObject inGamePrefab = Instantiate(prefab);
        inGameInvManager = inGamePrefab.GetComponent<InGameInventoryManager>();

#if UNITY_EDITOR
        if (UICreator.inventoryCanvas != null)
        {
            Destroy(UICreator.inventoryCanvas);
        }
#endif

    }
  



}



