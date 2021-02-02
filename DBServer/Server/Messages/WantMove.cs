using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBServer.Server.Messages
{
    class WantMove
    {
        public int wantMoveFromX { get; set; }
        public int wantMoveFromY { get; set; }

        public WantMove(int wantMoveFromX, int wantMoveFromY)
        {
            this.wantMoveFromX = wantMoveFromX;
            this.wantMoveFromY = wantMoveFromY;
        }
    }
}
