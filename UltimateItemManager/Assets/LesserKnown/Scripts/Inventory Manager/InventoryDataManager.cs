﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class InventoryDataManager
{
    public List<CellDataManager> cells = new List<CellDataManager>();
    public CellDataManager transitionCell;
}
