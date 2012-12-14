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

using MahApps.Metro.Controls;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    [Export(typeof(JobOffersPanViewModel))]
    public class JobOffersPanViewModel : Screen
    {
        private IBombaJobRepository dbRepo;
        private TabberViewModel tabm;
        private PanoramaGroup PGOffers;
        private PanoramaGroup PGCategories;
        public ObservableCollection<JobOffer> ListOffers { get; set; }
        public ObservableCollection<Category> ListCategories { get; set; }
        public ObservableCollection<PanoramaGroup> PGOffersCategories { get; set; }

        public JobOffersPanViewModel(TabberViewModel _tabm)
        {
            this.tabm = _tabm;
            this.DisplayName = Properties.Resources.menu_Jobs;

            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();
            this.LoadLists();
        }

        private void LoadLists()
        {
            this.ListOffers = this.dbRepo.GetJobOffers(AppSettings.OffersPerPage);
            this.ListCategories = this.dbRepo.GetCategoriesFor(false);
            NotifyOfPropertyChange(() => ListOffers);
            NotifyOfPropertyChange(() => ListCategories);

            this.PGOffers = new PanoramaGroup(Properties.Resources.menu_Offers, this.ListOffers);
            this.PGCategories = new PanoramaGroup(Properties.Resources.menu_Categories, this.ListCategories);
            this.PGOffersCategories = new ObservableCollection<PanoramaGroup> { this.PGOffers, this.PGCategories };
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
            this.LoadLists();
        }
        #endregion
    }
}
