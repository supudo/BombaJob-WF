﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using BombaJob.Database;
using BombaJob.Database.Domain;
using BombaJob.Database.Repository;
using BombaJob.Utilities.Interfaces;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    [Export(typeof(PeopleOffersCategoriesViewModel))]
    public class PeopleOffersCategoriesViewModel : Screen
    {
        private IBombaJobRepository dbRepo;
        private TabberViewModel tabm;
        public ObservableCollection<Category> CategoriesList { get; set; }

        public PeopleOffersCategoriesViewModel(TabberViewModel _tabm)
        {
            this.tabm = _tabm;
            this.DisplayName = Properties.Resources.menu_People;

            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();
            this.LoadCategories();
        }

        private void LoadCategories()
        {
            this.CategoriesList = this.dbRepo.GetCategoriesFor(true);
            NotifyOfPropertyChange(() => CategoriesList);
        }
    }
}
