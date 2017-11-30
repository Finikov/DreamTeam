using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SeaBattle;

// Здесь происходит отладка SeaBattle.dll

namespace SeaBattleServer.Controllers
{
    [RoutePrefix("Default")]
    public class DefaultController : ApiController
    {
        private Grid _grid = new Grid();

        [Route("Game")]
        [HttpGet()]
        public string Game()
        {
            try
            {
                List<Point> pos = new List<Point>();

                for(int i = 0; i < 1; i++)
                    pos.Add(new Point(i, 0));
                _grid.AddShip(ShipType.SingleDecker, pos);

                pos = new List<Point>();
                for (int i = 0; i < 3; i++)
                    pos.Add(new Point(i, 2));
                _grid.AddShip(ShipType.ThreeDecker, pos);

                return _grid.Shot(new Point(0, 0)).ToString();
            }
            catch (GameException exc)
            {
                return "Error text: " + exc.ErrorText + " Error code: " + (int)exc.Code;
            }
        }
    }
}
