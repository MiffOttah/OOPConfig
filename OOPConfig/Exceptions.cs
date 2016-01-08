using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.OOPConfig
{
    public class OOPConfigSyntaxException : Exception
    {
        public OOPConfigSyntaxException() : base("Config file syntax error.") { }
    }
}
