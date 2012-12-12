using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using BombaJob.Database.Domain;
using BombaJob.Database.Repository;
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
        #endregion

        #region Properties
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

        private string offerTitle;
        [Required]
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
        [RegularExpression(@"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$")]
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
        [Required]
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
        [Required]
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
        }

        public void SetLabels(RadioButton rb)
        {
            this.SetLabels(int.Parse(rb.Tag.ToString()) == 1);
        }

        public void SetLabels(bool humanYn)
        {
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
            AppSettings.LogThis("Post offer... " + this.SelectedCategory.CategoryID);
        }
        #endregion
    }
}
