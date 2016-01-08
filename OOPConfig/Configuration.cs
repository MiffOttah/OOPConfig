using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.OOPConfig
{
    /// <summary>
    /// The base class from which your configuration is inherited.
    /// </summary>
    public abstract class Configuration
    {
        public void Save(string filename)
        {
            using (var writer = new StreamWriter(File.Open(filename, FileMode.Create), Encoding.UTF8))
            {
                this.Save(writer);
            }
        }
        public void Save(TextWriter writer)
        {
            writer.NewLine = "\n";
            var thisType = this.GetType();

            // write class name
            writer.WriteLine("[{0}]", thisType.FullName);

            foreach (var property in thisType.GetProperties())
            {
                if (property.CanRead)
                {
                    TextEncoder.Encode(property.Name, writer);
                    writer.Write('=');

                    string serialized = TextEncoder.ObjToString(property.GetValue(this));
                    TextEncoder.Encode(serialized, writer);

                    writer.WriteLine();
                }
            }
        }

        public static object Load(string filename)
        {
            using (var reader = new StreamReader(File.Open(filename, FileMode.Open), Encoding.UTF8))
            {
                return Load(reader);
            }
        }

        private static object Load(StreamReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
