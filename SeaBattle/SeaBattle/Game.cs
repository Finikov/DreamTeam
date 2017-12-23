using System;
using System.Collections.Generic;
using System.Net;
using SeaBattleFramework;

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

        public Player Player1 = null;//new Player();
        public Player Player2 = null;//new Player();
        public Player CurrentTurn = null;

        public bool Complexity = false;

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
            field.RemoveShip(field.GridCells[p.X, p.Y].ShipId);
        }

        public ShotResult ShotPlayer(Point p, Guid peerId)
        {
            if (peerId != Player1.PeerId && peerId != Player2.PeerId)
                throw GameException.MakeExeption(GameErrorCode.InvalidSession, "Unkown player with peerId = " + peerId);

            Grid field = (peerId == Player1.PeerId) ? _player2Field : _player1Field;
            Grid fieldEnemy = (peerId == Player1.PeerId) ? _player1EnemyField : _player2EnemyField;
            

            var res = field.Shot(p, Complexity);
            switch (res)
            {
                case ShotResult.Hit:
                    fieldEnemy.GridCells[p.X, p.Y].State = GridState.Damaged;
                    break;
                case ShotResult.Kill:
                    fieldEnemy.GridCells[p.X, p.Y].State = GridState.Damaged;
                    Ship ship = field.FindShip(field.GridCells[p.X, p.Y].ShipId);
                    fieldEnemy.AddShip(ship);
                    if (!Complexity)
                        fieldEnemy.KillShipArea(ship);
                    break;
                case ShotResult.Miss:
                    fieldEnemy.GridCells[p.X, p.Y].State = GridState.Miss;
                    CurrentTurn = (peerId == Player1.PeerId) ? Player2 : Player1;
                    break;
            }
            return res;
        }
    }
}
