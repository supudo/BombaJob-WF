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
        }

        public void LoadText()
        {
            BombaJob.Database.Domain.Text t = this.dbRepo.GetTextById(35);
            if (t != null)
                this.AboutText = t.Content;
        }

        public void CloseAbout()
        {
            this.vmShell.ActivateTabber();
        }
    }
}
