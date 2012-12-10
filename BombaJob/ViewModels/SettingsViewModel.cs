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

        public bool stPrivateData
        {
            get
            {
                return Properties.Settings.Default.stPrivateData;
            }
            set
            {
                Properties.Settings.Default.stPrivateData = value;
            }
        }

        public bool stInitSync
        {
            get
            {
                return Properties.Settings.Default.stInitSync;
            }
            set
            {
                Properties.Settings.Default.stInitSync = value;
            }
        }

        public bool stOnlineSearch
        {
            get
            {
                return Properties.Settings.Default.stOnlineSearch;
            }
            set
            {
                Properties.Settings.Default.stOnlineSearch = value;
            }
        }

        public bool stInAppEmail
        {
            get
            {
                return Properties.Settings.Default.stInAppEmail;
            }
            set
            {
                Properties.Settings.Default.stInAppEmail = value;
            }
        }

        public bool stShowCategories
        {
            get
            {
                return Properties.Settings.Default.stShowCategories;
            }
            set
            {
                Properties.Settings.Default.stShowCategories = value;
            }
        }

        public SettingsViewModel(ShellViewModel _shell)
        {
            this.vmShell = _shell;
            this.DisplayName = Properties.Resources.menu_Settings;
        }

        public void SaveSetting()
        {
            Properties.Settings.Default.Save();
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.Save();
            this.vmShell.ActivateTabber();
        }
    }
}
