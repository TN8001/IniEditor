using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using IniEditor.Util;
using IniWrapper.Attribute;

namespace IniEditor.Models
{
    public class FontModel : Observable
    {
        [IniOptions(Section = "Font", Key = "Family")]
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<保留中>")]
        public string FamilyString
        {
            get => ConvertToString(Family);
            set { try { Family = ConvertFromString(Family, value); } catch { } }
        }

        [IniIgnore]
        public FontFamily Family { get => _Family; set => Set(ref _Family, value); }
        private FontFamily _Family;

        [IniOptions(Section = "Font")]
        public double Size { get => _Size; set => Set(ref _Size, value); }
        private double _Size;

        public FontModel()
        {
            Family = new FontFamily("メイリオ");
            Size = 11;
        }

        private static T ConvertFromString<T>(T target, string value)
            => (T)TypeDescriptor.GetConverter(target.GetType()).ConvertFrom(value);

        private static string ConvertToString<T>(T value)
            => (string)TypeDescriptor.GetConverter(value.GetType()).ConvertTo(value, typeof(string));
    }
}
