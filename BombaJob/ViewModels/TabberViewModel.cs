﻿using System;
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
            this.RefreshTabs(showSearch, 0);
        }

        public void RefreshTabs(bool showSearch, int aitem)
        {
            Items.Clear();

            Items.Add(new NewestOffersViewModel(this));
            if (Properties.Settings.Default.stShowCategories)
            {
                Items.Add(new JobOffersCategoriesViewModel(this));
                Items.Add(new PeopleOffersCategoriesViewModel(this));
            }
            else
            {
                Items.Add(new JobOffersViewModel(this));
                Items.Add(new PeopleOffersViewModel(this));
            }
            Items.Add(new PostViewModel(this));

            if (showSearch)
            {
                Items.Add(new SearchResultsViewModel(this, this.searchQuery));
                if (aitem == 0)
                    ActivateItem(Items.Last());
            }
            else if (aitem == 0)
                ActivateItem(Items.First());
            else
            {
                ActivateItem(Items[aitem]);
            }
        }
        #endregion

        #region Tab actions
        public void TabSelected(TabControl tabc)
        {
            if (Items.Count > 4)
            {
                if (tabc != null && tabc.SelectedIndex <= 3)
                    RefreshTabs(false, tabc.SelectedIndex);
            }
        }
        #endregion

        #region Controlls actions
        public void CategoriesList_SelectionChanged(Category cat, bool humanYn)
        {
            if (humanYn)
            {
                PeopleOffersViewModel vm = new PeopleOffersViewModel(this, cat.CategoryID);
                ActivateItem(vm);
            }
            else
            {
                JobOffersViewModel vm = new JobOffersViewModel(this, cat.CategoryID);
                ActivateItem(vm);
            }
        }

        public void OffersList_SelectionChanged(JobOffer jobOffer)
        {
            OfferDetailsViewModel od = new OfferDetailsViewModel(this, jobOffer);
            ActivateItem(od);
        }

        public void OffersList_Menu_View(JobOffer jobOffer)
        {
            this.OffersList_SelectionChanged(jobOffer);
        }

        public void OffersList_Menu_Message(JobOffer jobOffer)
        {
            SendMessageViewModel od = new SendMessageViewModel(this, jobOffer);
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
