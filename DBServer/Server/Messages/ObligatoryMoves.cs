using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBServer.Server.Messages
{
    class ObligatoryMoves
    {
        public int obligatoryFromX { get; set; }
        public int obligatoryFromY { get; set; }
        public int obligatoryToX { get; set; }
        public int obligatoryToY { get; set; }

        public ObligatoryMoves(int obligatoryFromX, int obligatoryFromY, int obligatoryToX, int obligatoryToY)
        {
            this.obligatoryFromX = obligatoryFromX;
            this.obligatoryFromY = obligatoryFromY;
            this.obligatoryToX = obligatoryToX;
            this.obligatoryToY = obligatoryToY;
        }
    }
}
