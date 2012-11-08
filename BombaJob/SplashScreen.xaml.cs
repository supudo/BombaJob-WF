using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Threading;
using System.Windows.Threading;

using BombaJob.Sync;
using BombaJob.Utilities;

using MahApps.Metro.Controls;

namespace BombaJob
{
    public partial class SplashScreen : MetroWindow
    {
        public delegate void EventHandler(Object sender, BombaJobEventArgs e);

        private Synchronization syncManager;

        Thread loadingThread;

        public SplashScreen()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(SplashScreen_Loaded);
        }

        void SplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.syncManager == null)
                this.syncManager = new Synchronization();
            this.syncManager.SyncError += new Synchronization.EventHandler(syncManager_SyncError);
            this.syncManager.SyncComplete += new Synchronization.EventHandler(syncManager_SyncComplete);

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

        void syncManager_SyncComplete(object sender, BombaJobEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate() { Close(); });
        }

        void syncManager_SyncError(object sender, BombaJobEventArgs e)
        {
            if (e.IsError)
                MessageBox.Show(e.ErrorMessage);
            else
                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate() { Close(); });
        }
    }
}
