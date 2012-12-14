﻿using System;
using System.ComponentModel.Composition;
using System.Windows;

using BombaJob.Database.Domain;
using BombaJob.Utilities.Events;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    [Export(typeof(OfferDetailsViewModel))]
    public partial class OfferDetailsViewModel : Screen
    {
        private TabberViewModel tabvm;
        public JobOffer CurrentJobOffer { get; set; }
        public string OfferPositiv { get; set; }
        public string OfferNegativ { get; set; }

        public OfferDetailsViewModel(TabberViewModel tab, JobOffer offer)
        {
            this.DisplayName = " | #" + offer.OfferID;
            this.tabvm = tab;
            this.CurrentJobOffer = offer;
            this.DisplayOffer(offer);
        }

        private void DisplayOffer(JobOffer jo)
        {
            this.CurrentJobOffer = jo;
            if (jo.HumanYn)
            {
                this.OfferPositiv = Properties.Resources.offer_Human_Positiv;
                this.OfferNegativ = Properties.Resources.offer_Human_Negativ;
            }
            else
            {
                this.OfferPositiv = Properties.Resources.offer_Company_Positiv;
                this.OfferNegativ = Properties.Resources.offer_Company_Negativ;
            }
        }

        public void OffersList_Menu_Message()
        {
            this.tabvm.OffersList_Menu_Message(this.CurrentJobOffer);
        }
    }
}
