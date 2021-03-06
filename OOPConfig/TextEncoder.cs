﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MiffTheFox.OOPConfig
{
    /// <summary>
    /// Encodes and decodes strings for configuration.
    /// </summary>
    public static class TextEncoder
    {
        /// <summary>
        /// Prepares a string for insertion into a filename.
        /// </summary>
        public static string PrepareForFilename(string s)
        {
            const string ALLOWED_CHARS = "abcedefghijklmnopqrstuvwxyz0123456789";
            var sb = new StringBuilder();

            foreach (char c in s.ToLowerInvariant())
            {
                if (ALLOWED_CHARS.IndexOf(c) > -1)
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append('_');
                }
            }

            return sb.ToString();
        }

        public static void Encode(string s, TextWriter writer)
        {
            int i = 0;
            foreach (char c in s)
            {
                if (c == ' ')
                {
                    // encode only leading and trailing spaces
                    if (i > 0 && i < s.Length - 1)
                    {
                        writer.Write(' ');
                    }
                    else
                    {
                        writer.Write("%20");
                    }
                }
                else if ((c < ',' && c != '!') || c == '=' || c == '[' || c == ']' || c == ';') // takes care of % and other special chars, including control chars and newlines
                {
                    writer.Write('%');
                    writer.Write(((int)c).ToString("x2", CultureInfo.InvariantCulture));
                }
                else if (char.IsControl(c) || char.IsSeparator(c) || char.IsSurrogate(c) || char.IsWhiteSpace(c)) // encode other misc special characters
                {
                    writer.Write("%u");
                    writer.Write(((int)c).ToString("x4", CultureInfo.InvariantCulture));
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
            var rv = new StringBuilder();

            int originalLength = s.Length;
            s += "????"; // cheap and dirty way to prevent buffer overrun when parsing %

            for (int i = 0; i < originalLength; i++)
            {
                if (s[i] == '%')
                {
                    if (s[i + 1] == 'u')
                    {
                        string codepoint = s.Substring(i + 2, 4);
                        int n;
                        if (int.TryParse(codepoint, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out n))
                        {
                            rv.Append((char)n);
                        }
                        i += 5;
                    }
                    else
                    {
                        string codepoint = s.Substring(i + 1, 2);
                        int n;
                        if (int.TryParse(codepoint, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out n))
                        {
                            rv.Append((char)n);
                        }
                        i += 2;
                    }
                }
                else
                {
                    rv.Append(s[i]);
                }
            }

            return rv.ToString();
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
            else if (o is Enum)
            {
                return o.ToString();
            }
            else if (o.GetType().GetCustomAttributes(false).Any(attr => attr is SerializableAttribute))
            {
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, o);
                    return o.GetType().FullName + "!" + Convert.ToBase64String(ms.ToArray());
                }
            }
            else
            {
                throw new OOPConfigUnsupportedTypeException(o.GetType());
            }
        }

        public static object StringToObj(string str, Type destinationType)
        {
            try
            {
                if (destinationType == typeof(string))
                {
                    return str;
                }
                else if (destinationType == typeof(bool))
                {
                    return str.ToLowerInvariant() == "true";
                }

                // there's probably a better way to do this
                else if (destinationType == typeof(int))
                {
                    return int.Parse(str, CultureInfo.InvariantCulture);
                }
                else if (destinationType == typeof(uint))
                {
                    return uint.Parse(str, CultureInfo.InvariantCulture);
                }
                else if (destinationType == typeof(long))
                {
                    return long.Parse(str, CultureInfo.InvariantCulture);
                }
                else if (destinationType == typeof(ulong))
                {
                    return ulong.Parse(str, CultureInfo.InvariantCulture);
                }
                else if (destinationType == typeof(short))
                {
                    return short.Parse(str, CultureInfo.InvariantCulture);
                }
                else if (destinationType == typeof(ushort))
                {
                    return ushort.Parse(str, CultureInfo.InvariantCulture);
                }
                else if (destinationType == typeof(byte))
                {
                    return byte.Parse(str, CultureInfo.InvariantCulture);
                }
                else if (destinationType == typeof(sbyte))
                {
                    return sbyte.Parse(str, CultureInfo.InvariantCulture);
                }
                else if (destinationType == typeof(float))
                {
                    return float.Parse(str, CultureInfo.InvariantCulture);
                }
                else if (destinationType == typeof(double))
                {
                    return double.Parse(str, CultureInfo.InvariantCulture);
                }
                else if (destinationType == typeof(decimal))
                {
                    return decimal.Parse(str, CultureInfo.InvariantCulture);
                }
                else if (destinationType.IsEnum)
                {
                    return Enum.Parse(destinationType, str, false);
                }

                else if (destinationType.GetCustomAttributes(false).Any(attr => attr is SerializableAttribute))
                {
                    int ix = str.LastIndexOf('!');
                    if (ix == -1)
                        throw new OOPConfigCannotDeseralizeException();

                    string sourceType = str.Remove(ix);
                    string dataString = str.Substring(ix + 1);

                    if (sourceType != destinationType.FullName) throw new OOPConfigCannotDeseralizeException();

                    var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    byte[] data = Convert.FromBase64String(dataString);
                    using (var ms = new MemoryStream(data))
                    {
                        return bf.Deserialize(ms);
                    }
                }

                else
                {
                    // ignore any types that can't be deseralized
                    throw new OOPConfigUnsupportedTypeException(destinationType);
                }
            }
            catch (OOPConfigUnsupportedTypeException)
            {
                // don't wrap any OOPConfigUnsupportedTypeException
                throw;
            }
            catch (Exception ex)
            {
                // wrap all other exceptions
                throw new OOPConfigParseExecption(ex);
            }
        }
    }
}
