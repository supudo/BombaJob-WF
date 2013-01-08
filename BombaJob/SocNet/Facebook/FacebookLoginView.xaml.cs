using System;
using System.Linq;
using System.Windows.Controls;

using MahApps.Metro;
using MahApps.Metro.Controls;

namespace BombaJob.SocNet.Facebook
{
    public partial class FacebookLoginView : MetroWindow
    {
        public FacebookLoginView()
        {
            InitializeComponent();
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Orange"), Theme.Light);
        }
    }
}
