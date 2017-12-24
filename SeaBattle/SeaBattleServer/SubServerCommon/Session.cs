using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SeaBattle;
using SeaBattleFramework;

namespace SeaBattleServer.SubServerCommon
{
    public class Session
    {
        public Guid Id { get; set; }
        public GameStatus Status { get; set; }
        public Game Game { get; set; }
        public short CheckSendFinish { get; set; }

        public Session(Guid id)
        {
            Id = id;
            Status = GameStatus.Empty;
            CheckSendFinish = 1;
            Game = new Game();
        }

        public Guid CreateSession()
        {
            Id = Guid.NewGuid();
            Status = GameStatus.Empty;
            Game = new Game();
            return Id;
        }

        public void AddPlayer(Guid peerId)
        {
            Status = GameStatus.FindingOpponent;
            if (Game.Player1 == null)
            {
                Game.Player1 = new Player();
                Game.Player1.PeerId = peerId;
            }
            else if (Game.Player2 == null)
            {
                Game.Player2 = new Player();
                Game.Player2.PeerId = peerId;
            }

            if (Game.Player1 != null && Game.Player2 != null)
            {
                Random ran = new Random();
                if (ran.Next(2) == 0)
                    Game.CurrentTurn = Game.Player1;
                else
                    Game.CurrentTurn = Game.Player2;
                Status = GameStatus.Started;
            }
        }

        public void RemovePlayer(Guid peerId)
        {
            if (Game.Player1.PeerId == peerId)
            {
                Status = GameStatus.Finished;
                Game.Player1 = null;
            }
            else if (Game.Player2.PeerId == peerId)
            {
                Status = GameStatus.Finished;
                Game.Player2 = null;
            }
        }

        public void FinishSession()
        {
            if (CheckSendFinish == 0)
            {
                Game.Player1 = null;
                Game.Player2 = null;
                Status = GameStatus.Finished;
                Server.CloseSession(Id);
            }
            else
                CheckSendFinish -= 1;
        }

        public void CloseSession()
        {
            if (Status == GameStatus.Started)
                FinishSession();
            if (Status == GameStatus.FindingOpponent)
            {
                Status = GameStatus.Finished;
                Server.CloseSession(Id);
            }
                
        }
    }
}