using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace ResourceMigrator
{
    public static class Extensions
    {
        public static string ToEscapedString(this string input)
        {
            var literal = new StringBuilder(input.Length + 2);
            foreach (var c in input)
            {
                switch (c)
                {
                    case '\'': literal.Append(@"\'"); break;
                    case '\"': literal.Append("\\\""); break;
                    case '\\': literal.Append(@"\\"); break;
                    case '\0': literal.Append(@"\0"); break;
                    case '\a': literal.Append(@"\a"); break;
                    case '\b': literal.Append(@"\b"); break;
                    case '\f': literal.Append(@"\f"); break;
                    case '\n': literal.Append(@"\n"); break;
                    case '\r': literal.Append(@"\r"); break;
                    case '\t': literal.Append(@"\t"); break;
                    case '\v': literal.Append(@"\v"); break;
                    default:
                        if (Char.GetUnicodeCategory(c) != UnicodeCategory.Control)
                        {
                            literal.Append(c);
                        }
                        else
                        {
                            literal.Append(@"\u");
                            literal.Append(((ushort)c).ToString("x4"));
                        }
                        break;
                }
            }
            return literal.ToString();
        }

        public static Dictionary<string, string> LoadResources(this FileSystemInfo file)
        {
            if (file == null) throw new ArgumentNullException("file");
            var result = new Dictionary<string, string>();

            var doc = new XmlDocument();
            doc.Load(file.FullName);

            var nodes = doc.SelectNodes("//data");

            if (nodes != null)
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes == null) continue;
                    var name = node.Attributes["name"].Value;
                    var value = node.ChildNodes[1].InnerText;
                    result.Add(name, value);
                }

            return result;
        }

        public static string GetResourceType(this FileSystemInfo fileName)
        {
            switch (fileName.Name.ToLower().Substring(0, Math.Min(3, fileName.Name.Length)))
            {
                case "col":
                    return "color";
                case "boo":
                   return "bool";
                case "dim":
                   return "dimen";
                case "ite":
                   return "item";
                case "int":
                   return "integer";
                default:
                   return "string";
            }
        }
    }
}