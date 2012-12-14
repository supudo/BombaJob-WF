using System;
using System.Windows.Controls;

using BombaJob.Utilities.Events;

using Caliburn.Micro;

namespace BombaJob.Views
{
    public partial class OfferDetailsView : UserControl
    {
        public OfferDetailsView()
        {
            InitializeComponent();
            this.Loaded += new System.Windows.RoutedEventHandler(OfferDetailsView_Loaded);
        }

        void OfferDetailsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.FDOfferPositiv.FontFamily = this.txtPositiv.FontFamily;
            this.FDOfferNegativ.FontFamily = this.txtPositiv.FontFamily;
        }
    }
}
