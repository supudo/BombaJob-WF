using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

using BombaJob.Sync;
using BombaJob.Utilities;
using BombaJob.Utilities.Events;
using BombaJob.Utilities.Interfaces;
using BombaJob.Utilities.Network;
using BombaJob.Views;

using Caliburn.Micro;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using MahApps.Metro;
using MahApps.Metro.Controls;

namespace BombaJob.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<IScreen>
    {
        #region Properties
        private Synchronization syncManager;
        private Thread thinkThread;
        private int _overlayVisible;
        private NetworkHelper netHelper = new NetworkHelper();
        private BackgroundWorker bgWorker;
        private bool connRunning;

        public bool IsBusy { get; set; }
        public bool TabberEnabled { get; set; }
        public double TabberOpacity { get; set; }
        public string SBStatus { get; set; }
        public string SBOffers { get; set; }

        public bool IsOverlayVisible
        {
            get
            {
                return this._overlayVisible > 0;
            }
        }

        private TabberViewModel tabber = new TabberViewModel();
        public TabberViewModel Tabber
        {
            get
            {
                return this.tabber;
            }
            private set
            {
                this.tabber = value;
            }
        }
        #endregion

        public ShellViewModel()
        {
            this.DisplayName = Properties.Resources.appName;
            Properties.Resources.Culture = new CultureInfo(System.Configuration.ConfigurationManager.AppSettings["Culture"]);
            ActivateItem(this.Tabber);

            if (this.syncManager == null)
                this.syncManager = new Synchronization();

            this.connRunning = false;

            this.SBStatus = Properties.Resources.sb_status + " " + Properties.Resources.sb_status_online;
            this.SBOffers = Properties.Resources.sb_offers + " " + this.syncManager.GetJobOffersCount();
            this.StartSynchronization();
        }

        #region Synchronization
        private void StartSynchronization()
        {
            this.syncManager.SyncError += new Synchronization.EventHandler(syncManager_SyncError);
            this.syncManager.SyncComplete += new Synchronization.EventHandler(syncManager_SyncComplete);

            this.IsBusy = true;
            this.ShowOverlay();
            this.thinkThread = new Thread(load);
            this.thinkThread.Start();
        }

        private void load()
        {
            Thread.Sleep(500);
            if (AppSettings.ConfInitSync)
                this.syncManager.StartSync();
            else
            {
                Thread.Sleep(2000);
                this.syncManager_SyncComplete(null, new BombaJobEventArgs(false, "", ""));
            }
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
            this.HideOverlay();
            this.StartConnectivityCheck();
        }
        #endregion

        #region Connectivity check
        private void StartConnectivityCheck()
        {
            AppSettings.LogThis(" ---- StartConnectivityCheck ... ");
            this.bgWorker = new BackgroundWorker();
            this.connectivityHandler();
        }

        private void connectivityHandler()
        {
            if (!this.bgWorker.IsBusy)
            {
                this.bgWorker.DoWork += delegate(object s, DoWorkEventArgs args)
                {
                    this.connRunning = true;
                    args.Result = this.netHelper.hasConnection();
                };
                this.bgWorker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                {
                    bool hasInternet = (bool)args.Result;
                    this.SBStatus = Properties.Resources.sb_status + " ";
                    if (hasInternet)
                        this.SBStatus += Properties.Resources.sb_status_online;
                    else
                        this.SBStatus += Properties.Resources.sb_status_offline;
                    Thread.Sleep(5000);
                    this.connRunning = false;
                    this.connectivityHandler();
                };
                if (!this.connRunning)
                {
                    AppSettings.LogThis(" ---- RunWorkerAsync ... ");
                    this.bgWorker.RunWorkerAsync();
                }
            }
        }
        #endregion

        #region Main bar
        public void Synchronize()
        {
            if (!this.IsBusy)
                AppSettings.LogThis("Synchronization called...");
        }

        public void Settings()
        {
            if (!this.IsBusy)
                AppSettings.LogThis("Settings called...");
        }

        public void Search(TextBox txt, System.Windows.Input.KeyEventArgs e)
        {
            if (!this.IsBusy)
            {
                if (e.Key == Key.Enter && !txt.Text.Trim().Equals(""))
                    AppSettings.LogThis("Search called (enter)..." + txt.Text);
            }
        }
        #endregion

        #region Message box
        public void ShowOverlay()
        {
            this._overlayVisible++;
            this.TabberEnabled = false;
            this.TabberOpacity = 0.5;
            NotifyOfPropertyChange(() => IsOverlayVisible);
            NotifyOfPropertyChange(() => TabberOpacity);
            NotifyOfPropertyChange(() => TabberEnabled);
            AppSettings.LogThis(" ------ ShowOverlay...");
        }

        public void HideOverlay()
        {
            this._overlayVisible--;
            this.TabberEnabled = true;
            this.TabberOpacity = 1;
            NotifyOfPropertyChange(() => IsOverlayVisible);
            NotifyOfPropertyChange(() => TabberEnabled);
            NotifyOfPropertyChange(() => TabberOpacity);
            AppSettings.LogThis(" ------ HideOverlay...");
        }
        #endregion
    }
}
