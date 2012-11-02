using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BombaJob.Utilities
{
    public class BombaJobEventArgs : EventArgs
    {
        private AppSettings.ServiceOp serviceOp;
        private bool isError;
        private string errorMessage;
        private string xmlContent;

        public BombaJobEventArgs(bool ise, string eMsg, string xml)
        {
            this.isError = ise;
            this.errorMessage = eMsg;
            this.xmlContent = xml;
            this.serviceOp = 0;
        }

        public BombaJobEventArgs(bool ise, string eMsg, string xml, AppSettings.ServiceOp sOp)
        {
            this.isError = ise;
            this.errorMessage = eMsg;
            this.xmlContent = xml;
            this.serviceOp = sOp;
        }

        public bool IsError
        {
            get { return this.isError; }
        }

        public string ErrorMessage
        {
            get { return this.errorMessage; }
        }

        public string XmlContent
        {
            get { return this.xmlContent; }
        }

        public AppSettings.ServiceOp ServiceOp
        {
            get { return this.serviceOp; }
        }
    }
}
