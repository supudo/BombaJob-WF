using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

using BombaJob.Database;
using BombaJob.Database.Domain;
using BombaJob.Utilities.Interfaces;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    [Export(typeof(JobOffersViewModel))]
    public class JobOffersViewModel : Screen
    {
        private IBombaJobRepository dbRepo;
        public ObservableCollection<JobOffer> OffersList { get; set; }

        public JobOffersViewModel()
        {
            this.DisplayName = Properties.Resources.menu_Jobs;

            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();
            this.OffersList = this.dbRepo.GetNewestOffers(AppSettings.OffersPerPage);
        }
    }
}
