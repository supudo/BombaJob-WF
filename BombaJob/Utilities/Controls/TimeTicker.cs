using System;
using System.ComponentModel;
using System.Timers;

namespace BombaJob.Utilities.Controls
{
    public class TimeTicker : INotifyPropertyChanged
    {
        public TimeTicker()
        {
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        public String Now
        {
            get { return AppSettings.DoLongDate(DateTime.Now); } // .Now.ToString("H:mm:ss / MM/dd/yyyy")
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Now"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
