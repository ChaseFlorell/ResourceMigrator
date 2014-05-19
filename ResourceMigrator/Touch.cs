﻿using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ResourceMigrator
{
    public static class Touch
    {
        private static string _targetDir;
        private static Dictionary<string, string> _strings;
        private static string _touchNameSpace;

        public static void WriteToTarget(FileInfo sourceFile, string targetDir, Dictionary<string, string> strings, string touchNameSpace)
        {
            _targetDir = targetDir;
            _strings = strings;
            _touchNameSpace = touchNameSpace;

            var resourceType = sourceFile.GetResourceType();
            if (resourceType == "color")
            {
                BuildColorResourceForTouch();
            }
        }

        private static void BuildColorResourceForTouch()
        {
            var builder = new StringBuilder();
            builder.Append(@"
#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:" + Helpers.GetAssemblyVersion() + @"
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;

namespace " + _touchNameSpace + @"
{
    internal static class CustomUIColor
    {

        private static MonoTouch.UIKit.UIColor FromHexString(string hexValue, float alpha = 1.0f)
        {
            var colorString = hexValue.Replace(""#"", """");
            if (alpha > 1.0f)
            {
                alpha = 1.0f;
            }
            else if (alpha < 0.0f)
            {
                alpha = 0.0f;
            }

            float red, green, blue;

            switch (colorString.Length)
            {
                case 3: // #RGB
                    {
                        red = Convert.ToInt32(string.Format(""{0}{0}"", colorString.Substring(0, 1)), 16) / 255f;
                        green = Convert.ToInt32(string.Format(""{0}{0}"", colorString.Substring(1, 1)), 16) / 255f;
                        blue = Convert.ToInt32(string.Format(""{0}{0}"", colorString.Substring(2, 1)), 16) / 255f;
                        return MonoTouch.UIKit.UIColor.FromRGBA(red, green, blue, alpha);
                    }
                case 6: // #RRGGBB
                    {
                        red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
                        green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
                        blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
                        return MonoTouch.UIKit.UIColor.FromRGBA(red, green, blue, alpha);
                    }

                default:
                    throw new ArgumentOutOfRangeException(string.Format(""Invalid color value {0} is invalid. It should be a hex value of the form #RBG, #RRGGBB"", hexValue));

            }
        }
");

            foreach (var key in _strings.Keys)
            {
                builder.Append("        ");
                builder.AppendLine(string.Format("public static MonoTouch.UIKit.UIColor {0} = FromHexString(\"{1}\");", key, _strings[key].ToEscapedString()));
            }

            builder.Append(@"    }
}
#pragma warning restore 1591");


            File.WriteAllText(Path.Combine(_targetDir, "CustomUIColor.cs"), builder.ToString());
        }
    }
}