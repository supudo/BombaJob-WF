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
using BombaJob.Database.Repository;
using BombaJob.Utilities.Interfaces;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    [Export(typeof(NewestOffersViewModel))]
    public class PeopleOffersViewModel : Screen
    {
        private IBombaJobRepository dbRepo;
        public ObservableCollection<JobOffer> OffersList { get; set; }
        private TabberViewModel tabm;

        public PeopleOffersViewModel(TabberViewModel _tabm)
        {
            this.tabm = _tabm;
            this.DisplayName = Properties.Resources.menu_People;

            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();
            this.LoadOffers();
        }

        public PeopleOffersViewModel()
        {
            this.DisplayName = Properties.Resources.menu_People;

            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();
            this.OffersList = this.dbRepo.GetPeopleOffers(AppSettings.OffersPerPage);
        }

        private void LoadOffers()
        {
            this.OffersList = this.dbRepo.GetNewestOffers(AppSettings.OffersPerPage);
            NotifyOfPropertyChange(() => OffersList);
        }

        #region Context menu
        public void OffersList_Menu_View(JobOffer jobOffer)
        {
            if (this.tabm != null)
                this.tabm.OffersList_Menu_View(jobOffer);
        }

        public void OffersList_Menu_Message(JobOffer jobOffer)
        {
            if (this.tabm != null)
                this.tabm.OffersList_Menu_Message(jobOffer);
        }

        public void OffersList_Menu_Mark(JobOffer jobOffer)
        {
            IBombaJobRepository dbRepo = new BombaJobRepository();
            dbRepo.MarkAsRead(jobOffer);
            this.LoadOffers();
        }
        #endregion
    }
}
