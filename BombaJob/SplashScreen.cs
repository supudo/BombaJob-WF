using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BombaJob.Utilities;

namespace BombaJob
{
    public partial class SplashScreen : Form
    {
        public delegate void EventHandler(Object sender, BombaJobEventArgs e);
        public event EventHandler SplashComplete;
        public event EventHandler SplashError;

        private Timer t = new Timer();

        public SplashScreen()
        {
            InitializeComponent();
            this.lblLoading.Text = AppSettings.GetLanguageValue("loading");
            SetAndStartTimer();
        }

        public void StartSync()
        {
            this.Show();
            System.Threading.Thread.Sleep(2);
            this.BeginInvoke(SplashComplete, new BombaJobEventArgs(false, "", ""));
        }

        private void SetAndStartTimer()
        {
            t.Interval = 3000;
            t.Tick += new System.EventHandler(t_Tick);
            t.Start();
        }

        void t_Tick(object sender, EventArgs e)
        {
            if (t.Interval <= 1000)
            {
                t.Stop();
                this.Close();
            }
            else
                t.Interval -= 1000;
        }
    }
}
