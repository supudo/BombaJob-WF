using System;
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

using BombaJob.Sync;
using BombaJob.Utilities;
using BombaJob.Utilities.Adorners;
using BombaJob.Utilities.Events;
using BombaJob.Utilities.Interfaces;
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
        private Thread loadingThread;
        private int _overlayVisible;
        public bool IsBusy { get; set; }
        public bool TabberEnabled { get; set; }
        public double TabberOpacity { get; set; }

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
            this.StartSynchronization();
        }

        #region Synchronization
        private void StartSynchronization()
        {
            if (this.syncManager == null)
                this.syncManager = new Synchronization();
            this.syncManager.SyncError += new Synchronization.EventHandler(syncManager_SyncError);
            this.syncManager.SyncComplete += new Synchronization.EventHandler(syncManager_SyncComplete);

            this.ShowOverlay();
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
