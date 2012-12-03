using System;
using System.ComponentModel;
using System.Timers;

using BombaJob.Sync;

namespace BombaJob.Utilities.Controls
{
    public class OfferCountTicker : INotifyPropertyChanged
    {
        Synchronization syncManager;

        public OfferCountTicker()
        {
            Timer timer = new Timer();
            timer.Interval = AppSettings.ConnectivityCheckTimer;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        public string OffersCount
        {
            get
            {
                if (this.syncManager == null)
                    this.syncManager = new Synchronization();
                return Properties.Resources.sb_offers + " " + this.syncManager.GetJobOffersCount();
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("OfferCount"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
