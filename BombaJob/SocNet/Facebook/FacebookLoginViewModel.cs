using System;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;

using Caliburn.Micro;

using Facebook;

namespace BombaJob.SocNet.Facebook
{
    public class FacebookLoginViewModel : Screen
    {
        private Uri _loginUrl;
        private FacebookClient _fb;

        public WebBrowser wbFacebook { get; set; }
        public FacebookOAuthResult FacebookOAuthResult { get; private set; }

        public FacebookLoginViewModel()
        {
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

        void wbFacebook_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            FacebookOAuthResult oauthResult;
            if (_fb.TryParseOAuthCallbackUrl(e.Uri, out oauthResult))
            {
                this.FacebookOAuthResult = oauthResult;
                this.TryClose();
            }
            else
                this.FacebookOAuthResult = null;
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
    }
}
