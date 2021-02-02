using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warcaby.Messages
{
    class LoginData
    {
        public string login { get; set; }
        public string password { get; set; }

        public LoginData(string login, string password)
        {
            this.login = login;
            this.password = password;
        }
    }
}
