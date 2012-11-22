using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

using BombaJob.Utilities.Commands;

namespace BombaJob.Utilities.Converters
{
    public class HTMLtoFDConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var regEx = new Regex(@"\<a\s(href\=""|[^\>]+?\shref\="")(?<link>[^""]+)"".*?\>(?<text>.*?)(\<\/a\>|$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            FlowDocument flowDocument = new FlowDocument();
            string html = AppSettings.HyperlinkifyAll((string)value, "");

            int nextOffset = 0;
            foreach (Match match in regEx.Matches(html))
            {
                if (match.Index > nextOffset)
                {
                    AppendText(flowDocument, html.Substring(nextOffset, match.Index - nextOffset));
                    nextOffset = match.Index + match.Length;
                    AppendLink(flowDocument, match.Groups["text"].Value, new Uri(match.Groups["link"].Value));
                }
            }

            if (nextOffset < html.Length)
                AppendText(flowDocument, html.Substring(nextOffset));

            return flowDocument;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static void AppendText(FlowDocument fdoc, string text)
        {
            Paragraph paragraph;

            if (fdoc.Blocks.Count == 0 || (paragraph = fdoc.Blocks.LastBlock as Paragraph) == null)
            {
                paragraph = new Paragraph();
                fdoc.Blocks.Add(paragraph);
            }

            paragraph.Inlines.Add(new Run { Text = text });
        }

        public static void AppendLink(FlowDocument fdoc, string text, Uri uri)
        {
            Paragraph paragraph;

            if (fdoc.Blocks.Count == 0 || (paragraph = fdoc.Blocks.LastBlock as Paragraph) == null)
            {
                paragraph = new Paragraph();
                fdoc.Blocks.Add(paragraph);
            }
            var run = new Run { Text = text };
            var link = new Hyperlink { NavigateUri = uri, Foreground = fdoc.Foreground, Command = new HyperlinkCommand(uri) };

            link.Inlines.Add(run);
            paragraph.Inlines.Add(link);
        }
    }
}
