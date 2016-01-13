using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MiffTheFox.OOPConfig
{
    /// <summary>
    /// The base class from which your configuration is inherited.
    /// </summary>
    public abstract class Configuration
    {
        private static string _GetConfigurationSavePathForAssembly(Assembly asm, Type t)
        {
            string baseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OOPConfig");

            string title = asm.GetCustomAttribute<AssemblyTitleAttribute>().Title;
            if (string.IsNullOrEmpty(title)) title = "_";

            string company = asm.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            if (string.IsNullOrEmpty(company)) company = "_";

            string guid = asm.GetCustomAttribute<GuidAttribute>().Value;
            if (string.IsNullOrEmpty(guid)) guid = "_";

            return Path.Combine(baseDirectory, TextEncoder.PrepareForFilename(company), TextEncoder.PrepareForFilename(title), TextEncoder.PrepareForFilename(guid), TextEncoder.PrepareForFilename(asm.GetName().Name) + ".oopconfig");
        }

        /// <summary>
        /// Saves the configuration to an automatically-determined location.
        /// </summary>
        public void Save()
        {
            Save(_GetConfigurationSavePathForAssembly(Assembly.GetCallingAssembly(), this.GetType()));
        }

        /// <summary>
        /// Saves the configuration to the path provided.
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            this.Save(filename, this.GetType().Name);
        }

        /// <summary>
        /// Saves the configuration to the TextWriter.
        /// </summary>
        /// <param name="writer"></param>
        public void Save(TextWriter writer)
        {
            this.Save(writer, this.GetType().Name);
        }

        /// <summary>
        /// Saves the configuration to the path provided.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="section"></param>
        public void Save(string filename, string section)
        {
            string directory = Path.GetDirectoryName(filename);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            using (var writer = new StreamWriter(File.Open(filename, FileMode.Create), Encoding.UTF8))
            {
                this.Save(writer, section);
            }
        }

        /// <summary>
        /// Saves the configuration to the TextWriter.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="section"></param>
        public void Save(TextWriter writer, string section)
        {
            writer.NewLine = "\n";
            var thisType = this.GetType();

            // write class name
            writer.Write('[');
            TextEncoder.Encode(section, writer);
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


        /// <summary>
        /// Loads the configuration from an automatically-determined location.
        /// </summary>
        public static T Load<T>() where T : Configuration, new()
        {
            string filename = _GetConfigurationSavePathForAssembly(Assembly.GetCallingAssembly(), typeof(T));
            if (File.Exists(filename))
            {
                return Load<T>(filename);
            }
            else
            {
                return new T();
            }
        }

        /// <summary>
        /// Loads configuration from a a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static T Load<T>(string filename) where T : Configuration, new()
        {
            return Load<T>(filename, typeof(T).Name);
        }

        /// <summary>
        /// Loads configuration from a TextReader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static T Load<T>(TextReader reader) where T : Configuration, new()
        {
            return Load<T>(reader, typeof(T).Name);
        }

        /// <summary>
        /// Loads the specified section of confiration from a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static T Load<T>(string filename, string section) where T : Configuration, new()
        {
            using (var reader = new StreamReader(File.Open(filename, FileMode.Open), Encoding.UTF8))
            {
                return Load<T>(reader, section);
            }
        }

        /// <summary>
        /// Loads the specified section of confiration from a TextReader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        private static T Load<T>(TextReader reader, string section) where T : Configuration, new()
        {
            string line;
            bool active = false;

            var thatType = typeof(T);
            var that = new T();

            while (reader.Peek() != -1)
            {
                line = reader.ReadLine().Trim();
                if (string.IsNullOrEmpty(line) || line[0] == ';') continue;

                if (line[0] == '[')
                {
                    string classname = TextEncoder.Decode(line.Substring(1, line.Length - 2));
                    active = classname == section;
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
