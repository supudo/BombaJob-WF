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
    [Export(typeof(PeopleOffersViewModel))]
    public class PeopleOffersViewModel : Screen
    {
        private IBombaJobRepository dbRepo;
        private TabberViewModel tabm;
        private int CategoryID;
        public ObservableCollection<JobOffer> OffersList { get; set; }
        
        #region Constructors
        public PeopleOffersViewModel(TabberViewModel _tabm, int categoryID)
        {
            this.DisplayName = " | " + Properties.Resources.menu_Jobs;
            this.tabm = _tabm;
            this.CategoryID = categoryID;

            this.InitVM();
            this.LoadOffers();
        }

        public PeopleOffersViewModel(TabberViewModel _tabm)
        {
            this.tabm = _tabm;
            this.DisplayName = Properties.Resources.menu_People;

            this.InitVM();
            this.LoadOffers();
        }

        public PeopleOffersViewModel()
        {
            this.DisplayName = Properties.Resources.menu_People;

            this.InitVM();
            this.OffersList = this.dbRepo.GetPeopleOffers(AppSettings.OffersPerPage);
        }
        #endregion

        #region UI
        private void InitVM()
        {
            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();
        }

        private void LoadOffers()
        {
            if (this.CategoryID > 0)
                this.OffersList = this.dbRepo.GetPeopleOffers(AppSettings.OffersPerPage, this.CategoryID);
            else
                this.OffersList = this.dbRepo.GetPeopleOffers(AppSettings.OffersPerPage);
            NotifyOfPropertyChange(() => OffersList);
        }
        #endregion

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
