using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

using MahApps.Metro.Controls;

namespace BombaJob.Utilities.Adorners
{
    public class OverlayAdorner : Adorner
    {
        public OverlayAdorner(MetroWindow adornerElement) : base(adornerElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.Blue, new Pen(Brushes.Red, 1), new Rect(new Point(0, 0), DesiredSize));
            base.OnRender(drawingContext);
        }
    }
}
