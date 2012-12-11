using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using BombaJob.Utilities.Interfaces;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    [Export(typeof(PostViewModel))]
    public class PostViewModel : Screen
    {
        private TabberViewModel tabm;
        public string LabelCategory { get; set; }
        public string LabelTitle { get; set; }
        public string LabelEmail { get; set; }
        public string LabelPositiv { get; set; }
        public string LabelNegativ { get; set; }

        public PostViewModel(TabberViewModel _tabm)
        {
            this.tabm = _tabm;
            this.DisplayName = Properties.Resources.menu_Post;
            this.SetLabels(true);
        }

        public PostViewModel()
        {
            this.DisplayName = Properties.Resources.menu_Post;
            this.SetLabels(true);
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
    }
}
