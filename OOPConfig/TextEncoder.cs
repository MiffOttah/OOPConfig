using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                        writer.Write('%');
                    }
                }
                else if (c < '/') // takes care of % and other special chars, including control chars and newlines
                {
                    writer.Write("%{0:2x}", (int)c);
                }
                else if (c == '=') // equals is special
                {
                    writer.Write("%3d");
                }
                else if (char.IsControl(c) || char.IsSeparator(c) || char.IsSurrogate(c) || char.IsWhiteSpace(c)) // encode other misc special characters
                {
                    writer.Write("%u{0:4x}", (int)c);
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
    }
}
