using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BombaJob.Utilities.Converters
{
    public class OfferDateShortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return AppSettings.DoShortDate((DateTime)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
