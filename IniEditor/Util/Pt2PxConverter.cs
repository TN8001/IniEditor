using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IniEditor.Util
{
    public class Pt2PxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double pt)
                return pt * (96.0 / 72.0);
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double px)
                return 96.0 / 72.0 / px;
            return DependencyProperty.UnsetValue;
        }
    }
}
