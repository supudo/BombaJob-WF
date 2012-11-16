using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BombaJob.Utilities.Extensions
{
    public static class RichTextBoxExtensions
    {
        public static void SetLinkedText(this RichTextBox richTextBox, string htmlFragment)
        {
            var regEx = new Regex(@"\<a\s(href\=""|[^\>]+?\shref\="")(?<link>[^""]+)"".*?\>(?<text>.*?)(\<\/a\>|$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            richTextBox.Document.Blocks.Clear();

            int nextOffset = 0;
            foreach (Match match in regEx.Matches(htmlFragment))
            {
                if (match.Index > nextOffset)
                {
                    richTextBox.AppendText(htmlFragment.Substring(nextOffset, match.Index - nextOffset));
                    nextOffset = match.Index + match.Length;
                    richTextBox.AppendLink(match.Groups["text"].Value, new Uri(match.Groups["link"].Value));
                }

                AppSettings.LogThis(match.Groups["text"] + ":" + match.Groups["link"]);
            }

            if (nextOffset < htmlFragment.Length)
                richTextBox.AppendText(htmlFragment.Substring(nextOffset));
        }

        public static void AppendText(this RichTextBox richTextBox, string text)
        {
            Paragraph paragraph;

            if (richTextBox.Document.Blocks.Count == 0 || (paragraph = richTextBox.Document.Blocks.LastBlock as Paragraph) == null)
            {
                paragraph = new Paragraph();
                richTextBox.Document.Blocks.Add(paragraph);
            }

            paragraph.Inlines.Add(new Run { Text = text });
        }

        public static void AppendLink(this RichTextBox richTextBox, string text, Uri uri)
        {
            Paragraph paragraph;

            if (richTextBox.Document.Blocks.Count == 0 || (paragraph = richTextBox.Document.Blocks.LastBlock as Paragraph) == null)
            {
                paragraph = new Paragraph();
                richTextBox.Document.Blocks.Add(paragraph);
            }

            var run = new Run { Text = text };
            var link = new Hyperlink { NavigateUri = uri, Foreground = richTextBox.Foreground };

            link.Inlines.Add(run);
            paragraph.Inlines.Add(link);
        }
    }

}
