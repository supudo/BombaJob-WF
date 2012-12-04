using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using BombaJob.Properties;
using BombaJob.Database.Domain;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    [Export(typeof(TabberViewModel))]
    public class TabberViewModel : Conductor<IScreen>.Collection.OneActive
    {
        [ImportingConstructor]
        public TabberViewModel()
        {
            this.DisplayName = Resources.appName;

            Items.Add(new NewestOffersViewModel());
            Items.Add(new JobOffersViewModel());
            Items.Add(new PeopleOffersViewModel());
            Items.Add(new PostViewModel());
            ActivateItem(Items.First());
        }

        #region Tab actions
        public void TabSelected(TabControl tabc)
        {
            if (Items.Count > 4)
            {
                if (tabc != null && tabc.SelectedIndex < 3)
                    Items.RemoveAt(Items.Count - 1);
            }
        }
        #endregion

        #region Controlls actions
        public void OffersList_SelectionChanged(JobOffer jobOffer)
        {
            OfferDetailsViewModel od = new OfferDetailsViewModel(jobOffer);
            ActivateItem(od);
        }

        public void OffersList_Menu_View(object e)
        {
            AppSettings.LogThis("--- OffersList_Menu_View ...");
        }
        #endregion
    }
}
