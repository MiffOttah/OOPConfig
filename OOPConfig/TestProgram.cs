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

            conf.Save(testFile);
            Process.Start(testFile); // open the resultant file in an editor
        }

        class MyConfig : Configuration
        {
            public string SomeString { get; set; }
            public bool SomeBool { get; set; }
            public int SomeInt { get; set; }   
        }
    }
}
