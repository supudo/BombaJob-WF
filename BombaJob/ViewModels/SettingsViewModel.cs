using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

using BombaJob.Utilities.Interfaces;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    [Export(typeof(SettingsViewModel))]
    public class SettingsViewModel : Screen
    {
        private ShellViewModel vmShell;

        public SettingsViewModel(ShellViewModel _shell)
        {
            this.vmShell = _shell;
            this.DisplayName = Properties.Resources.menu_Settings;
        }

        public void SaveSettings()
        {
            this.vmShell.ActivateTabber();
        }
    }
}
