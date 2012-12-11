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
        private string searchQuery;
        private ShellViewModel shellVM;
        public bool showSearch;

        [ImportingConstructor]
        public TabberViewModel(ShellViewModel _shell)
        {
            this.shellVM = _shell;
            this.DisplayName = Resources.appName;
            this.showSearch = false;
            this.RefreshTabs(false);
        }

        #region Init
        public void RefreshTabs(bool _showSearch, string _searchQuery)
        {
            this.searchQuery = _searchQuery;
            this.RefreshTabs(_showSearch);
        }

        public void RefreshTabs(bool showSearch)
        {
            Items.Clear();

            Items.Add(new NewestOffersViewModel(this));
            Items.Add(new JobOffersViewModel(this));
            Items.Add(new PeopleOffersViewModel(this));
            Items.Add(new PostViewModel(this));

            if (showSearch)
            {
                Items.Add(new SearchResultsViewModel(this, this.searchQuery));
                ActivateItem(Items.Last());
            }
            else
                ActivateItem(Items.First());
        }
        #endregion

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

        public void OffersList_Menu_View(JobOffer jobOffer)
        {
            this.OffersList_SelectionChanged(jobOffer);
        }

        public void OffersList_Menu_Message(JobOffer jobOffer)
        {
            SendMessageViewModel od = new SendMessageViewModel(jobOffer);
            ActivateItem(od);
        }
        #endregion

        #region Shell gateway
        public void StartLoading()
        {
            this.shellVM.ShowOverlay();
        }

        public void StopLoading()
        {
            this.shellVM.HideOverlay();
        }
        #endregion
    }
}
