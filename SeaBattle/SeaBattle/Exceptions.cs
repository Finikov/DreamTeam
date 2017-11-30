using System;

namespace SeaBattle
{
    public enum GameErrorCode
    {
        InvalidPosition,
        InvalidShip,
        RuleError,
        InvalidSession,
    }

    public class GameException : Exception
    {
        public GameErrorCode ErrorCode { get; set; }
        public string ErrorText { get; set; }
        
        public static GameException MakeExeption(GameErrorCode code, string errText)
        {
            GameException exc = new GameException
            {
                ErrorCode = code,
                ErrorText = errText
            };
            return exc;
        }
    }
}
