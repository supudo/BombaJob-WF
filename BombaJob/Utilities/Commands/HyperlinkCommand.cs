using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

using BombaJob.ViewModels;

using Caliburn.Micro;

namespace BombaJob.Utilities.Commands
{
    public class HyperlinkCommand : ICommand
    {
        private Uri URI;

        public HyperlinkCommand(Uri _uri)
        {
            this.URI = _uri;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            AppSettings.LogThis("HyperlinkCommand invoked... " + this.URI.AbsoluteUri);
            try
            {
                System.Diagnostics.Process.Start(this.URI.AbsoluteUri);
            }
            catch
            {
                IWindowManager windowManager;
                try
                {
                    windowManager = IoC.Get<IWindowManager>();
                }
                catch
                {
                    windowManager = new WindowManager();
                }
                MessageBoxViewModel mBox = new MessageBoxViewModel(Properties.Resources.noLinkClient, Properties.Resources.errorTitle, MessageBoxButton.OK);
                windowManager.ShowDialog(mBox);
            }
        }
    }
}
