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
    [Export(typeof(PostViewModel))]
    public class PostViewModel : Screen
    {
        private IBombaJobRepository dbRepo;
        public ObservableCollection<JobOffer> OffersList { get; set; }

        public PostViewModel()
        {
            this.DisplayName = Properties.Resources.menu_Post;

            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();
        }
    }
}
