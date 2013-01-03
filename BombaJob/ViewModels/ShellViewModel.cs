using System;
using System.Collections.Generic;
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
using BombaJob.Utilities.Controls;
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
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
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
        public string MaxMinLabel { get; set; }

        public bool IsOverlayVisible
        {
            get
            {
                return this._overlayVisible > 0;
            }
        }

        private TabberViewModel vmTab;
        public TabberViewModel VMTab
        {
            get
            {
                return this.vmTab;
            }
            private set
            {
                this.vmTab = value;
            }
        }

        private SettingsViewModel vmSettings;
        public SettingsViewModel VMSettings
        {
            get
            {
                return this.vmSettings;
            }
            private set
            {
                this.vmSettings = value;
            }
        }
        #endregion

        #region Init
        public ShellViewModel()
        {
            this.DisplayName = Properties.Resources.appName;
            Properties.Resources.Culture = new CultureInfo(System.Configuration.ConfigurationManager.AppSettings["Culture"]);
            this.vmSettings = new SettingsViewModel(this);
            this.vmTab = new TabberViewModel(this);
            this.MaxMinLabel = Properties.Resources.tb_Close;
            NotifyOfPropertyChange(() => MaxMinLabel);
            ActivateItem(this.VMTab);
        }

        public void Bom()
        {
            if (this.syncManager == null)
                this.syncManager = new Synchronization();

            this.connRunning = false;

            this.SBStatus = Properties.Resources.sb_status + " " + Properties.Resources.sb_status_online;
            this.SBOffers = Properties.Resources.sb_offers + " " + this.syncManager.GetJobOffersCount();
            this.StartSynchronization();
        }
        #endregion

        #region Synchronization
        private void StartSynchronization()
        {
            this.IsBusy = true;
            this.syncManager.SyncError += new Synchronization.EventHandler(syncManager_SyncError);
            this.syncManager.SyncComplete += new Synchronization.EventHandler(syncManager_SyncComplete);

            this.ShowOverlay();
            this.thinkThread = new Thread(load);
            this.thinkThread.Start();
        }

        private void load()
        {
            Thread.Sleep(500);
            if (Properties.Settings.Default.stInitSync)
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
                Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowMessageBox(e.ErrorMessage, Properties.Resources.errorTitle, MessageBoxButton.OK));
        }

        private void FinishSync()
        {
            this.IsBusy = false;
            if (!this.VMTab.IsActive)
                ActivateItem(this.VMTab);
            NotifyOfPropertyChange(() => VMTab);
            this.HideOverlay();
            this.StartConnectivityCheck();
        }
        #endregion

        #region Connectivity check
        private void StartConnectivityCheck()
        {
            this.bgWorker = new BackgroundWorker();
            this.connectivityHandler();
        }

        private void connectivityHandler()
        {
            if (!this.bgWorker.IsBusy && !AppSettings.InDebug)
            {
                this.bgWorker.DoWork += delegate(object s, DoWorkEventArgs args)
                {
                    if (!this.connRunning)
                    {
                        this.connRunning = true;
                        args.Result = this.netHelper.hasConnection();
                    }
                };
                this.bgWorker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                {
                    bool hasInternet = (bool)args.Result;
                    this.SBStatus = Properties.Resources.sb_status + " ";
                    if (hasInternet)
                        this.SBStatus += Properties.Resources.sb_status_online;
                    else
                        this.SBStatus += Properties.Resources.sb_status_offline;
                    this.connRunning = false;
                    Thread.Sleep(AppSettings.ConnectivityCheckTimer);
                    this.connectivityHandler();
                };
                if (!this.connRunning)
                {
                    this.bgWorker.RunWorkerAsync();
                }
            }
        }
        #endregion

        #region Main bar
        public void Synchronize()
        {
            if (!this.IsBusy)
                this.StartSynchronization();
        }

        public void Settings()
        {
            if (!this.IsBusy)
                ActivateItem(this.VMSettings);
        }

        public void Search(TextBox txt, System.Windows.Input.KeyEventArgs e)
        {
            if (!this.IsBusy)
            {
                if (e.Key == Key.Enter && !txt.Text.Trim().Equals(""))
                {
                    AppSettings.LogThis("Search called (enter)..." + txt.Text);
                    this.VMTab.RefreshTabs(true, txt.Text);
                    txt.Text = "";
                    ActivateItem(this.VMTab);
                }
            }
        }

        public void ActivateTabber()
        {
            this.VMTab.RefreshTabs(false);
            ActivateItem(this.VMTab);
        }
        #endregion

        #region Overlay
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

        #region Taskbar icon
        public void TBOpen()
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Normal || Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                this.MaxMinLabel = Properties.Resources.tb_Open;
                NotifyOfPropertyChange(() => MaxMinLabel);
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
            }
            else
            {
                this.MaxMinLabel = Properties.Resources.tb_Close;
                NotifyOfPropertyChange(() => MaxMinLabel);
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
        }

        public void TBSettings()
        {
            this.Settings();
        }

        public void TBExit()
        {
            AppSettings.LogThis("Shutting down...");
            Application.Current.Shutdown();
        }
        #endregion
    }
}
