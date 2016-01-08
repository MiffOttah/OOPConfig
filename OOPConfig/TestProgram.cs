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
         
            var conf = new MyConfig();
            conf.SomeBool = true;
            conf.SomeString = "Hello, world! Lorem ipsum!";
            conf.SomeInt = 117;
            conf.SomeDouble = 3.14;
            conf.SomeObject = new MySerObject { Message = "Hello, world!" };

            conf.Save(testFile);

            var config = (MyConfig)Configuration.Load(testFile);
        }

        class MyConfig : Configuration
        {
            public string SomeString { get; set; }
            public bool SomeBool { get; set; }
            public int SomeInt { get; set; }
            public double SomeDouble { get; set; }
            public MySerObject SomeObject { get; set; }
        }

        [Serializable]
        class MySerObject
        {
            public string Message { get; set; }
        }
    }
}
