using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point()
        {
            X = 0;
            Y = 0;
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(Point p)
        {
            X = p.X;
            Y = p.Y;
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

    public class Grid
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

        public void AddShip(ShipType shipType, List<Point> pos)
        {
            if (pos == null)
                throw GameException.MakeExeption(ErrorCode.InvalidPosition, "Invalid ship's position.");

            if ((int) shipType != pos.Count)
                throw GameException.MakeExeption(ErrorCode.InvalidShip, "Invalid ship's settings.");

            if (_ships.Count >= _maxShips)
                throw GameException.MakeExeption(ErrorCode.RuleError, "The number of ships is maximum. Can't add one else.");

            if(_ships.FindAll(s => s.Type == shipType).Count + (int) shipType >= 5)
                throw GameException.MakeExeption(ErrorCode.RuleError, "The number of ships of this type is maximum. Can't add one else.");

            if (pos.Count != 1)
                pos = pos.OrderBy(a => a.X + a.Y).ToList();

            if (CheckArea(pos) != 0)
                throw GameException.MakeExeption(ErrorCode.InvalidPosition, "Invalid ship's position.");

            Ship ship = new Ship(shipType, pos);
            _ships.Add(ship);

            foreach (Point p in pos)
                _grid[p.X, p.Y].Ship = ship;
        }

        // TODO: Обдумать, какой аргумент лучше передавать: Id or Ship
        public void RemoveShip(Ship ship)
        {
            //if (shipId < 0 || shipId > 99)
            if(ship == null)
                throw GameException.MakeExeption(ErrorCode.InvalidShip, "Try to remove the ship that does not exist.");

            int i = _ships.FindIndex(a => a.Id == ship.Id);

            if (i < 0 || i >=_ships.Count)
                throw GameException.MakeExeption(ErrorCode.InvalidShip, "Ship was not found to remove.");

            foreach (Point point in _ships[i].Position)
                _grid[point.X, point.Y].Ship = null;
                   
            _ships.RemoveAt(i);
        }

        public ShotResult Shot(Point point)
        {
            if (point.X > 9 || point.X < 0 || point.Y > 9 || point.Y < 0)
                throw GameException.MakeExeption(ErrorCode.InvalidPosition, "Shot point is off-field");

            GridCell cell = _grid[point.X, point.Y];
            if (cell.State == GridState.Damaged)
                throw GameException.MakeExeption(ErrorCode.RuleError, "This point has already been shooted.");

            _grid[point.X, point.Y].State = GridState.Damaged;

            // TODO: добавить обводку убитого корабля с помощью KillShipArea в зависимости от настроек игры
            Ship ship;
            if ((ship = cell.Ship) != null)
            {
                if (ship.Injury() == ShipStatus.Damaged)
                    return ShotResult.Hit;
                return ShotResult.Kill;
            }
            return ShotResult.Miss;
        }


        /// <summary>
        /// Возвращает область вокруг заданной позиции(корабля)
        /// </summary>
        /// <param name="position">Отсортированный список позиций. 
        /// Первый элемент - позиция головы корбля, последний элмент - позиция хвоста</param>
        /// <returns>Возвращает кортеж из двух точек области вокруг корабля: первая - самая левая верхняя точка, 
        /// вторая - самая правая нижняя точка</returns>
        private static Tuple<Point, Point> GetShipArea(Point[] position)
        {
            Point first = new Point(position.First());
            Point last = new Point(position.Last());

            if ((first.X - 1) >= 0)
                --first.X;

            if ((first.Y - 1) >= 0)
                --first.Y;

            if ((last.X + 1) <= 10)
                ++last.X;

            if ((last.Y + 1) <= 10)
                ++last.Y;

            return Tuple.Create(first, last);
        }

        // TODO: Обдумать, какой аргумент лучше передавать: Id or Ship
        private void KillShipArea(Ship ship)
        {
            if (ship == null)
                throw GameException.MakeExeption(ErrorCode.InvalidShip, "Ship was not found.");

            var points = GetShipArea(ship.Position);

            for (int i = points.Item1.X; i <= points.Item2.X; i++)
                 for (int j = points.Item1.Y; j <= points.Item2.Y; j++)
                     _grid[i, j].State = GridState.Damaged;

        }

        //проверяет область на отсутствие кораблей
        private int CheckArea(List<Point> pos)
        {
            //проверили: не выходит ли наша область за рамки поля
            if (pos.Exists(a => a.X < 0 || a.Y < 0 || a.X > 10 || a.Y > 10))
                return -1;

            var points = GetShipArea(pos.ToArray());

            //проверяем свободность клеток
            for (int i = points.Item1.X; i <= points.Item2.X; i++)
                for (int j = points.Item1.Y; j <= points.Item2.Y; j++)
                    if (_grid[i, j].Ship != null)
                        return -1;
            return 0;
        }
    }
}
