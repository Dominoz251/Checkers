using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBServer.Server.Messages
{
    class PawnToDelete
    {
        public int pawnToDeleteX { get; set; }
        public int pawnToDeleteY { get; set; }

        public PawnToDelete(int pawnToDeleteX, int pawnToDeleteY)
        {
            this.pawnToDeleteX = pawnToDeleteX;
            this.pawnToDeleteY = pawnToDeleteY;
        }
    }
}
