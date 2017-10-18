using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle
{
    class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    enum GridState
    {
        Empty,
        Ship
    }

    struct GridCell
    {
        public object Ship;
        public GridState State;
    }

    class Grid
    {
        private int _maxShips = 10;
        private List<Ship> _ships = new List<Ship>();
        private GridCell[,] _grid = new GridCell[10, 10];

        public Grid()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    _grid[i, j].Ship = null;
                    _grid[i, j].State = GridState.Empty;
                }
        }

        public int AddShip(Ship ship, List<Point> pos)
        {
            if (_ships.Count >= _maxShips)
                return -1;
            if ((int) ship.Type != pos.Count)
                return -2;

            _ships.Add(ship);
            foreach (Point point in pos)
            {
                
                _grid[point.X, point.Y].Ship = ship;
                _grid[point.X, point.Y].State = GridState.Ship;
            }
            return 0;
        }

        private int _checkArea()
        {
            
        }
    }
}
