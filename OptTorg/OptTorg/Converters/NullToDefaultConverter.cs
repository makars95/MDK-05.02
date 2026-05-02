using System;
using System.Globalization;
using System.Windows.Data;

namespace OptTorg.Converters
{
    public class NullToDefaultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DBNull.Value)
            {
                return parameter?.ToString() ?? "—";
            }

            if (value is string str && string.IsNullOrWhiteSpace(str))
            {
                return parameter?.ToString() ?? "—";
            }

            if (value is int intValue && intValue == 0)
            {
                return parameter?.ToString() ?? "Добавление";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}