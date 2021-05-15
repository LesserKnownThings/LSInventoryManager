using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get { return instance; } }
    private static ItemManager instance;

    private ItemsAssets itemsData;

    public InventoryUIData uIData;

    private void Awake()
    {
        uIData.LoadData();

        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        itemsData = Resources.Load<ItemsAssets>("Generated/Items/ItemsData");
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
       
    }
}
