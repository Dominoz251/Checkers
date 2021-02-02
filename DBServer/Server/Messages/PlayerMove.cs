using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBServer.Server.Messages
{
    class PlayerMove
    {
        public int playerMoveFromX { get; set; }
        public int playerMoveFromY { get; set; }
        public int playerMoveToX { get; set; }
        public int playerMoveToY { get; set; }
    }
}
