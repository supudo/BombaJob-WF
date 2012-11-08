using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BombaJob.Utilities.Converters
{
    public class OfferIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            try
            {
                if ((bool)value)
                    return AppSettings.IconHuman;
                else
                    return AppSettings.IconCompany;
            }
            catch (Exception ex)
            {
                AppSettings.LogThis("OfferIconConverter - " + ex.Message);
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
