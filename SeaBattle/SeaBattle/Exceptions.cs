using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle
{
    enum ErrorCode
    {
        InvalidPosition,
        InvalidShip,
        RuleError,
    }

    class GameException : Exception
    {
        public ErrorCode Code { get; set; }
        public string ErrorText { get; set; }

        public static GameException MakeExeption(ErrorCode code, string errText)
        {
            GameException exc = new GameException
            {
                Code = code,
                ErrorText = errText
            };
            return exc;
        }
    }
}
