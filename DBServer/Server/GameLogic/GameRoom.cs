using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DBServer.Server.GameLogic
{
    class GameRoom
    {
        public Player player1 { get; set; } //gracz z pionkami czerwonymi na dole
        public Player player2 { get; set; } //gracz z pionkami białymi na górze

        public Player turn { get; set; }

        public Pawn[,] chessboardState = new Pawn[8, 8];

        public List<Pawn> pawns = new List<Pawn>();

        public GameRoom(Player player1, Player player2)
        {
            this.player1 = player1;
            this.player2 = player2;
            this.chessboardState = createChesboard();
        }


        private Pawn[,] createChesboard()
        {
            Pawn[,] chesboard = new Pawn[8, 8];
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (y % 2 == 0 && x % 2 == 1 && y < 3)
                    {
                        Pawn pawn = new Pawn(x, y, "white");
                        pawns.Add(pawn);
                        chesboard[x, y] = pawn;
                    }
                    else if (y % 2 == 1 && x % 2 == 0 && y < 3)
                    {
                        Pawn pawn = new Pawn(x, y, "white");
                        pawns.Add(pawn);
                        chesboard[x, y] = pawn;
                    }
                    else if (y % 2 == 1 && x % 2 == 0 && y > 4)
                    {
                        Pawn pawn = new Pawn(x, y, "red");
                        pawns.Add(pawn);
                        chesboard[x, y] = pawn;
                    }
                    else if (y % 2 == 0 && x % 2 == 1 && y > 4)
                    {
                        Pawn pawn = new Pawn(x, y, "red");
                        pawns.Add(pawn);
                        chesboard[x, y] = pawn;
                    }
                }
            }
            return chesboard;
        }

    }
}
