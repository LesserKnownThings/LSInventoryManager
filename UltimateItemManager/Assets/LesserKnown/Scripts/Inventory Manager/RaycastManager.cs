using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastManager
{   
    private static RaycastManager instance;
    public static RaycastManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new RaycastManager();
            }
            return instance;
        }
    }

    private RaycastManager()
    {

    }

    public CellDataManager DetectCell()
    {
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        for (int i = 0; i < results.Count; i++)
        {
            CellDataManager cellManager = results[i].gameObject.GetComponent<CellDataManager>();

            if (cellManager == null)
            {
                results.RemoveAt(i);
                i--;
            }
            else if (cellManager.cellData.isTransitCell)
            {
                results.RemoveAt(i);
                i--;
            }
        }

        if (results.Count == 0)
            return null;

        return results[0].gameObject.GetComponent<CellDataManager>();

    }


}

