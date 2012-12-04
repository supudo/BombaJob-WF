using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Xml.Linq;

using BombaJob.Database;
using BombaJob.Database.Domain;
using BombaJob.Database.Repository;
using BombaJob.Utilities;
using BombaJob.Utilities.Events;
using BombaJob.Utilities.Interfaces;
using BombaJob.Utilities.Network;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace BombaJob.Sync
{
    public class Synchronization
    {
        #region Variables
        public delegate void EventHandler(Object sender, BombaJobEventArgs e);
        public event EventHandler SyncComplete;
        public event EventHandler SyncError;
        NetworkHelper _networkHelper;
        private AppSettings.ServiceOp currentOp;
        BackgroundWorker bgWorker;
        #endregion

        #region Constructor
        public Synchronization()
        {
            this._networkHelper = new NetworkHelper();
            this._networkHelper.DownloadComplete += new NetworkHelper.EventHandler(_networkHelper_DownloadComplete);
            this._networkHelper.DownloadInBackgroundComplete += new NetworkHelper.EventHandler(_networkHelper_DownloadInBackgroundComplete);
            this._networkHelper.DownloadError += new NetworkHelper.EventHandler(_networkHelper_DownloadError);
        }
        #endregion

        #region Dispatcher
        private void dispatchDownload(string xmlContent)
        {
            switch (this.currentOp)
            {
                case AppSettings.ServiceOp.ServiceOpTexts:
                    this.doTexts(xmlContent);
                    break;
                case AppSettings.ServiceOp.ServiceOpCategories:
                    this.doCategories(xmlContent);
                    break;
                case AppSettings.ServiceOp.ServiceOpNewestOffers:
                    this.doNewestOffers(xmlContent);
                    break;
                case AppSettings.ServiceOp.ServiceOpSearch:
                    this.doJobOffers(xmlContent);
                    break;
                case AppSettings.ServiceOp.ServiceOpJobs:
                    this.doJobOffers(xmlContent);
                    break;
                default:
                    SyncComplete(this, new BombaJobEventArgs(false, "", ""));
                    break;
            }
        }
        #endregion

        #region Public
        public void DoPostOffer(Dictionary<string, string> postParams)
        {
            this.currentOp = AppSettings.ServiceOp.ServiceOpPost;
            this._networkHelper.uploadURL(AppSettings.ServicesURL + "?action=postNewJob", postParams);
        }

        public void DoSearch(string keyword, int freelance)
        {
            this.currentOp = AppSettings.ServiceOp.ServiceOpSearch;
            Dictionary<string, string> postArray = new Dictionary<string, string>();
            postArray.Add("keyword", keyword);
            postArray.Add("freelance", "" + freelance);
            this._networkHelper.uploadURL(AppSettings.ServicesURL + "?action=searchOffers", postArray);
        }

        public void DoSendEmail(Dictionary<string, string> postParams)
        {
            this.currentOp = AppSettings.ServiceOp.ServiceOpSendEmail;
            this._networkHelper.uploadURL(AppSettings.ServicesURL + "?action=sendEmailMessage", postParams);
        }

        public void DoSendPM(Dictionary<string, string> postParams)
        {
            this.currentOp = AppSettings.ServiceOp.ServiceOpSendEmail;
            this._networkHelper.uploadURL(AppSettings.ServicesURL + "?action=postMessage", postParams);
        }

        public void LoadOffersInBackground()
        {
            this.currentOp = AppSettings.ServiceOp.ServiceOpJobs;
            this.bgWorker = new BackgroundWorker();
            RunProcess();
        }
        #endregion

        #region Threads
        private void RunProcess()
        {
            this.bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            this.bgWorker.RunWorkerAsync();
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (this.currentOp)
            {
                case AppSettings.ServiceOp.ServiceOpTexts:
                    this._networkHelper.downloadURL(AppSettings.ServicesURL + "?action=getTextContent", true, this.currentOp);
                    break;
                case AppSettings.ServiceOp.ServiceOpCategories:
                    this._networkHelper.downloadURL(AppSettings.ServicesURL + "?action=getCategories", true, this.currentOp);
                    break;
                case AppSettings.ServiceOp.ServiceOpNewestOffers:
                    this._networkHelper.downloadURL(AppSettings.ServicesURL + "?action=getNewJobs", true, this.currentOp);
                    break;
                case AppSettings.ServiceOp.ServiceOpJobs:
                    this._networkHelper.downloadURL(AppSettings.ServicesURL + "?action=getJobs", true, this.currentOp);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Handle URLs
        private void syncTexts()
        {
            this.currentOp = AppSettings.ServiceOp.ServiceOpTexts;
            this.bgWorker = new BackgroundWorker();
            RunProcess();
        }

        private void syncCategories()
        {
            this.currentOp = AppSettings.ServiceOp.ServiceOpCategories;
            this.bgWorker = new BackgroundWorker();
            RunProcess();
        }

        private void syncNewestOffers()
        {
            this.currentOp = AppSettings.ServiceOp.ServiceOpNewestOffers;
            this.bgWorker = new BackgroundWorker();
            RunProcess();
        }
        #endregion

        #region Events
        private void SynchronizationComplete()
        {
            this.SyncComplete(this, new BombaJobEventArgs(false, "", ""));
        }

        void _networkHelper_DownloadComplete(object sender, BombaJobEventArgs e)
        {
            if (!e.IsError)
                this.dispatchDownload(e.XmlContent);
        }

        void _networkHelper_DownloadInBackgroundComplete(object sender, BombaJobEventArgs e)
        {
            if (!e.IsError)
            {
                if (e.ServiceOp > 0)
                    this.currentOp = e.ServiceOp;
                this.dispatchDownload(e.XmlContent);
            }
        }

        void _networkHelper_DownloadError(object sender, BombaJobEventArgs e)
        {
            this.SyncError(this, new BombaJobEventArgs(true, e.ErrorMessage, ""));
        }
        #endregion

        #region Initial Synchronization
        public void StartSync()
        {
            this.syncTexts();
        }

        private void doTexts(string xmlContent)
        {
            IBombaJobRepository repo = new BombaJobRepository();
            XDocument doc = XDocument.Parse(xmlContent);
            foreach (XElement txt in doc.Descendants("tctxt"))
            {
                var t = new Text
                {
                    TextID = (int)txt.Attribute("id"),
                    Title = (string)txt.Element("tctitle"),
                    Content = (string)txt.Element("tccontent")
                };
                repo.AddText(t);
            }
            this.syncCategories();
        }

        private void doCategories(string xmlContent)
        {
            IBombaJobRepository repo = new BombaJobRepository();
            XDocument doc = XDocument.Parse(xmlContent);
            foreach (XElement cat in doc.Descendants("cat"))
            {
                var t = new Category
                {
                    CategoryID = (int)cat.Attribute("id"),
                    Title = (string)cat.Element("cttl"),
                    OffersCount = (int)cat.Attribute("cnt")
                };
                repo.AddCategory(t);
            }
            this.syncNewestOffers();
        }

        private void doNewestOffers(string xmlContent)
        {
            IBombaJobRepository repo = new BombaJobRepository();
            XDocument doc = XDocument.Parse(xmlContent);
            foreach (XElement job in doc.Descendants("job"))
            {
                var t = new JobOffer
                {
                    OfferID = (int)job.Attribute("id"),
                    CategoryID = (int)job.Attribute("cid"),
                    HumanYn = (bool)job.Attribute("hm"),
                    FreelanceYn = (bool)job.Attribute("fyn"),
                    Title = (string)job.Element("jottl"),
                    Email = (string)job.Element("joem"),
                    CategoryTitle = (string)job.Element("jocat"),
                    Positivism = (string)job.Element("jopos"),
                    Negativism = (string)job.Element("joneg"),
                    PublishDate = DateTime.ParseExact((string)job.Element("jodt"), AppSettings.DateTimeFormat, null)
                };
                repo.AddJobOffer(t);
            }
            this.SynchronizationComplete();
        }

        public int GetJobOffersCount()
        {
            IBombaJobRepository repo = new BombaJobRepository();
            return repo.GetJobOffersCount();
        }
        #endregion

        #region Parsers
        private void doJobOffers(string xmlContent)
        {
            if (this.currentOp != AppSettings.ServiceOp.ServiceOpJobs)
                this.SyncComplete(this, new BombaJobEventArgs(false, "", ""));
        }
        #endregion
    }
}
