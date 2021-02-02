using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBServer.Server.Messages
{
    class RankingPlayer
    {
        public string username { get; set; }
        public string rangingGameWon { get; set; }
        public string rankingGameLost { get; set; }

        public RankingPlayer(string username, string rangingGameWon, string rankingGameLost)
        {
            this.username = username;
            this.rangingGameWon = rangingGameWon;
            this.rankingGameLost = rankingGameLost;
        }
    }
}
