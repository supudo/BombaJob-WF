using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;

namespace BombaJob.Utilities.Network
{
    public class NetworkHelper
    {
        public delegate void EventHandler(Object sender, BombaJobEventArgs e);
        public event EventHandler DownloadComplete;
        public event EventHandler DownloadError;
        public event EventHandler DownloadInBackgroundComplete;

        private WebClient webClient;

        public bool InBackground;
        private AppSettings.ServiceOp webServiceOp;

        #region Constructor
        public NetworkHelper()
        {
            this.webClient = new WebClient();
            this.webClient.Encoding = Encoding.UTF8;
            this.webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.webClient_DownloadStringCompleted);
            this.InBackground = false;
        }
        #endregion

        #region Helpers
        private bool hasConnection()
        {
            return this.hasConnection(AppSettings.ServicesURL);
        }

        private bool hasConnection(String URL)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Timeout = 5000;
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region GET
        public void downloadURL(string url)
        {
            this.downloadURL(url, false);
        }

        public void downloadURL(string url, bool inBackground)
        {
            this.downloadURL(url, inBackground, 0);
        }

        public void downloadURL(string url, bool inBackground, AppSettings.ServiceOp serviceOp)
        {
            this.InBackground = inBackground;
            this.webServiceOp = serviceOp;

            if (this.hasConnection())
                this.webClient.DownloadStringAsync(new System.Uri(url));
            else
                this.DownloadError(this, new BombaJobEventArgs(true, Properties.Resources.error_NoInternet, ""));
        }

        private void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                this.DownloadError(this, new BombaJobEventArgs(true, e.Error.Message, ""));
            else if (this.InBackground)
                this.DownloadInBackgroundComplete(this, new BombaJobEventArgs(false, "", e.Result, this.webServiceOp));
            else
                this.DownloadComplete(this, new BombaJobEventArgs(false, "", e.Result));
        }
        #endregion

        #region POST
        public void uploadURL(string url, Dictionary<string, string> postArray)
        {
            if (this.hasConnection())
            {
                this.webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var uri = new Uri(url, UriKind.Absolute);

                StringBuilder postData = new StringBuilder();
                int c = 0;
                foreach (KeyValuePair<string, string> kvp in postArray)
                {
                    postData.AppendFormat((c > 0 ? "&" : "") + "{0}={1}", kvp.Key, System.Web.HttpUtility.UrlEncode(kvp.Value));
                    c++;
                }

                this.webClient.Headers[HttpRequestHeader.ContentLength] = postData.Length.ToString();
                this.webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(webClient_UploadStringCompleted);
                this.webClient.UploadStringAsync(uri, "POST", postData.ToString());
            }
            else
            {
                this.DownloadError(this, new BombaJobEventArgs(true, Properties.Resources.error_NoInternet, ""));
            }
        }

        void webClient_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                this.DownloadError(this, new BombaJobEventArgs(true, e.Error.Message, ""));
            else
                this.DownloadComplete(this, new BombaJobEventArgs(false, "", e.Result));
        }
        #endregion
    }
}
