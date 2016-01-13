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
    public class OOPConfigParseExecption : Exception
    {
        public OOPConfigParseExecption(Exception innerException) : base("Failed to parse configuration.", innerException) { }
    }
    public class OOPConfigUnsupportedTypeException : Exception
    {
        public OOPConfigUnsupportedTypeException(string typeName) : base(typeName + " cannot be saved by OOPConfig.") { }
        public OOPConfigUnsupportedTypeException(Type type) : this(type.FullName) { }
    }
}
