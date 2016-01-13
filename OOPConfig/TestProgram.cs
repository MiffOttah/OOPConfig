using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MiffTheFox.OOPConfig
{
    internal class TestProgram
    {
        static void Main(string[] args)
        {
            //string testFile = Path.Combine(Path.GetTempPath(), "OOPConfig.txt");

            var r = new Random();

            var randomString = new StringBuilder();
            for (int i = 0; i < 128; i++)
            {
                randomString.Append((char)(r.Next(95) + 32));
            }

            var config1 = new MyConfig();
            config1.SomeBool = true;
            config1.SomeString = "  Hello; wörld!\r\n🐧\r\nLorem ipsum!  =\x00=  ";
            config1.SomeString2 = randomString.ToString();
            config1.SomeInt = r.Next(-9999, 9999);
            config1.SomeDouble = r.NextDouble();
            config1.SomeObject = new MySerObject { Message = "Hello, world!" };
            config1.SomeDateTime = new DateTime(1976, 7, 4, r.Next(24), r.Next(60), r.Next(60));
            config1.SomeDTOffset = DateTimeOffset.Now;
            config1.SomeTimeSpan = new TimeSpan(r.Next(4), r.Next(60), r.Next(60));
            config1.SomeEnum = (MyEnum)r.Next(4);

            config1.Save();

            var config2 = Configuration.Load<MyConfig>();
            Debug.Assert(config2.SomeBool == config1.SomeBool);
            Debug.Assert(config2.SomeString == config1.SomeString);
            Debug.Assert(config2.SomeString2 == config1.SomeString2);
            Debug.Assert(config2.SomeInt == config1.SomeInt);
            Debug.Assert(config2.SomeDouble == config1.SomeDouble);
            Debug.Assert(config2.SomeObject.Message == config1.SomeObject.Message);
            Debug.Assert(config2.SomeDateTime == config1.SomeDateTime);
            Debug.Assert(config2.SomeDTOffset == config1.SomeDTOffset);
            Debug.Assert(config2.SomeTimeSpan == config1.SomeTimeSpan);
            Debug.Assert(config2.SomeEnum == config1.SomeEnum);
        }

        class MyConfig : Configuration
        {
            public string SomeString { get; set; }
            public string SomeString2 { get; set; }
            public bool SomeBool { get; set; }
            public int SomeInt { get; set; }
            public double SomeDouble { get; set; }
            public MySerObject SomeObject { get; set; }

            public DateTime SomeDateTime { get; set; }
            public DateTimeOffset SomeDTOffset { get; set; }
            public TimeSpan SomeTimeSpan { get; set; }

            public MyEnum SomeEnum { get; set; }
        }

        [Serializable]
        class MySerObject
        {
            public string Message { get; set; }
        }

        enum MyEnum
        {
            Hearts,
            Diamonds,
            Spades,
            Clubs
        }
    }
}
