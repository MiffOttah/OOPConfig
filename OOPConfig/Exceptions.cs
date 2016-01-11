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
    public class OOPConfigCannotDeseralizeException : Exception
    {
        public OOPConfigCannotDeseralizeException() : base("This doesn't look like a seralized object.") { }
    }
}
