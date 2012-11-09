using System;
using System.Collections.Generic;
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
using BombaJob.Views;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using MahApps.Metro;
using MahApps.Metro.Controls;

namespace BombaJob.Views
{
    public partial class OfferDetails : UserControl
    {
        JobOffer currentOffer;

        public OfferDetails()
        {
            InitializeComponent();
        }

        public OfferDetails(JobOffer jo)
        {
            InitializeComponent();
            this.currentOffer = jo;
            this.Loaded += new RoutedEventHandler(OfferDetails_Loaded);
        }

        void OfferDetails_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtTitle.Text = this.currentOffer.Title;
            
            this.txtCategory.Text = this.currentOffer.CategoryTitle;
            this.txtDate.Text = AppSettings.DoLongDate(this.currentOffer.PublishDate);

            this.txtFreelanceLabel.Text = Properties.Resources.ResourceManager.GetString("offer_FreelanceYn");
            if (this.currentOffer.FreelanceYn)
                this.txtFreelance.Text = Properties.Resources.ResourceManager.GetString("yes");
            else
                this.txtFreelance.Text = Properties.Resources.ResourceManager.GetString("no");

            if (this.currentOffer.HumanYn)
                this.txtNegativLabel.Text = Properties.Resources.ResourceManager.GetString("offer_Human_Negativ");
            else
                this.txtNegativLabel.Text = Properties.Resources.ResourceManager.GetString("offer_Company_Negativ");
    
            if (this.currentOffer.HumanYn)
                this.txtPositivLabel.Text = Properties.Resources.ResourceManager.GetString("offer_Human_Positiv");
            else
                this.txtPositivLabel.Text = Properties.Resources.ResourceManager.GetString("offer_Company_Positiv");

            this.rtbPositiv.Document.Blocks.Clear();
            //this.rtbPositiv.Document.Blocks.Add(new Paragraph(new Run(AppSettings.Hyperlinkify(this.currentOffer.Positivism))));
            this.wPositiv.NavigateToString(this.currentOffer.Positivism);

            this.rtbNegativ.Document.Blocks.Clear();
            this.rtbNegativ.Document.Blocks.Add(new Paragraph(new Run(AppSettings.Hyperlinkify(this.currentOffer.Negativism))));
        }
    }
}
