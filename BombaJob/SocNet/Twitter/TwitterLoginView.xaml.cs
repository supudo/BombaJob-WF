using System;
using System.Linq;
using System.Windows.Controls;

using MahApps.Metro;
using MahApps.Metro.Controls;

namespace BombaJob.SocNet.Twitter
{
    public partial class TwitterLoginView : MetroWindow
    {
        public TwitterLoginView()
        {
            InitializeComponent();
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Orange"), Theme.Light);
        }
    }
}
