using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using System.Windows.Media;

namespace IniEditor.Models
{
    internal class SystemFontsModel
    {
        public static IReadOnlyCollection<FontFamily> SystemFonts => _SystemFonts ?? (_SystemFonts = GetSystemFonts());
        private static List<FontFamily> _SystemFonts;

        public static IReadOnlyCollection<string> SystemFontNames { get; private set; }

        private static List<FontFamily> GetSystemFonts()
        {
            var systemFonts = new List<FontFamily>();
            var jp = XmlLanguage.GetLanguage("ja-jp");
            var us = XmlLanguage.GetLanguage("en-us");

            foreach(var font in Fonts.SystemFontFamilies)
            {
                string name;
                if(font.FamilyNames.ContainsKey(jp))
                    name = font.FamilyNames[jp];
                else if(font.FamilyNames.ContainsKey(us))
                    name = font.FamilyNames[us];
                else
                    name = font.FamilyNames.First().Value;

                systemFonts.Add(new FontFamily(name));
            }

            systemFonts.Sort((x, y) => string.Compare(x.Source, y.Source, StringComparison.Ordinal));
            SystemFontNames = systemFonts.Select(x => x.Source).ToList();

            return systemFonts;
        }
    }
}
