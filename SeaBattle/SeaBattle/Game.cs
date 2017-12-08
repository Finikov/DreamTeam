using System;
using System.Collections.Generic;
using System.Net;

namespace SeaBattle
{
    public enum ShotResult
    {
        Hit,
        Miss,
        Kill
    }

    public class Game
    {
        private Grid _player1Field = new Grid();
        private Grid _player2Field = new Grid();
        private Grid _player1EnemyField = new Grid();
        private Grid _player2EnemyField = new Grid();

        public Player Player1 = new Player();
        public Player Player2 = new Player();

        public Tuple<Grid,Grid> GetGridInfo(Guid peerId)
        {
            if (peerId == Player1.PeerId)
                return new Tuple<Grid,Grid>(_player1Field, _player1EnemyField);
            if (peerId == Player2.PeerId)
                return new Tuple<Grid, Grid>(_player2Field, _player2EnemyField);

            throw GameException.MakeExeption(GameErrorCode.InvalidSession, "Unkown player with peerId = "+peerId);
        }
        
        public void AddShip(Ship ship, Guid peerId)
        {
            if (peerId != Player1.PeerId && peerId != Player2.PeerId)
                throw GameException.MakeExeption(GameErrorCode.InvalidSession, "Unkown player with peerId = " + peerId);

            Grid field = (peerId == Player1.PeerId) ? _player1Field : _player2Field;
            field.AddShip(ship);
        }

        public void AddShip(ShipType shipType, List<Point> pos, Guid peerId)
        {
            if (peerId != Player1.PeerId && peerId != Player2.PeerId)
                throw GameException.MakeExeption(GameErrorCode.InvalidSession, "Unkown player with peerId = " + peerId);

            Grid field = (peerId == Player1.PeerId) ? _player1Field : _player2Field;
            field.AddShip(shipType, pos);
        }

        public void AutoFilling(Guid peerId)
        {
            if (peerId != Player1.PeerId && peerId != Player2.PeerId)
                throw GameException.MakeExeption(GameErrorCode.InvalidSession, "Unkown player with peerId = " + peerId);

            Grid field = (peerId == Player1.PeerId) ? _player1Field : _player2Field;
            field.AutoFilling();
        }

        public void PrintGrig(Guid peerId)
        {
            if (peerId != Player1.PeerId && peerId != Player2.PeerId)
                throw GameException.MakeExeption(GameErrorCode.InvalidSession, "Unkown player with peerId = " + peerId);

            Console.WriteLine("Field");
            Grid field = (peerId == Player1.PeerId) ? _player1Field : _player2Field;
            field.Pringrid();

            Console.WriteLine("Enemy Field");
            field = (peerId == Player1.PeerId) ? _player1EnemyField : _player2EnemyField;
            field.Pringrid();
        }

        //TODO: Подумать, что лучше передавать Ship или position
        public void RemoveShip(Point p, Guid peerId)
        {
            if (peerId != Player1.PeerId && peerId != Player2.PeerId)
                throw GameException.MakeExeption(GameErrorCode.InvalidSession, "Unkown player with peerId = " + peerId);

            Grid field = (peerId == Player1.PeerId) ? _player1Field : _player2Field;
            field.RemoveShip(field.grid[p.X, p.Y].ShipId);
        }

        public ShotResult ShotPlayer(Point p, Guid peerId)
        {
            if (peerId != Player1.PeerId && peerId != Player2.PeerId)
                throw GameException.MakeExeption(GameErrorCode.InvalidSession, "Unkown player with peerId = " + peerId);

            Grid field = (peerId == Player1.PeerId) ? _player2Field : _player1Field;
            Grid fieldEnemy = (peerId == Player1.PeerId) ? _player1EnemyField : _player2EnemyField;

            var res = field.Shot(p);
            switch (res)
            {
                case ShotResult.Hit:
                    fieldEnemy.grid[p.X, p.Y].State = GridState.Damaged;
                    break;
                case ShotResult.Kill:
                    fieldEnemy.grid[p.X, p.Y].State = GridState.Damaged;
                    Ship ship = field.FindShip(field.grid[p.X, p.Y].ShipId);
                    fieldEnemy.AddShip(ship);
                    fieldEnemy.KillShipArea(ship);
                    break;
                case ShotResult.Miss:
                    fieldEnemy.grid[p.X, p.Y].State = GridState.Miss;
                    break;
            }

            return res;
        }

        public ShotResult ShotPlayerLevlUp(Point p, Guid peerId)
        {
            if (peerId != Player1.PeerId && peerId != Player2.PeerId)
                throw GameException.MakeExeption(GameErrorCode.InvalidSession, "Unkown player with peerId = " + peerId);

            Grid field = (peerId == Player1.PeerId) ? _player2Field : _player1Field;
            Grid fieldEnemy = (peerId == Player1.PeerId) ? _player1EnemyField : _player2EnemyField;

            var res = field.ShotLevlUp(p);
            switch (res)
            {
                case ShotResult.Hit:
                    fieldEnemy.grid[p.X, p.Y].State = GridState.Damaged;
                    break;
                case ShotResult.Kill:
                    fieldEnemy.grid[p.X, p.Y].State = GridState.Damaged;
                    Ship ship = field.FindShip(field.grid[p.X, p.Y].ShipId);
                    fieldEnemy.AddShip(ship);
                    break;
                case ShotResult.Miss:
                    fieldEnemy.grid[p.X, p.Y].State = GridState.Miss;
                    break;
            }

            return res;
        }
    }

    public class GameCtxProvider
    {
        private static GameCtxProvider _instance;
        private Dictionary<Guid, Game> _sessions = new Dictionary<Guid, Game>();

        public static GameCtxProvider Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameCtxProvider();
                return _instance;
            }
        }

        public Guid CreateNewSession()
        {
            Guid id = Guid.NewGuid();
            _sessions.Add(id, new Game());
            return id;
        }

        public void CloseSession(Guid id)
        {
            if (!_sessions.Remove(id))
                throw GameException.MakeExeption(GameErrorCode.InvalidSession, "There isn't game session with id:" + id);
        }

        public Game GetSession(Guid id)
        {
            if (!_sessions.TryGetValue(id, out Game session))
                throw GameException.MakeExeption(GameErrorCode.InvalidSession, "There isn't game session with id:" + id);

            return session;
        }
    }
}
