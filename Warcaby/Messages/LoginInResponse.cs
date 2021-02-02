using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warcaby.Messages
{
    class LoginInResponse
    {
        public string loginInReponse { get; set; }

        public LoginInResponse(string loginInReponse)
        {
            this.loginInReponse = loginInReponse;
        }
    }
}
