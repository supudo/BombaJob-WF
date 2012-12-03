using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using BombaJob.ViewModels;

using Caliburn.Micro;

namespace BombaJob.Utilities.Controls
{
    public static class WindowManagerExtensions
    {
        public static MessageBoxResult ShowMessageBox(this IWindowManager @this, string message, string title, MessageBoxButton buttons)
        {
            MessageBoxResult retval;
            ShellViewModel shellViewModel = IoC.Get<ShellViewModel>();
            try
            {
                shellViewModel.ShowOverlay();
                var model = new MessageBoxViewModel(message, title, buttons);
                @this.ShowDialog(model);

                retval = model.Result;
            }
            finally
            {
                shellViewModel.HideOverlay();
            }

            return retval;
        }

        public static void ShowMessageBox(this IWindowManager @this, string message)
        {
            @this.ShowMessageBox(message, "System Message", MessageBoxButton.OK);
        }
    }
}
