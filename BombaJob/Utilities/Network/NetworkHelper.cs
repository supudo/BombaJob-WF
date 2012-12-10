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

using BombaJob.Utilities.Events;

namespace BombaJob.Utilities.Network
{
    public class NetworkHelper
    {
        public delegate void EventHandler(Object sender, BombaJobEventArgs e);
        public event EventHandler DownloadComplete;
        public event EventHandler DownloadError;
        public event EventHandler DownloadInBackgroundComplete;
        public bool InBackground;

        private WebClient webClient;
        private WebRequest webRequest;
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
        public bool hasConnection()
        {
            return this.hasConnection(AppSettings.ServicesURL);
        }

        private bool hasConnection(String URL)
        {
            try
            {
                AppSettings.LogThis("NET hasConnection... ");
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
                var uri = new Uri(url, UriKind.Absolute);

                StringBuilder postData = new StringBuilder();
                int c = 0;
                foreach (KeyValuePair<string, string> kvp in postArray)
                {
                    postData.AppendFormat((c > 0 ? "&" : "") + "{0}={1}", kvp.Key, System.Web.HttpUtility.UrlEncode(kvp.Value));
                    c++;
                }

                this.webRequest = System.Net.WebRequest.Create(uri);
                this.webRequest.ContentType = "application/x-www-form-urlencoded";
                this.webRequest.Method = "POST";

                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(postData.ToString());
                this.webRequest.ContentLength = bytes.Length;
                System.IO.Stream os = this.webRequest.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);
                os.Close();

                System.Net.WebResponse resp = this.webRequest.GetResponse();
                if (resp == null)
                    this.DownloadError(this, new BombaJobEventArgs(true, Properties.Resources.error_NoInternet, ""));
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                this.DownloadComplete(this, new BombaJobEventArgs(false, "", sr.ReadToEnd().Trim()));
            }
            else
                this.DownloadError(this, new BombaJobEventArgs(true, Properties.Resources.error_NoInternet, ""));
        }
        #endregion
    }
}
