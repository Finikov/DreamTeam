using System;
using System.Collections.Generic;

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
        public Grid PlayField1 = new Grid();
        public Grid PlayField2 = new Grid();
        
        public Player Player1 = new Player();
        public Player Player2 = new Player();

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
