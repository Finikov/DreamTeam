using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattleFramework
{
    public enum GridState
    {
        Unbroken,
        Damaged,
        Miss,
        Destroyed
    }

    public struct GridCell
    {
        public int? ShipId;
        public GridState State;
    }

    public interface IGrid
    {
        int MaxShips { get; set; }
        List<Ship> Ships { get; set; }
        GridCell[,] GridCells { get; set; }
    }
}
