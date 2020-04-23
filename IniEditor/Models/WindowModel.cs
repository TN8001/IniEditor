using IniEditor.Util;
using IniWrapper.Attribute;

namespace IniEditor.Models
{
    public class WindowModel : Observable
    {
        [IniOptions(Section = "Window")]
        public double Top { get => _Top; set => Set(ref _Top, value); }
        private double _Top;

        [IniOptions(Section = "Window")]
        public double Left { get => _Left; set => Set(ref _Left, value); }
        private double _Left;

        [IniOptions(Section = "Window")]
        public double Width { get => _Width; set => Set(ref _Width, value); }
        private double _Width;

        [IniOptions(Section = "Window")]
        public double Height { get => _Height; set => Set(ref _Height, value); }
        private double _Height;

        [IniOptions(Section = "Window")]
        public bool ShowLineNumbers { get => _ShowLineNumbers; set => Set(ref _ShowLineNumbers, value); }
        private bool _ShowLineNumbers;

        public WindowModel()
        {
            Top = double.NaN;
            Left = double.NaN;
            Width = 600;
            Height = 400;
            ShowLineNumbers = true;
        }
    }
}
