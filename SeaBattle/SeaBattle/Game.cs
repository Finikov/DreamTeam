using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private Grid _playField1 = new Grid();
        private Grid _playField2 = new Grid();
        
        public Player Player1 = new Player();
        public Player Player2 = new Player();

        
    }
}
