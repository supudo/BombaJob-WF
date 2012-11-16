using System;
using System.ComponentModel.Composition;
using System.Windows;

using BombaJob.Database.Domain;

using Caliburn.Micro;

namespace BombaJob.View
{
    [Export(typeof(OfferDetailsViewModel))]
    public partial class OfferDetailsViewModel : Screen
    {
        public JobOffer currentOffer { get; set; }

        public OfferDetailsViewModel(JobOffer jo)
        {
            this.currentOffer = jo;
            /*
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

            RichTextBoxExtensions.SetLinkedText(this.rtbNegativ, AppSettings.Hyperlinkify(this.currentOffer.Negativism));
            RichTextBoxExtensions.SetLinkedText(this.rtbPositiv, AppSettings.Hyperlinkify(this.currentOffer.Positivism));
             * */
        }
    }
}
