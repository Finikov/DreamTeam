using System;
using System.Collections;
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
        Unbroken,
        Damaged
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
                    _grid[i, j].State = GridState.Unbroken;
                }
        }

        public int AddShip(Ship ship, List<Point> pos)
        {
            if (_ships.Count >= _maxShips)
                return -1;
            if ((int) ship.Type != pos.Count)
                return -2;

            if (_checkArea(pos) != 0)
                return -3;
           
            _ships.Add(ship);
            foreach (Point point in pos)
                _grid[point.X, point.Y].Ship = ship;

            return 0;
        }


        private int _checkArea(List<Point> pos)
        {
            if (pos.Exists(a => a.X < 0 || a.Y < 0 || a.X > 10 || a.Y > 10)) //проверили: не выходит ли наша область за рамки поля
                return -1;

            var points = pos.OrderBy(a => a.X + a.Y).ToArray();
            Point first = points.First();
            Point last = points.Last();

            //далее учитываем, что могут быть клетки на границе области

            if ((first.X - 1) >= 0)
                --first.X;

            if ((first.Y - 1) >= 0)
                --first.Y;

            if ((last.X + 1) <= 10)
                ++last.X;

            if ((last.Y + 1) <= 10)
                ++last.Y;
            
            //проверяем свободность клеток

            for (int i = first.X; i <= last.X; i++)
                for (int j = first.Y; j <= last.Y; j++)
                {
                    if (_grid[i, j].Ship != null)
                        return -2;
                }

            return 0;
        }
    }
}
