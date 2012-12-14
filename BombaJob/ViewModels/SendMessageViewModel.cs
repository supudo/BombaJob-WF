using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using BombaJob.Database.Domain;
using BombaJob.Database.Repository;
using BombaJob.Sync;
using BombaJob.Utilities.Controls;
using BombaJob.Utilities.Events;
using BombaJob.Utilities.Interfaces;
using BombaJob.Utilities.Misc;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    [Export(typeof(SendMessageViewModel))]
    public class SendMessageViewModel : Screen, IDataErrorInfo
    {
        #region Variables
        private JobOffer currentOffer;
        private readonly IValidator validator = new DefaultValidator();
        private IBombaJobRepository dbRepo;
        private TabberViewModel tabm;
        private Synchronization syncManager;
        private Thread thinkThread;
        #endregion

        #region Properties
        private string offerMessage;
        [Required(ErrorMessageResourceType = typeof(Properties.Resources), ErrorMessageResourceName = "offer_Message_Empty")]
        public string OfferMessage
        {
            get { return this.offerMessage; }
            set
            {
                this.offerMessage = value;
                NotifyOfPropertyChange(() => OfferMessage);
                NotifyOfPropertyChange(() => CanSendPM);
            }
        }
        #endregion

        public SendMessageViewModel(TabberViewModel _tabm, JobOffer jo)
        {
            this.tabm = _tabm;
            this.currentOffer = jo;
            this.DisplayName = " | #" + this.currentOffer.OfferID;

            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();
        }

        #region Forms
        public bool CanSendPM
        {
            get { return string.IsNullOrEmpty(Error); }
        }
        #endregion

        #region IDataErrorInfo Members
        public string this[string columnName]
        {
            get { return string.Join(Environment.NewLine, validator.Validate(this, columnName).Select(x => x.Message)); }
        }

        public string Error
        {
            get { return string.Join(Environment.NewLine, validator.Validate(this).Select(x => x.Message)); }
        }
        #endregion

        #region Send
        public void SendPM()
        {
            this.tabm.StartLoading();
            AppSettings.LogThis("Send message... " + this.OfferMessage);
            if (this.syncManager == null)
                this.syncManager = new Synchronization();
            this.syncManager.SyncError += new Synchronization.EventHandler(syncManager_Error);
            this.syncManager.SyncComplete += new Synchronization.EventHandler(syncManager_Complete);
            this.thinkThread = new Thread(send);
            this.thinkThread.Start();
        }

        private void send()
        {
            Thread.Sleep(500);
            Dictionary<string, string> postParams = new Dictionary<string, string>();
            postParams.Add("oid", "" + this.currentOffer.OfferID);
            postParams.Add("message", "" + this.OfferMessage);
            this.syncManager.DoSendPM(postParams);
        }

        private void syncManager_Complete(object sender, BombaJobEventArgs e)
        {
            this.SendFinished();
        }

        private void syncManager_Error(object sender, BombaJobEventArgs e)
        {
            this.tabm.StopLoading();
            if (e.IsError)
                Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowMessageBox(e.ErrorMessage, Properties.Resources.errorTitle, MessageBoxButton.OK));
        }

        private void SendFinished()
        {
            this.OfferMessage = "";
            this.tabm.StopLoading();
            Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowMessageBox(Properties.Resources.offer_ThankYou, Properties.Resources.offer_Boom, MessageBoxButton.OK));
        }
        #endregion
    }
}
