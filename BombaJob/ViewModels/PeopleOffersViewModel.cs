﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using BombaJob.Database;
using BombaJob.Database.Domain;
using BombaJob.Utilities.Interfaces;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    [Export(typeof(NewestOffersViewModel))]
    public class PeopleOffersViewModel : Screen
    {
        private IBombaJobRepository dbRepo;
        public ObservableCollection<JobOffer> OffersList { get; set; }

        public PeopleOffersViewModel()
        {
            this.DisplayName = Properties.Resources.menu_People;

            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();
            this.OffersList = this.dbRepo.GetPeopleOffers(AppSettings.OffersPerPage);
        }
    }
}