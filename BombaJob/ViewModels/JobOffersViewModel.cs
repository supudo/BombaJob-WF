using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using BombaJob.Database;
using BombaJob.Database.Domain;
using BombaJob.Database.Repository;
using BombaJob.Utilities.Controls;
using BombaJob.Utilities.Interfaces;

using Caliburn.Micro;

using Facebook;

namespace BombaJob.ViewModels
{
    [Export(typeof(JobOffersViewModel))]
    public class JobOffersViewModel : Screen
    {
        private IBombaJobRepository dbRepo;
        private TabberViewModel tabm;
        private int CategoryID;
        public ObservableCollection<JobOffer> OffersList { get; set; }

        #region Constructors
        public JobOffersViewModel(TabberViewModel _tabm, int categoryID)
        {
            this.DisplayName = " | " + Properties.Resources.menu_Jobs;
            this.tabm = _tabm;
            this.CategoryID = categoryID;

            this.InitVM();
            this.LoadOffers();
        }

        public JobOffersViewModel(TabberViewModel _tabm)
        {
            this.DisplayName = Properties.Resources.menu_Jobs;
            this.tabm = _tabm;
            this.CategoryID = 0;

            this.InitVM();
            this.LoadOffers();
        }

        public JobOffersViewModel()
        {
            this.DisplayName = Properties.Resources.menu_Jobs;
            this.InitVM();
            this.OffersList = this.dbRepo.GetJobOffers(AppSettings.OffersPerPage);
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
                this.OffersList = this.dbRepo.GetJobOffers(AppSettings.OffersPerPage, this.CategoryID);
            else
                this.OffersList = this.dbRepo.GetJobOffers(AppSettings.OffersPerPage);
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

        public void OffersList_Menu_ShareFacebook(JobOffer jobOffer)
        {
            AppSettings.LogThis("Share Facebook...");
            FacebookOAuthResult fbResult = null;
            Caliburn.Micro.Execute.OnUIThread(() => fbResult = IoC.Get<IWindowManager>().ShowFacebookLogin());
            if (fbResult != null)
                AppSettings.FacebookPost(fbResult, jobOffer);
        }

        public void OffersList_Menu_ShareTwitter(JobOffer jobOffer)
        {
            AppSettings.LogThis("Share Twitter...");
        }
        #endregion
    }
}
