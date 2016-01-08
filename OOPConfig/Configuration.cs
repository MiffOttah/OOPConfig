using System;
using System.Collections.Generic;
using System.Globalization;
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
            writer.Write('[');
            TextEncoder.Encode(thisType.FullName, writer);
            writer.Write(']');
            writer.WriteLine();

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

        public static T Load<T>(string filename) where T : Configuration, new()
        {
            using (var reader = new StreamReader(File.Open(filename, FileMode.Open), Encoding.UTF8))
            {
                return Load<T>(reader);
            }
        }

        private static T Load<T>(StreamReader reader) where T : Configuration, new()
        {
            string line;
            bool active = false;

            var thatType = typeof(T);
            var that = new T();

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine().Trim();
                if (string.IsNullOrEmpty(line) || line[0] == ';') continue;

                if (line[0] == '[')
                {
                    string classname = TextEncoder.Decode(line.Substring(1, line.Length - 2));
                    active = classname == thatType.FullName;
                }
                else if (active)
                {
                    int ix = line.IndexOf('=');
                    if (ix <= 0) throw new OOPConfigSyntaxException();

                    string key = TextEncoder.Decode(line.Remove(ix).TrimEnd(' ', '\t'));
                    string value = TextEncoder.Decode(line.Substring(ix + 1).TrimStart(' ', '\t'));

                    var prop = thatType.GetProperty(key);
                    if (prop != null)
                    {
                        prop.SetValue(that, TextEncoder.StringToObj(value, prop.PropertyType));
                    }
                }
            }

            return that;
        }
    }
}
