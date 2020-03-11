using AutoFeedbackWindows10.UI.DataProviders;
using AutoFeedbackWindows10.UI.Models;
using AutoFeedbackWindows10.UI.Utils;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AutoFeedbackWindows10.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginForm : Page
    {
        // TODO: Implement relogin in main page
        // we might need to change web view logic to a user control
        // add saved cookie then navigate to accounts auth server

        HttpBaseProtocolFilter httpBase = new HttpBaseProtocolFilter();
        public ObservableCollection<AccountModel> SavedAccounts = new ObservableCollection<AccountModel>();
        public const string GoogleLoginLink = @"https://accounts.google.com/o/oauth2/auth?scope=profile%20email&state=%2Fprofile&redirect_uri=http://fschool.fpt.edu.vn/LoginPage/Login.aspx&response_type=code&client_id=699800698114-ahs58mmqlmscei6tvhdcabidotspimif.apps.googleusercontent.com&approval_prompt=auto&access_type=offline";
        public const string FschoolDomain = "http://fschool.fpt.edu.vn";
        private const string GoogleAccountDomain = "https://accounts.google.com";
        public const string SessionIDCookieName = "ASP.NET_SessionId";

        public LoginForm()
        {
            this.InitializeComponent();
        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if ((string)e.Parameter == CommonPageCommand.CannotGoBackLoginPage)
                btnGoBack.Visibility = Visibility.Collapsed;

            webLogin.Navigate(new Uri(GoogleLoginLink));
            savedLogin.SetAccountPool(await AccountProvider.GetAccounts());
        }

        private async void webLogin_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if(args.Uri.ToString().Contains( $"{FschoolDomain}/DefaultPage/StudentDefaultPage.aspx"))
            {
                args.Cancel = true;
                string uriString = args.Uri.ToString();
                webLogin.Stop();

                string sessionID = httpBase.CookieManager.GetCookies(new Uri(FschoolDomain)).FirstOrDefault(x => x.Name == SessionIDCookieName).Value;
                var cookies = LoginHelper.GetAndDeleteLoginCookie();

                // We can kinda *guess* the email address by the student id
                string email = uriString.Substring(uriString.IndexOf("=") + 1).Replace("#", "") + "@fpt.edu.vn";
                string name = await AccountModel.GetAccountName(sessionID);
                if (!string.IsNullOrEmpty(name))
                {
                    var createdAccount = await AccountProvider.AddOrUpdateAccountAsync(name, email, sessionID, cookies);
                    await AccountProvider.SetActiveAccountAsync(createdAccount);
                    Frame.GoBack(new DrillInNavigationTransitionInfo());
                }
                else
                {
                    ContentDialog failedToLoginDialog = new ContentDialog()
                    {
                        Content = "Sorry, we can't login using this account, please try again",
                        CloseButtonText = "OK"
                    };
                    await failedToLoginDialog.ShowAsync();
                    LoginHelper.GetAndDeleteLoginCookie();
                    webLogin.Navigate(new Uri(GoogleLoginLink));
                }
            }
        }

        

        private void Github_Click(object sender, RoutedEventArgs e) => Process.Start("https://github.com/quangaming2929/FschoolAutoFeedbackWindows10");

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = sender as ToggleButton;
            bdSavedAccount.Opacity = btn.IsChecked == true ? 1 : 0;
        }

        private async void SavedAccunts_DeleteAccount(object sender, ValueEventArgs<AccountModel> e)
        {
            await AccountProvider.RemoveAccountAsync(e.Value);
        }

        private void SavedAccounts_SelecteAccount(object sender, ValueEventArgs<AccountModel> e)
        {
            if(e.Value.Cookies != null)
            {
                LoginHelper.GetAndDeleteLoginCookie();
                LoginHelper.SetLoginCookies(e.Value.Cookies);
                webLogin.Navigate(new Uri(GoogleLoginLink));
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack(new DrillInNavigationTransitionInfo());
        }
    }
}
