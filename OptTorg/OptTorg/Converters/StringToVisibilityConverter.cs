using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OptTorg.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;

            // Если параметр "invert" - инвертируем
            if (parameter as string == "invert")
                return string.IsNullOrWhiteSpace(str) ? Visibility.Visible : Visibility.Collapsed;

            return string.IsNullOrWhiteSpace(str) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}