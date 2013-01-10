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
    [Export(typeof(NewestOffersViewModel))]
    public class NewestOffersViewModel : Screen
    {
        private IBombaJobRepository dbRepo;
        private TabberViewModel tabm;
        public ObservableCollection<JobOffer> OffersList { get; set; }

        public NewestOffersViewModel(TabberViewModel _tabm)
        {
            this.tabm = _tabm;
            this.DisplayName = Properties.Resources.menu_Newest;

            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();
            this.LoadOffers();
        }

        public NewestOffersViewModel()
        {
            this.DisplayName = Properties.Resources.menu_Newest;

            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();
            this.LoadOffers();
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

        public void OffersList_Menu_ShareFacebook(JobOffer jobOffer)
        {
            Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowFacebookLogin(jobOffer));
        }

        public void OffersList_Menu_ShareTwitter(JobOffer jobOffer)
        {
            Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowTwitterLogin(jobOffer));
        }
        #endregion
    }
}
