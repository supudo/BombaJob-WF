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
using BombaJob.Utilities.Events;
using BombaJob.Utilities.Interfaces;
using BombaJob.Utilities.Misc;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    [Export(typeof(PostViewModel))]
    public class PostViewModel : Screen, IDataErrorInfo
    {
        #region Variables
        private readonly IValidator validator = new DefaultValidator();
        private IBombaJobRepository dbRepo;
        private TabberViewModel tabm;
        private Synchronization syncManager;
        private Thread thinkThread;
        #endregion

        #region Properties
        private bool IsHuman = false;
        public string LabelCategory { get; set; }
        public string LabelTitle { get; set; }
        public string LabelEmail { get; set; }
        public string LabelPositiv { get; set; }
        public string LabelNegativ { get; set; }
        public ObservableCollection<Category> Categories { get; set; }
        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get { return this._selectedCategory; }
            set
            {
                this._selectedCategory = value;
                NotifyOfPropertyChange(() => SelectedCategory);
            }
        }

        private bool offerFreelance;
        public bool OfferFreelance
        {
            get { return this.offerFreelance; }
            set
            {
                this.offerFreelance = value;
                NotifyOfPropertyChange(() => OfferFreelance);
            }
        }

        private string offerTitle;
        [Required(ErrorMessageResourceType = typeof(Properties.Resources), ErrorMessageResourceName = "offer_error_Title")]
        public string OfferTitle
        {
            get { return this.offerTitle; }
            set
            {
                this.offerTitle = value;
                NotifyOfPropertyChange(() => OfferTitle);
                NotifyOfPropertyChange(() => CanPostOffer);
            }
        }

        private string offerEmail;
        [Required(ErrorMessageResourceType = typeof(Properties.Resources), ErrorMessageResourceName = "offer_error_Email")]
        [RegularExpression(@"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$", ErrorMessageResourceType = typeof(Properties.Resources), ErrorMessageResourceName = "offer_error_Email_Valid")]
        public string OfferEmail
        {
            get { return this.offerEmail; }
            set
            {
                this.offerEmail = value;
                NotifyOfPropertyChange(() => OfferEmail);
                NotifyOfPropertyChange(() => CanPostOffer);
            }
        }

        private string offerPositiv;
        [Required(ErrorMessageResourceType = typeof(Properties.Resources), ErrorMessageResourceName = "offer_error_Positiv")]
        public string OfferPositiv
        {
            get { return this.offerPositiv; }
            set
            {
                this.offerPositiv = value;
                NotifyOfPropertyChange(() => OfferPositiv);
                NotifyOfPropertyChange(() => CanPostOffer);
            }
        }

        private string offerNegativ;
        [Required(ErrorMessageResourceType = typeof(Properties.Resources), ErrorMessageResourceName = "offer_error_Negativ")]
        public string OfferNegativ
        {
            get { return this.offerNegativ; }
            set
            {
                this.offerNegativ = value;
                NotifyOfPropertyChange(() => OfferNegativ);
                NotifyOfPropertyChange(() => CanPostOffer);
            }
        }
        #endregion

        #region Constructors
        public PostViewModel(TabberViewModel _tabm)
        {
            this.tabm = _tabm;
            this.DisplayName = Properties.Resources.menu_Post;
            this.LoadCategories();
            this.SetLabels(true);
        }

        public PostViewModel()
        {
            this.DisplayName = Properties.Resources.menu_Post;
            this.LoadCategories();
            this.SetLabels(true);
        }
        #endregion

        #region UI
        private void LoadCategories()
        {
            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();
            this.Categories = this.dbRepo.GetCategories();
            NotifyOfPropertyChange(() => Categories);
            this.SelectedCategory = this.Categories[0];
        }

        public void SetLabels(RadioButton rb)
        {
            this.SetLabels(int.Parse(rb.Tag.ToString()) == 1);
        }

        public void SetLabels(bool humanYn)
        {
            this.IsHuman = humanYn;
            if (humanYn)
            {
                this.LabelCategory = Properties.Resources.offer_category_title;
                this.LabelTitle = Properties.Resources.offer_Human_Title;
                this.LabelEmail = Properties.Resources.offer_Human_Email;
                this.LabelPositiv = Properties.Resources.offer_Human_Positiv;
                this.LabelNegativ = Properties.Resources.offer_Human_Negativ;
            }
            else
            {
                this.LabelCategory = Properties.Resources.offer_Category;
                this.LabelTitle = Properties.Resources.offer_Company_Title;
                this.LabelEmail = Properties.Resources.offer_Company_Email;
                this.LabelPositiv = Properties.Resources.offer_Company_Positiv;
                this.LabelNegativ = Properties.Resources.offer_Company_Negativ;
            }
            NotifyOfPropertyChange(() => LabelCategory);
            NotifyOfPropertyChange(() => LabelTitle);
            NotifyOfPropertyChange(() => LabelEmail);
            NotifyOfPropertyChange(() => LabelPositiv);
            NotifyOfPropertyChange(() => LabelNegativ);
        }

        public void SelectedItemChanged(Category cat)
        {
            this.SelectedCategory = cat;
        }
        #endregion

        #region Forms
        public bool CanPostOffer
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

        #region Post
        public void PostOffer()
        {
            this.tabm.StartLoading();
            AppSettings.LogThis("Post offer... " + this.SelectedCategory.CategoryID);
            if (this.syncManager == null)
                this.syncManager = new Synchronization();
            this.syncManager.SyncError += new Synchronization.EventHandler(syncManager_SearchError);
            this.syncManager.SyncComplete += new Synchronization.EventHandler(syncManager_SearchComplete);
            this.thinkThread = new Thread(post);
            this.thinkThread.Start();
        }

        private void post()
        {
            Thread.Sleep(500);
            this.PostFinished();
            /*
            Dictionary<string, string> postParams = new Dictionary<string, string>();
            postParams.Add("oid", "0");
            postParams.Add("cid", "" + this.SelectedCategory.CategoryID);
            postParams.Add("h", (this.IsHuman ? "1" : "0"));
            postParams.Add("fr", (this.OfferFreelance ? "1" : "0"));
            postParams.Add("tt", this.OfferTitle);
            postParams.Add("em", this.OfferEmail);
            postParams.Add("pos", this.OfferPositiv);
            postParams.Add("neg", this.OfferNegativ);
            postParams.Add("mob_app", "win");
            this.syncManager.DoPostOffer(postParams);
             * */
        }

        private void syncManager_SearchComplete(object sender, BombaJobEventArgs e)
        {
            this.PostFinished();
        }

        private void syncManager_SearchError(object sender, BombaJobEventArgs e)
        {
            this.tabm.StopLoading();
            if (e.IsError)
                MessageBox.Show(e.ErrorMessage);
        }

        private void PostFinished()
        {
            MessageBoxResult result = MessageBox.Show(Properties.Resources.offer_ThankYou);
            if (result == MessageBoxResult.OK)
            {
                this.SelectedCategory = this.Categories[0];
                this.IsHuman = true;
                this.SetLabels(this.IsHuman);
                this.OfferFreelance = true;
                this.OfferTitle = "";
                this.OfferEmail = "";
                this.OfferPositiv = "";
                this.OfferNegativ = "";
                NotifyOfPropertyChange(() => SelectedCategory);
                NotifyOfPropertyChange(() => IsHuman);
                NotifyOfPropertyChange(() => OfferFreelance);
                NotifyOfPropertyChange(() => OfferTitle);
                NotifyOfPropertyChange(() => OfferEmail);
                NotifyOfPropertyChange(() => OfferPositiv);
                NotifyOfPropertyChange(() => OfferNegativ);
                this.tabm.StopLoading();
            }
        }
        #endregion
    }
}
