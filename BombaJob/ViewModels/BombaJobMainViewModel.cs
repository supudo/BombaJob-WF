﻿using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

using BombaJob.Database.Domain;
using BombaJob.Sync;
using BombaJob.Utilities;
using BombaJob.Utilities.Adorners;
using BombaJob.Utilities.Events;
using BombaJob.Utilities.Interfaces;

using Caliburn.Micro;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using MahApps.Metro;
using MahApps.Metro.Controls;

namespace BombaJob.ViewModels
{
    [Export(typeof(IShell))]
    public class BombaJobMainViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public bool IsBusy { get; set; }
        private Synchronization syncManager;
        private Thread loadingThread;
        private int _overlayDependencies;

        [ImportingConstructor]
        public BombaJobMainViewModel()
        {
            this.DisplayName = Properties.Resources.appName;
            Properties.Resources.Culture = new CultureInfo(System.Configuration.ConfigurationManager.AppSettings["Culture"]);
            this.StartSynchronization();
        }

        #region Synchronization
        private void StartSynchronization()
        {
            if (this.syncManager == null)
                this.syncManager = new Synchronization();
            this.syncManager.SyncError += new Synchronization.EventHandler(syncManager_SyncError);
            this.syncManager.SyncComplete += new Synchronization.EventHandler(syncManager_SyncComplete);

            this.IsBusy = true;
            this.loadingThread = new Thread(load);
            this.loadingThread.Start();
        }

        private void load()
        {
            Thread.Sleep(500);
            if (AppSettings.ConfInitSync)
                this.syncManager.StartSync();
            else
                this.syncManager_SyncComplete(null, new BombaJobEventArgs(false, "", ""));
        }

        private void syncManager_SyncComplete(object sender, BombaJobEventArgs e)
        {
            this.FinishSync();
        }

        private void syncManager_SyncError(object sender, BombaJobEventArgs e)
        {
            this.FinishSync();
            if (e.IsError)
                MessageBox.Show(e.ErrorMessage);
        }

        private void FinishSync()
        {
            Items.Add(new NewestOffersViewModel());
            Items.Add(new JobOffersViewModel());
            Items.Add(new PeopleOffersViewModel());
            Items.Add(new PostViewModel());
            ActivateItem(Items.First());
        }
        #endregion

        #region Main bar
        public void Synchronize()
        {
            if (!this.IsBusy)
            {
                AppSettings.LogThis("Synchronization called...");
            }
        }

        public void Settings()
        {
            if (!this.IsBusy)
            {
                AppSettings.LogThis("Settings called...");
            }
        }

        public void Search(TextBox txt, System.Windows.Input.KeyEventArgs e)
        {
            if (!this.IsBusy)
            {
                if (e.Key == Key.Enter && !txt.Text.Trim().Equals(""))
                {
                    AppSettings.LogThis("Search called (enter)..." + txt.Text);
                }
            }
        }
        #endregion

        #region Tab actions
        public void TabSelected(TabControl tabc)
        {
            if (this.IsBusy)
            {
                this.IsBusy = false;
                NotifyOfPropertyChange(() => IsBusy);
            }
            if (Items.Count > 4)
            {
                if (tabc != null && tabc.SelectedIndex < 3)
                    Items.RemoveAt(Items.Count - 1);
            }
        }

        public void OffersList_SelectionChanged(JobOffer jobOffer)
        {
            OfferDetailsViewModel od = new OfferDetailsViewModel(jobOffer);
            ActivateItem(od);
        }
        #endregion

        #region Message box
        public void ShowOverlay()
        {
            _overlayDependencies++;
            NotifyOfPropertyChange(() => IsOverlayVisible);
            AppSettings.LogThis(" ------ ShowOverlay...");
        }

        public void HideOverlay()
        {
            _overlayDependencies--;
            NotifyOfPropertyChange(() => IsOverlayVisible);
            AppSettings.LogThis(" ------ HideOverlay...");
        }

        public bool IsOverlayVisible
        {
            get
            {
                AppSettings.LogThis(" ------ IsOverlayVisible ... " + _overlayDependencies);
                return _overlayDependencies > 0;
            }
        }
        #endregion
    }
}
