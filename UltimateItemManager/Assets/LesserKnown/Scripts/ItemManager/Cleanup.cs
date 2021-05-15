using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class Cleanup
{
    static Cleanup()
    {
        GameObject inventoryUI = GameObject.FindGameObjectWithTag("Preview");

        Debug.Log(inventoryUI);

        if (inventoryUI != null)
            GameObject.DestroyImmediate(inventoryUI);
    }
}
