using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.OOPConfig
{
    /// <summary>
    /// Encodes and decodes strings for configuration.
    /// </summary>
    public static class TextEncoder
    {
        public static void Encode(string s, TextWriter writer)
        {
            int i = 0;
            foreach (char c in s)
            {
                if (c == ' ')
                {
                    // encode only leading and trailing spaces
                    if (i > 0 && i < s.Length)
                    {
                        writer.Write(' ');
                    }
                    else
                    {
                        writer.Write("%20");
                    }
                }
                else if (c < ',') // takes care of % and other special chars, including control chars and newlines
                {
                    writer.Write("%{0:x2}", (int)c);
                }
                else if (c == '=') // equals is special
                {
                    writer.Write("%3d");
                }
                else if (char.IsControl(c) || char.IsSeparator(c) || char.IsSurrogate(c) || char.IsWhiteSpace(c)) // encode other misc special characters
                {
                    writer.Write("%u{0:x4}", (int)c);
                }
                else // everything else can be written as-is
                {
                    writer.Write(c);
                }

                i++;
            }
        }

        public static string Decode(string s)
        {
            // TODO: Decode encoded string
            throw new NotImplementedException();
        }

        public static string ObjToString(object o)
        {
            if (o is string)
            {
                return o.ToString();
            }
            else if (o is int || o is uint || o is long || o is ulong || o is short || o is ushort || o is byte || o is sbyte)
            {
                return ((IFormattable)o).ToString("g", CultureInfo.InvariantCulture);
            }
            else if (o is float || o is double)
            {
                return ((IFormattable)o).ToString("r", CultureInfo.InvariantCulture);
            }
            else if (o is decimal)
            {
                return ((decimal)o).ToString(CultureInfo.InvariantCulture);
            }
            else if (o is bool)
            {
                return ((bool)o) ? "true" : "false";
            }
            else if (o.GetType().GetCustomAttributes(false).Any(attr => attr is SerializableAttribute))
            {
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, o);
                    return "object:" + Convert.ToBase64String(ms.ToArray());
                }
            }
            else
            {
                return o.ToString();
            }
        }
    }
}
