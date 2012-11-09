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

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using MahApps.Metro;
using MahApps.Metro.Controls;

namespace BombaJob
{
    /// <summary>
    /// Interaction logic for OfferDetails.xaml
    /// </summary>
    public partial class OfferDetails : MetroWindow
    {
        public OfferDetails()
        {
            Properties.Resources.Culture = new CultureInfo(System.Configuration.ConfigurationManager.AppSettings["Culture"]);
            InitializeComponent();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
