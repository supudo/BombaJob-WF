using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

using BombaJob.Database.Repository;
using BombaJob.Utilities.Interfaces;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    [Export(typeof(AboutViewModel))]
    public class AboutViewModel : Screen
    {
        private ShellViewModel vmShell;
        private IBombaJobRepository dbRepo;

        public string AboutText { get; set; }

        public AboutViewModel(ShellViewModel _shell)
        {
            this.vmShell = _shell;
            this.DisplayName = Properties.Resources.menu_About;

            if (this.dbRepo == null)
                this.dbRepo = new BombaJobRepository();

            this.AboutText = this.dbRepo.GetTextById(35).Content;
        }

        public void CloseAbout()
        {
            this.vmShell.ActivateTabber();
        }
    }
}
