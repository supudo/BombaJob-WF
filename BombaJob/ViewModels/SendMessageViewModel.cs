using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

using BombaJob.Database;
using BombaJob.Database.Domain;
using BombaJob.Database.Repository;
using BombaJob.Utilities.Interfaces;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    [Export(typeof(SendMessageViewModel))]
    public class SendMessageViewModel : Screen
    {
        private IBombaJobRepository dbRepo;
        private JobOffer currentOffer;

        public SendMessageViewModel(JobOffer jo)
        {
            this.currentOffer = jo;
            this.DisplayName = " | #" + this.currentOffer.OfferID;

            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();
        }
    }
}
