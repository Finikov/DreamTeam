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

    public enum GridState
    {
        Unbroken,
        Damaged,
        Miss
       // Destroyed
    }

    public struct GridCell
    {
        public int? ShipId;
        public GridState State;
    }

    public class Grid
    {
        private int _maxShips = 10;
        private List<Ship> _ships = new List<Ship>();
        public GridCell[,] grid = new GridCell[10, 10];

        public Grid()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    grid[i, j].ShipId = null;
                    grid[i, j].State = GridState.Unbroken;
                }
        }
        
        public void Pringrid()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (grid[j, i].State == GridState.Unbroken && grid[j, i].ShipId == null)
                    {
                        Console.Write("0");
                        continue;
                    }

                    if (grid[j, i].State == GridState.Miss)
                    {
                        Console.Write("*");
                        continue;
                    }

                    if (grid[j, i].State == GridState.Unbroken && grid[j, i].ShipId != null)
                    {
                        Console.Write("1");
                        continue;
                    }

                    if (grid[j, i].State == GridState.Damaged && grid[j, i].ShipId == null)
                    {
                        Console.Write("X");
                        continue;
                    }

                    if (FindShip(grid[j,i].ShipId).Status == ShipStatus.Destroyed)
                        Console.Write("=");
                    else
                        Console.Write("X");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }

        public Ship FindShip(int? shipId)
        {
            if (shipId < 0 || shipId > 99)
                throw GameException.MakeExeption(GameErrorCode.InvalidShip, "Try to remove the ship that does not exist.");
            int i = _ships.FindIndex(a => a.Id == shipId);

            if (i < 0 || i >= _ships.Count)
                throw GameException.MakeExeption(GameErrorCode.InvalidShip, "Ship was not found to remove.");

            return _ships[i];
        }

        public void AddShip(Ship ship)
        {
            if (ship.Position.Length <= 0)
                throw GameException.MakeExeption(GameErrorCode.InvalidPosition, "Invalid ship's position.");

            if (_ships.Count >= _maxShips)
                throw GameException.MakeExeption(GameErrorCode.RuleError, "The number of ships is maximum. Can't add one else.");

            if (_ships.FindAll(s => s.Type == ship.Type).Count + (int)ship.Type >= 5)
                throw GameException.MakeExeption(GameErrorCode.RuleError, "The number of ships of this type is maximum. Can't add one else.");

            var pos = ship.Position;
            if (pos.Length != 1)
                pos = pos.OrderBy(a => a.X + a.Y).ToArray();

            if (CheckArea(pos.ToList()) != 0)
                throw GameException.MakeExeption(GameErrorCode.InvalidPosition, "Invalid ship's position.");

            _ships.Add(ship);

            foreach (Point p in pos)
                grid[p.X, p.Y].ShipId = ship.Id;
        }

        public void AddShip(ShipType shipType, List<Point> pos)
        {
            if (pos == null)
                throw GameException.MakeExeption(GameErrorCode.InvalidPosition, "Invalid ship's position.");

            if ((int) shipType != pos.Count)
                throw GameException.MakeExeption(GameErrorCode.InvalidShip, "Invalid ship's settings.");

            if (_ships.Count >= _maxShips)
                throw GameException.MakeExeption(GameErrorCode.RuleError, "The number of ships is maximum. Can't add one else.");
            
            AddShip(new Ship(shipType, pos));
        }

        // TODO: Обдумать, какой аргумент лучше передавать: Id or Ship
        public void RemoveShip(int? shipId)
        {
            if(shipId == null)
                throw GameException.MakeExeption(GameErrorCode.InvalidShip, "Try to delete nonexistent ship");

            Ship ship = FindShip(shipId);

            foreach (Point point in ship.Position)
                grid[point.X, point.Y].ShipId = null;
                   
            _ships.Remove(ship);
        }

        public ShotResult ShotLevlUp(Point point)
        {
            if (point.X > 9 || point.X < 0 || point.Y > 9 || point.Y < 0)
                throw GameException.MakeExeption(GameErrorCode.InvalidPosition, "Shot point is off-field");

            GridCell cell = grid[point.X, point.Y];
            if (cell.State == GridState.Damaged || cell.State == GridState.Miss)
                throw GameException.MakeExeption(GameErrorCode.RuleError, "This point has already been shooted.");


            grid[point.X, point.Y].State = (cell.ShipId == null) ? GridState.Miss : GridState.Damaged;

            if (cell.ShipId != null)
            {
                var ship = _ships.Find(s => s.Id == cell.ShipId);
                if (ship.Injury() == ShipStatus.Damaged)
                    return ShotResult.Hit;
                return ShotResult.Kill;
            }
            return ShotResult.Miss;
        }

        //Добавлена обводка убитого корабля
        public ShotResult Shot(Point point)
        {
            if (point.X > 9 || point.X < 0 || point.Y > 9 || point.Y < 0)
                throw GameException.MakeExeption(GameErrorCode.InvalidPosition, "Shot point is off-field");

            GridCell cell = grid[point.X, point.Y];
            if (cell.State == GridState.Damaged || cell.State == GridState.Miss)
                throw GameException.MakeExeption(GameErrorCode.RuleError, "This point has already been shooted.");


            grid[point.X, point.Y].State = (cell.ShipId == null) ? GridState.Miss : GridState.Damaged;

            if (cell.ShipId != null)
            {
                var ship = _ships.Find(s => s.Id == cell.ShipId);
                if (ship.Injury() == ShipStatus.Destroyed)
                {
                    KillShipArea(ship);
                    return ShotResult.Kill;
                }
                return ShotResult.Hit;
                
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

            if ((last.X + 1) <= 9)
                ++last.X;

            if ((last.Y + 1) <= 9)
                ++last.Y;

            return Tuple.Create(first, last);
        }

        // TODO: Обдумать, какой аргумент лучше передавать: Id or Ship
        public void KillShipArea(Ship ship)
        {
            if (ship == null)
                throw GameException.MakeExeption(GameErrorCode.InvalidShip, "Ship was not found.");

            var points = GetShipArea(ship.Position);

            for (int k= points.Item1.X; k <= points.Item2.X; k++)
                 for (int j = points.Item1.Y; j <= points.Item2.Y; j++)
                     grid[k, j].State = (grid[k,j].ShipId == null) ? GridState.Miss : GridState.Damaged;

        }

        //Проверяет возможно ли поместить корабль
        private bool _isPossible(Point p, int type, int orientation)
        {
            return CheckArea(_positionCreat(p, type, orientation)) >= 0;
        }

        //Создает список из точек, занимаемых кораблем длины type 
        private List<Point> _positionCreat(Point p, int type, int orientation)
        {
            List<Point> pos = new List<Point>();

            for (int i = 0; i < type; i++)
                pos.Add(new Point(p.X + i * orientation, p.Y + i * (1-orientation)));

            return pos;
        }
        //Удаляет точки корабля из числа свободных 
        private List<Point> _occupyArea(List<Point>  list, List<Point> pos)
        {
            foreach (Point point in pos)
            {
                var index = list.FindIndex(a => a.X == point.X && a.Y == point.Y);
                list.RemoveAt(index);
            }

            return list;
        }

        public void AutoFilling()
        {
            List<ShipType> ships = new List<ShipType>() { ShipType.FourDecker,
                                                        ShipType.ThreeDecker, ShipType.ThreeDecker,
                                                        ShipType.TwoDecker, ShipType.TwoDecker, ShipType.TwoDecker,
                                                        ShipType.SingleDecker, ShipType.SingleDecker, ShipType.SingleDecker, ShipType.SingleDecker };
   
            Random ran = new Random();
            List<Point> freepoints = new List<Point>();

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    freepoints.Add(new Point(j, i));

            foreach (ShipType type in ships )
            {
                while (true)
                {
                    int orientation = ran.Next(2); //0 - вертикально, 1 - горизонтально
                    int index = ran.Next(freepoints.Count);
                    List<Point> pos;

                    if (_isPossible(freepoints[index], (int)type, orientation))
                    {
                        pos = _positionCreat(freepoints[index], (int) type, orientation);
                        AddShip(type, pos);
                        freepoints = _occupyArea(freepoints, pos);
                        break;
                    }

                    orientation = (orientation == 0) ? 1 : 0;

                    if (_isPossible(freepoints[index], (int)type, orientation))
                    {
                        pos = _positionCreat(freepoints[index], (int)type, orientation);
                        AddShip(type, pos);
                        freepoints = _occupyArea(freepoints, pos);
                        break;
                    }
                    
                }
                
            }

        }

        //проверяет область на отсутствие кораблей
        private int CheckArea(List<Point> pos)
        {
            //проверили: не выходит ли наша область за рамки поля
            if (pos.Exists(a => a.X < 0 || a.Y < 0 || a.X > 9 || a.Y > 9))
                return -1;

            var points = GetShipArea(pos.ToArray());

            //проверяем свободность клеток
            for (int i = points.Item1.X; i <= points.Item2.X; i++)
                for (int j = points.Item1.Y; j <= points.Item2.Y; j++)
                    if (grid[i, j].ShipId != null)
                        return -1;
            return 0;
        }
    }
}
