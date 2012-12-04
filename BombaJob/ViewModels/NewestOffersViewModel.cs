using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using BombaJob.Database;
using BombaJob.Database.Domain;
using BombaJob.Utilities.Interfaces;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    [Export(typeof(NewestOffersViewModel))]
    public class NewestOffersViewModel : Screen
    {
        private IBombaJobRepository dbRepo;
        public ObservableCollection<JobOffer> OffersList { get; set; }
        private TabberViewModel tabm;

        public NewestOffersViewModel(TabberViewModel _tabm)
        {
            this.tabm = _tabm;
            this.DisplayName = Properties.Resources.menu_Newest;

            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();
            this.OffersList = this.dbRepo.GetNewestOffers(AppSettings.OffersPerPage);
        }

        public NewestOffersViewModel()
        {
            this.DisplayName = Properties.Resources.menu_Newest;

            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();
            this.OffersList = this.dbRepo.GetNewestOffers(AppSettings.OffersPerPage);
        }

        public void OffersList_Menu_View(JobOffer jobOffer)
        {
            if (this.tabm != null)
                this.tabm.OffersList_Menu_View(jobOffer);
        }

        public void OffersList_Menu_Message(JobOffer jobOffer)
        {
            AppSettings.LogThis("--- MOUSE OffersList_Menu_Message ..." + ((jobOffer != null) ? "" + jobOffer.OfferID : "0"));
        }

        public void OffersList_Menu_Mark(JobOffer jobOffer)
        {
            AppSettings.LogThis("--- MOUSE OffersList_Menu_Mark ..." + ((jobOffer != null) ? "" + jobOffer.OfferID : "0"));
        }
    }
}
