using System;
using System.Linq;

using MahApps.Metro;
using MahApps.Metro.Controls;

namespace BombaJob.Views
{
    public partial class BombaJobMainView : MetroWindow
    {
        public BombaJobMainView()
        {
            InitializeComponent();
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Orange"), Theme.Light);
        }

        private void MetroWindow_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            AppSettings.LogThis("MetroWindow_SizeChanged " + e.NewSize.Height + "x" + e.NewSize.Width);
        }

        private void ProgressRing_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            AppSettings.LogThis("ProgressRing_IsVisibleChanged " + (((bool)e.NewValue) ? "Show....." : "Hide....."));
        }
    }
}
