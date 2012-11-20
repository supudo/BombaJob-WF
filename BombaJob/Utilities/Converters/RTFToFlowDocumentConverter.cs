using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;

using BombaJob.Utilities.HtmlToXaml;

namespace BombaJob.Utilities.Converters
{
    public class RTFToFlowDocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FlowDocument flowDocument = new FlowDocument();
            string xaml = HtmlToXamlConverter.ConvertHtmlToXaml(AppSettings.Hyperlinkify(value.ToString()), false);

            using (MemoryStream stream = new MemoryStream((new UTF8Encoding()).GetBytes(xaml)))
            {
                TextRange text = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                text.Load(stream, DataFormats.Xaml);
            }

            return flowDocument;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
