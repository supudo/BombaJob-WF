using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

using MahApps.Metro.Controls;

namespace BombaJob.Utilities.Adorners
{
    public class OverlayAdorner : Adorner
    {
        public OverlayAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //drawingContext.DrawRectangle(Brushes.Blue, null, WindowRect());
            //base.OnRender(drawingContext);
            
            SolidColorBrush sr = new SolidColorBrush();
            sr.Color = Colors.Black;
            sr.Opacity = 0.5;
            drawingContext.DrawRectangle(sr, null,new Rect(new Point(0,0), DesiredSize));
            base.OnRender(drawingContext);
        }

        /*
        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            GeometryGroup grp = new GeometryGroup();
            grp.Children.Add(new RectangleGeometry(WindowRect()));
            grp.Children.Add(new RectangleGeometry(new Rect(layoutSlotSize)));
            return grp;
        }
         */

        private Rect WindowRect()
        {
            //return new Rect(new Point(0, 0), DesiredSize);

            if (AdornedElement == null)
                throw new ArgumentException("cannot adorn a null control");
            else
            {
                Window windowMain = Application.Current.MainWindow;
                Point windowOffset;

                if (windowMain == null)
                    throw new ArgumentException("can't get main window");
                else
                {
                    GeneralTransform transf = AdornedElement.TransformToVisual(windowMain);
                    if (transf == null || transf.Inverse == null)
                        throw new ArgumentException("no transform to window");
                    else
                        windowOffset = transf.Inverse.Transform(new Point(0, 0));
                }

                Point windowLowerRight = windowOffset;
                windowLowerRight.Offset(windowMain.ActualWidth, windowMain.ActualHeight);
                return new Rect(windowOffset, windowLowerRight);
            }
        }
    }
}