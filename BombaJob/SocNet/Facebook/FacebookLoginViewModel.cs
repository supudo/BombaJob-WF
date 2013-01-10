using System;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;

using BombaJob.Database.Domain;
using BombaJob.Utilities.Controls;

using Caliburn.Micro;

using Facebook;

namespace BombaJob.SocNet.Facebook
{
    public class FacebookLoginViewModel : Screen
    {
        private Uri _loginUrl;
        private FacebookClient _fb;
        private JobOffer currentOffer;

        public WebBrowser wbFacebook { get; set; }
        public FacebookOAuthResult FacebookOAuthResult { get; private set; }

        public FacebookLoginViewModel(JobOffer jobOffer)
        {
            this.currentOffer = jobOffer;
            this.DisplayName = Properties.Resources.appName;
            this.wbFacebook = new WebBrowser();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var itemView = view as FacebookLoginView;
            if (itemView != null)
                this.wbFacebook = itemView.wbFacebook;
            this.LoadLogin();
        }

        public void LoadLogin()
        {
            this._fb = new FacebookClient();
            this._loginUrl = GenerateLoginUrl(AppSettings.FacebookAppID, AppSettings.FacebookPermissions);
            AppSettings.LogThis("Facebook URL = " + this._loginUrl);
            this.wbFacebook.Navigated += new System.Windows.Navigation.NavigatedEventHandler(wbFacebook_Navigated);
            this.wbFacebook.Navigate(this._loginUrl.AbsoluteUri);
        }

        private Uri GenerateLoginUrl(string appId, string extendedPermissions)
        {
            dynamic parameters = new ExpandoObject();
            parameters.client_id = appId;
            parameters.redirect_uri = "https://www.facebook.com/connect/login_success.html";

            parameters.response_type = "token";
            parameters.display = "popup";

            if (!string.IsNullOrWhiteSpace(extendedPermissions))
                parameters.scope = extendedPermissions;

            return this._fb.GetLoginUrl(parameters);
        }

        void wbFacebook_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs args)
        {
            FacebookOAuthResult oauthResult;
            if (_fb.TryParseOAuthCallbackUrl(args.Uri, out oauthResult))
            {
                this.FacebookOAuthResult = oauthResult;
                if (this.FacebookOAuthResult.IsSuccess)
                {
                    string _accessToken = this.FacebookOAuthResult.AccessToken;
                    var fb = new FacebookClient(_accessToken);

                    fb.PostCompleted += (o, e) =>
                    {
                        if (e.Cancelled)
                            return;
                        else if (e.Error != null)
                            Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowMessageBox(Properties.Resources.share_FacebookError + "\n" + e.Error.Message, Properties.Resources.errorTitle, MessageBoxButton.OK));
                        else
                            Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowMessageBox(Properties.Resources.share_FacebookOK, Properties.Resources.share_Facebook, MessageBoxButton.OK));
                    };

                    dynamic parameters = new ExpandoObject();
                    parameters.picture = "http://bombajob.bg/images/ibombajob.png";
                    parameters.name = this.currentOffer.Title;
                    parameters.link = "http://bombajob.bg/offer/" + this.currentOffer.OfferID;
                    parameters.caption = this.currentOffer.Positivism;
                    parameters.description = this.currentOffer.Negativism;

                    fb.PostAsync("me/feed", parameters);
                    this.TryClose();
                }
            }
            else
                this.FacebookOAuthResult = null;
        }
    }
}
