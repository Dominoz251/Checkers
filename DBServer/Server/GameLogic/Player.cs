using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace DBServer.Server.GameLogic
{
    class Player
    {
        public string login { get; set; }
        public Socket socket { get; set; }
        
        public string color { get; set; }

        public Player(string login, Socket socket)
        {
            this.login = login;
            this.socket = socket;
        }
    }
}
