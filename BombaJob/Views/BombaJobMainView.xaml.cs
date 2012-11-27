using System;
using System.Linq;
using System.Windows.Documents;

using BombaJob.ViewModels;

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
    }
}
