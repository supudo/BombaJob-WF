using System;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;

using BombaJob.Database.Domain;
using BombaJob.Utilities.Controls;

using Caliburn.Micro;

using TweetSharp;
using TweetSharp.Model;

namespace BombaJob.SocNet.Twitter
{
    public class TwitterLoginViewModel : Screen
    {
        #region Properties
        private TwitterService twService;
        private JobOffer currentOffer;

        public WebBrowser wbTwitter { get; set; }

        public OAuthRequestToken RequestToken
        {
            get;
            private set;
        }

        public OAuthAccessToken AccessToken
        {
            get;
            private set;
        }
        #endregion

        public TwitterLoginViewModel(JobOffer jobOffer)
        {
            this.currentOffer = jobOffer;
            this.DisplayName = Properties.Resources.appName;
            this.wbTwitter = new WebBrowser();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var itemView = view as TwitterLoginView;
            if (itemView != null)
                this.wbTwitter = itemView.wbTwitter;
            this.twService = new TwitterService(AppSettings.TwitterConsumerKey, AppSettings.TwitterConsumerKeySecret);
            if (AppSettings.TwitterToken != null && !AppSettings.TwitterToken.Equals("") &&
                AppSettings.TwitterTokenSecret != null && !AppSettings.TwitterTokenSecret.Equals(""))
                this.PostTweet(AppSettings.TwitterToken, AppSettings.TwitterTokenSecret);
            else
                this.LoadLogin();
        }

        public void LoadLogin()
        {
            this.wbTwitter.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(wbTwitter_LoadCompleted);
            this.RequestToken = this.twService.GetRequestToken();
            Uri uri = this.twService.GetAuthorizationUri(this.RequestToken);
            this.wbTwitter.Navigate(uri);
        }

        void wbTwitter_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (string.Compare(e.Uri.AbsoluteUri, AppSettings.TwitterAuthorizeUri, true) == 0)
            {
                if (!e.Uri.Query.Contains("oauth_token"))
                {
                    try
                    {
                        var doc = this.wbTwitter.Document as mshtml.HTMLDocument;
                        var oauthPinElement = doc.getElementById("oauth_pin") as mshtml.IHTMLElement;
                        if (null != oauthPinElement)
                        {
                            var div = oauthPinElement as mshtml.HTMLDivElement;
                            if (null != div)
                            {
                                var pinText = div.innerText;
                                pinText = pinText.Replace("Next, return to BombaJob and enter this PIN to complete the authorization process:", "");
                                pinText = pinText.Replace("\r\n", "");
                                if (!string.IsNullOrEmpty(pinText))
                                {
                                    this.AccessToken = this.twService.GetAccessToken(this.RequestToken, pinText.Trim());
                                    if (this.AccessToken != null)
                                    {
                                        AppSettings.TwitterToken = this.AccessToken.Token;
                                        AppSettings.TwitterTokenSecret = this.AccessToken.TokenSecret;
                                        this.PostTweet(this.AccessToken.Token, this.AccessToken.TokenSecret);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowMessageBox(Properties.Resources.share_TwitterError + "\n" + ex.Message, Properties.Resources.errorTitle, MessageBoxButton.OK));
                    }
                }
            }
        }

        private void PostTweet(string token, string tokenSecret)
        {
            this.twService.AuthenticateWith(token, tokenSecret);
            string tweet = "BombaJob.bg - ";
            tweet += this.currentOffer.Title + " - ";
            tweet += "http://bombajob.bg/offer/" + this.currentOffer.OfferID;
            tweet += " #bombajobbg";
            this.twService.SendTweet(tweet);
            this.TryClose();
            Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowMessageBox(Properties.Resources.share_TwitterOK, Properties.Resources.share_Twitter, MessageBoxButton.OK));
        }
    }
}
