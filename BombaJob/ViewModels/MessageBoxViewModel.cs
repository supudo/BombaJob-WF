using System;
using System.Windows;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    public class MessageBoxViewModel : Screen
    {
        #region Variables
        private MessageBoxButton _buttons = MessageBoxButton.OK;
        private string _message;
        private string _title;
        private MessageBoxResult _result = MessageBoxResult.None;
        #endregion

        #region Properties
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        public MessageBoxButton Buttons
        {
            get { return _buttons; }
            set
            {
                _buttons = value;
                NotifyOfPropertyChange(() => IsNoButtonVisible);
                NotifyOfPropertyChange(() => IsYesButtonVisible);
                NotifyOfPropertyChange(() => IsCancelButtonVisible);
                NotifyOfPropertyChange(() => IsOkButtonVisible);
            }
        }

        public MessageBoxResult Result { get { return _result; } }
        #endregion

        #region Init
        public MessageBoxViewModel(string message, string title, MessageBoxButton buttons)
        {
            this.DisplayName = "";
            Title = title;
            Message = message;
            Buttons = buttons;
        }
        #endregion

        #region Is
        public bool IsNoButtonVisible
        {
            get { return _buttons == MessageBoxButton.YesNo || _buttons == MessageBoxButton.YesNoCancel; }
        }

        public bool IsYesButtonVisible
        {
            get { return _buttons == MessageBoxButton.YesNo || _buttons == MessageBoxButton.YesNoCancel; }
        }

        public bool IsCancelButtonVisible
        {
            get { return _buttons == MessageBoxButton.OKCancel || _buttons == MessageBoxButton.YesNoCancel; }
        }

        public bool IsOkButtonVisible
        {
            get { return _buttons == MessageBoxButton.OK || _buttons == MessageBoxButton.OKCancel; }
        }
        #endregion

        #region Buttons
        public void No()
        {
            _result = MessageBoxResult.No;
            TryClose(false);
        }

        public void Yes()
        {
            _result = MessageBoxResult.Yes;
            TryClose(true);
        }

        public void Cancel()
        {
            _result = MessageBoxResult.Cancel;
            TryClose(false);
        }

        public void Ok()
        {
            _result = MessageBoxResult.OK;
            TryClose(true);
        }
        #endregion
    }
}