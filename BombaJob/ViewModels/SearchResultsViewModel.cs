using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using BombaJob.Database;
using BombaJob.Database.Domain;
using BombaJob.Database.Repository;
using BombaJob.Sync;
using BombaJob.Utilities.Controls;
using BombaJob.Utilities.Events;
using BombaJob.Utilities.Interfaces;

using Caliburn.Micro;

using Facebook;

namespace BombaJob.ViewModels
{
    [Export(typeof(SearchResultsViewModel))]
    public class SearchResultsViewModel : Screen
    {
        private IBombaJobRepository dbRepo;
        private TabberViewModel tabm;
        private Synchronization syncManager;
        private Thread thinkThread;
        public ObservableCollection<JobOffer> OffersList { get; set; }
        public string SearchStatus { get; set; }
        public string SearchQuery { get; set; }

        public SearchResultsViewModel(TabberViewModel _tabm, string sQuery)
        {
            this.tabm = _tabm;
            this.SearchQuery = sQuery;
            this.DisplayName = " | " + Properties.Resources.menu_Search;

            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();

            this.StartSearch();
            if (Properties.Settings.Default.stOnlineSearch)
                this.DoSearch();
            else
                this.SearchOffers();
        }

        #region Search
        private void DoSearch()
        {
            if (this.syncManager == null)
                this.syncManager = new Synchronization();
            this.syncManager.SyncError += new Synchronization.EventHandler(syncManager_SearchError);
            this.syncManager.SyncComplete += new Synchronization.EventHandler(syncManager_SearchComplete);
            this.thinkThread = new Thread(load);
            this.thinkThread.Start();
        }

        private void load()
        {
            Thread.Sleep(500);
            this.syncManager.DoSearch(this.SearchQuery);
        }

        private void syncManager_SearchComplete(object sender, BombaJobEventArgs e)
        {
            this.SearchOffers();
        }

        private void syncManager_SearchError(object sender, BombaJobEventArgs e)
        {
            this.SearchOffers();
            if (e.IsError)
                Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowMessageBox(e.ErrorMessage, Properties.Resources.errorTitle, MessageBoxButton.OK));
        }

        private void SearchOffers()
        {
            this.OffersList = this.dbRepo.SearchJobOffers(this.SearchQuery);
            NotifyOfPropertyChange(() => OffersList);
            this.FinishSearch();
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
            this.SearchOffers();
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

        #region Search status
        private void StartSearch()
        {
            this.SearchStatus = Properties.Resources.search_For;
            NotifyOfPropertyChange(() => SearchStatus);
            this.tabm.StartLoading();
        }

        private void FinishSearch()
        {
            this.SearchStatus = Properties.Resources.search_Results;
            NotifyOfPropertyChange(() => SearchStatus);
            this.tabm.StopLoading();
        }
        #endregion
    }
}
