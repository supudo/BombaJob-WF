using System;
using System.Windows.Controls;

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
            this.fdPositiv.FontFamily = this.txtPositiv.FontFamily;
            this.fdNegativ.FontFamily = this.txtPositiv.FontFamily;
        }
    }
}
