using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;

namespace _9M.Work.WSVariable
{
    public class Variable : SoapHeader
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
