using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;

using BombaJob.Utilities.Adorners;

using MahApps.Metro;
using MahApps.Metro.Controls;

namespace BombaJob.Views
{
    public partial class MessageBoxView : MetroWindow
    {
        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MOVE = 0xF010;

        public MessageBoxView()
        {
            InitializeComponent();
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Orange"), Theme.Light);

            this.SourceInitialized += new EventHandler(MessageBoxView_SourceInitialized);
            this.Loaded += new System.Windows.RoutedEventHandler(MessageBoxView_Loaded);
        }

        void MessageBoxView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //AdornerLayer al = AdornerLayer.GetAdornerLayer(this.cnt);
            //al.Add(new OverlayAdorner(this.cnt));
        }

        void MessageBoxView_SourceInitialized(object sender, EventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            HwndSource source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_SYSCOMMAND:
                    int command = wParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                        handled = true;
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }
    }
}
