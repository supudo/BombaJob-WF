using System;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;

using BombaJob.Utilities.Controls;

using Caliburn.Micro;

using Hammock;
using Hammock.Authentication.OAuth;
using Hammock.Web;

namespace BombaJob.SocNet.Twitter
{
    public class TwitterLoginViewModel : Screen
    {
        private string _oAuthToken;
        private string _oAuthTokenSecret;
        public WebBrowser wbTwitter { get; set; }

        public TwitterLoginViewModel()
        {
            this.DisplayName = Properties.Resources.appName;
            this.wbTwitter = new WebBrowser();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var itemView = view as TwitterLoginView;
            if (itemView != null)
                this.wbTwitter = itemView.wbTwitter;
            this.LoadLogin();
        }

        public void LoadLogin()
        {
            this.wbTwitter.Navigating += new System.Windows.Navigation.NavigatingCancelEventHandler(wbTwitter_Navigating);
            this.GetTwitterToken();
        }

        private void GetTwitterToken()
        {
            var credentials = new OAuthCredentials
            {
                Type = OAuthType.RequestToken,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                ConsumerKey = AppSettings.TwitterConsumerKey,
                ConsumerSecret = AppSettings.TwitterConsumerKeySecret,
                Version = AppSettings.TwitterOAuthVersion,
                CallbackUrl = AppSettings.TwitterCallbackUri
            };

            var client = new RestClient
            {
                Authority = AppSettings.TwitterOAuth,
                Credentials = credentials
            };

            var request = new RestRequest
            {
                Path = "/request_token"
            };
            client.BeginRequest(request, new RestCallback(TwitterRequestTokenCompleted));
        }

        private void TwitterRequestTokenCompleted(RestRequest request, RestResponse response, object userstate)
        {
            this._oAuthToken = GetQueryParameter(response.Content, "oauth_token");
            this._oAuthTokenSecret = GetQueryParameter(response.Content, "oauth_token_secret");
            var authorizeUrl = AppSettings.TwitterAuthorizeUri + "?oauth_token=" + this._oAuthToken;

            if (String.IsNullOrEmpty(this._oAuthToken) || String.IsNullOrEmpty(this._oAuthTokenSecret))
            {
                Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowMessageBox("error calling twitter", Properties.Resources.errorTitle, MessageBoxButton.OK));
                return;
            }

            Caliburn.Micro.Execute.OnUIThread(() => this.wbTwitter.Navigate(new Uri(authorizeUrl)));
        }

        private void GetAccessToken(string uri)
        {
            var requestToken = GetQueryParameter(uri, "oauth_token");
            if (requestToken != this._oAuthToken)
                Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowMessageBox("Twitter auth tokens don't match", Properties.Resources.errorTitle, MessageBoxButton.OK));

            var requestVerifier = GetQueryParameter(uri, "oauth_verifier");

            var credentials = new OAuthCredentials
            {
                Type = OAuthType.AccessToken,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                ConsumerKey = AppSettings.TwitterConsumerKey,
                ConsumerSecret = AppSettings.TwitterConsumerKeySecret,
                Token = this._oAuthToken,
                TokenSecret = this._oAuthTokenSecret,
                Verifier = requestVerifier
            };

            var client = new RestClient
            {
                Authority = AppSettings.TwitterOAuth,
                Credentials = credentials
            };

            var request = new RestRequest
            {
                Path = "/access_token",
                Method = WebMethod.Post
            };

            client.BeginRequest(request, new RestCallback(RequestAccessTokenCompleted));
        }

        private void RequestAccessTokenCompleted(RestRequest request, RestResponse response, object userstate)
        {
            var twitteruser = new TwitterAccess
            {
                AccessToken = GetQueryParameter(response.Content, "oauth_token"),
                AccessTokenSecret = GetQueryParameter(response.Content, "oauth_token_secret"),
                UserId = GetQueryParameter(response.Content, "user_id"),
                ScreenName = GetQueryParameter(response.Content, "screen_name")
            };

            if (String.IsNullOrEmpty(twitteruser.AccessToken) || String.IsNullOrEmpty(twitteruser.AccessTokenSecret))
            {
                Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowMessageBox(response.Content, Properties.Resources.errorTitle, MessageBoxButton.OK));
                return;
            }

            //Helper.SaveSetting(Constants.TwitterAccess, twitteruser);
            this.TryClose();
        }

        private static string GetQueryParameter(string input, string parameterName)
        {
            foreach (string item in input.Split('&'))
            {
                var parts = item.Split('=');
                if (parts[0] == parameterName)
                    return parts[1];
            }
            return String.Empty;
        }

        void wbTwitter_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.Uri.AbsoluteUri.CompareTo(AppSettings.TwitterAuthorizeUri) == 0)
            {
            }

            if (!e.Uri.AbsoluteUri.Contains(AppSettings.TwitterCallbackUri))
                return;

            e.Cancel = true;

            var arguments = e.Uri.AbsoluteUri.Split('?');
            if (arguments.Length < 1)
                return;

            GetAccessToken(arguments[1]);
        }
    }
}
