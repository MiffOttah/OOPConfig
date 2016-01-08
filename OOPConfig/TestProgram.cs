using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.OOPConfig
{
    internal class TestProgram
    {
        static void Main(string[] args)
        {
            string testFile = Path.Combine(Path.GetTempPath(), "OOPConfig.txt");
         
            var config1 = new MyConfig();
            config1.SomeBool = true;
            config1.SomeString = "  Hello, wörld! 🐧 Lorem ipsum!  ";
            config1.SomeInt = 117;
            config1.SomeDouble = 3.14;
            //config1.SomeObject = new MySerObject { Message = "Hello, world!" };

            config1.Save(testFile);

            var config2 = Configuration.Load<MyConfig>(testFile);
            Debug.Assert(config2.SomeBool == config1.SomeBool);
            Debug.Assert(config2.SomeString == config1.SomeString);
            Debug.Assert(config2.SomeInt == config1.SomeInt);
            Debug.Assert(config2.SomeDouble == config1.SomeDouble);
            //Debug.Assert(config2.SomeObject.Message == config1.SomeObject.Message);
        }

        class MyConfig : Configuration
        {
            public string SomeString { get; set; }
            public bool SomeBool { get; set; }
            public int SomeInt { get; set; }
            public double SomeDouble { get; set; }
            //public MySerObject SomeObject { get; set; }
        }

        [Serializable]
        class MySerObject
        {
            public string Message { get; set; }
        }
    }
}
