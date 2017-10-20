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
        public Ship Ship;
        public GridState State;
    }

    class Grid
    {
        private int _maxShips = 10;
        private List<Tuple<Ship, List<Point>>> _ships = new List<Tuple<Ship, List<Point>>>();//Храним список кораблей в виде кортежей (корабль, список, занимаемых им ячеек)
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

        public void AddShip(Ship ship, List<Point> pos)
        {
            if (_ships.Count >= _maxShips)
                throw GameException.MakeExeption(ErrorCode.RuleError, "Maximum number of ships. Can't add one else.");

            if ((int) ship.Type != pos.Count)
                throw GameException.MakeExeption(ErrorCode.InvalidShip, "Invalid ship's settings.");

            if (_checkArea(pos) != 0)
                throw GameException.MakeExeption(ErrorCode.InvalidPosition, "Invalid ship's position.");

            _ships.Add(Tuple.Create(ship, pos));
            foreach (Point point in pos)
                _grid[point.X, point.Y].Ship = ship;
        }

        public void RemoveSHip(Ship ship)
        {
            if (ship == null)
                throw GameException.MakeExeption(ErrorCode.InvalidShip, "Try to remove the ship that does not exist.");

            int i = 0;
            while (i < _ships.Count && _ships[i].Item1 != ship)
                i++;

            if (i >=_ships.Count)
                throw GameException.MakeExeption(ErrorCode.InvalidShip, "Ship was not found to remove.");

            foreach (Point point in _ships[i].Item2)
                _grid[point.X, point.Y].Ship = null;
                   
            _ships.RemoveAt(i);
        }

        public ShotResult Shot(Point point)
        {
            GridCell cell = _grid[point.X, point.Y];
            if (cell.State == GridState.Damaged)
                throw GameException.MakeExeption(ErrorCode.RuleError, "This point have already been shooted.");

            _grid[point.X, point.Y].State = GridState.Damaged;

            Ship ship;
            if ((ship = cell.Ship) != null)
            {
                if (ship.Injury() == ShipStatus.Damaged)
                    return ShotResult.Hit;
                else
                    return ShotResult.Kill;
            }
            return ShotResult.Miss;
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
                    if (_grid[i, j].Ship != null)
                        return -1;
            return 0;
        }
    }
}
