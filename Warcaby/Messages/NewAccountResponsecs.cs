using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warcaby.Messages
{
    class NewAccountResponsecs
    {
        public string newAccountResponse { get; set; }

        public NewAccountResponsecs(string newAccountResponse)
        {
            this.newAccountResponse = newAccountResponse;
        }
    }
}
