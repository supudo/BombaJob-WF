using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Caliburn.Micro;

namespace BombaJob.Utilities.Events
{
    public class HyperlinkEvent
    {
        public string URL { get; private set; }

        public HyperlinkEvent(string _url)
        {
            this.URL = _url;
        }

        public override string ToString()
        {
            return this.URL;
        }
    }
}
