using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle
{
    enum ShipType
    {
        SingleDecker = 1,
        TwoDecker,
        ThreeDecker,
        FourDecker
    }

    enum ShipStatus
    {
        Full,
        Damaged,
        Destroyed
    }

    /*struct ShipCell
    {
        public Point Position;
    }*/

    class Ship
    {
        public int Id { get; set; }
        public string Name { get; set; } = "UnknownShip";               //добавить список имён и генерировать случайно из него
        public ShipType Type { get; set; }
        public ShipStatus Status { get; set; } = ShipStatus.Full;

        private int _health;

        //private ShipCell[] _cells;

        public ShipStatus Injury()
        {
           return Status = (--_health) <= 0 ? ShipStatus.Destroyed : ShipStatus.Damaged;
        }

        public Ship(ShipType type)
        {
            Type = type;
            //_cells = new ShipCell[(int)Type];
            _health = (int) Type;
        }
    }
}
