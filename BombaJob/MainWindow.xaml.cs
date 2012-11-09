using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Navigation;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using BombaJob.Database;
using BombaJob.Database.Domain;
using BombaJob.Utilities;
using BombaJob.Views;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using MahApps.Metro;
using MahApps.Metro.Controls;

namespace BombaJob
{
    public partial class MainWindow : MetroWindow
    {
        IBombaJobRepository dbRepo;

        public MainWindow()
        {
            Properties.Resources.Culture = new CultureInfo(System.Configuration.ConfigurationManager.AppSettings["Culture"]);
            new SplashScreen().ShowDialog();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Orange"), Theme.Light);

            this.tbContent.SelectionChanged += new SelectionChangedEventHandler(tbContent_SelectionChanged);

            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();

            this.lstNewest.ItemsSource = this.dbRepo.GetNewestOffers(AppSettings.OffersPerPage);
            this.lstJobOffers.ItemsSource = this.dbRepo.GetCategoriesFor(true);
            this.lstPeopleOffers.ItemsSource = this.dbRepo.GetCategoriesFor(false);
        }

        private void tbContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.sepSearch.Visibility = Visibility.Hidden;
            this.tabSearch.Visibility = Visibility.Hidden;
            this.txtSearch.Text = "";
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !this.txtSearch.Text.Trim().Equals(""))
            {
                this.tbContent.SelectionChanged -= new SelectionChangedEventHandler(tbContent_SelectionChanged);
                this.sepSearch.Visibility = Visibility.Visible;
                this.tabSearch.Visibility = Visibility.Visible;
                this.lstSearchResults.ItemsSource = this.dbRepo.SearchJobOffers(this.txtSearch.Text.Trim());
                this.tabSearch.IsSelected = true;
                this.tbContent.SelectionChanged += new SelectionChangedEventHandler(tbContent_SelectionChanged);
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
        }

        private void newestList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            JobOffer jo = (JobOffer)this.lstNewest.SelectedItem;
            this.tabNewest.Content = new OfferDetails(jo);
        }
    }
}
