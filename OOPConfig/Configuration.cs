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

                    // TODO: Encode value

                    writer.WriteLine();
                }
            }
        }
    }
}
