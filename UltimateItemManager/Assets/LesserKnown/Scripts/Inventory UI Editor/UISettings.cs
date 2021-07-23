using System.Collections.Generic;

[System.Serializable]
public class UISettings
{
    public int cellAmount { get; private set; }
    public List<CellDataManager> cellData { get; private set; }

}