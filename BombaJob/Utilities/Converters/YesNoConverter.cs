using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BombaJob.Utilities.Converters
{
    public class YesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            try
            {
                if ((bool)value)
                    return Properties.Resources.yes;
                else
                    return Properties.Resources.no;
            }
            catch (Exception ex)
            {
                AppSettings.LogThis("YesNoConverter - " + ex.Message);
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
