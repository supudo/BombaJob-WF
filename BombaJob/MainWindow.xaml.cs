using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using BombaJob.Database;
using BombaJob.Database.Domain;
using BombaJob.Utilities;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using MahApps.Metro;
using MahApps.Metro.Controls;

namespace BombaJob
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            Properties.Resources.Culture = new CultureInfo(System.Configuration.ConfigurationManager.AppSettings["Culture"]);
            new SplashScreen().ShowDialog();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Orange"), Theme.Light);

            IBombaJobRepository repo = new BombaJobRepository();
            this.newestList.ItemsSource = repo.GetNewestOffers(AppSettings.OffersPerPage);
            this.jobOffersList.ItemsSource = repo.GetCategoriesFor(true);
            this.peopleOffersList.ItemsSource = repo.GetCategoriesFor(false);
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AppSettings.LogThis("Searching for " + this.txtSearch.Text);
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
