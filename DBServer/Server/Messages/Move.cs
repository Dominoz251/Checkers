﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace DBServer.Server.Messages
{
    class Move
    {

        public int fromX { get; set; }
        public int fromY { get; set; }
        public int toX { get; set; }
        public int toY { get; set; }

        public Move(int fromX, int fromY, int toX, int toY)
        {
            this.fromX = fromX;
            this.fromY = fromY;
            this.toX = toX;
            this.toY = toY;
        }
    }
}
