using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBServer.Server.GameLogic
{
    class Pawn
    {
        public int pozX { get; set; }
        public int pozY { get; set; }
        public string color { get; set; }
        public bool priority { get; set; }

        public Pawn(int pozX, int pozY, string color)
        {
            this.pozX = pozX;
            this.pozY = pozY;
            this.color = color;
            this.priority = false;
        }
    }
}
