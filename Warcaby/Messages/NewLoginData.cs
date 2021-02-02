using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warcaby.Messages
{ 
    class NewLoginData
    {
        public NewLoginData(string newLogin, string newPassword)
        {
            this.newLogin = newLogin;
            this.newPassword = newPassword;
        }

        public string newLogin { get; set; }
        public string newPassword { get; set; }
    }
}
