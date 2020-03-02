using AutoFeedbackWindows10.UI.DataProviders;
using AutoFeedbackWindows10.UI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
        HttpBaseProtocolFilter httpBase = new HttpBaseProtocolFilter();
        public const string GoogleLoginLink = @"https://accounts.google.com/o/oauth2/auth?scope=profile%20email&state=%2Fprofile&redirect_uri=http://fschool.fpt.edu.vn/LoginPage/Login.aspx&response_type=code&client_id=699800698114-ahs58mmqlmscei6tvhdcabidotspimif.apps.googleusercontent.com&approval_prompt=auto&access_type=offline";

        public LoginForm()
        {
            this.InitializeComponent();
        }


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            webLogin.Navigate(new Uri(GoogleLoginLink));
        }

        private async void webLogin_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if(args.Uri.ToString().Contains("http://fschool.fpt.edu.vn/DefaultPage/StudentDefaultPage.aspx"))
            {
                args.Cancel = true;
                string uriString = args.Uri.ToString();

                string sessionID = httpBase.CookieManager.GetCookies(new Uri("http://fschool.fpt.edu.vn")).FirstOrDefault(x => x.Name == "ASP.NET_SessionId").Value;

                Dictionary<string, string> cookies = new Dictionary<string, string>();
                foreach (var item in httpBase.CookieManager.GetCookies(new Uri("https://accounts.google.com")))
                {
                    cookies.Add(item.Name, item.Value);
                    httpBase.CookieManager.DeleteCookie(item);
                }

                // We can kinda *guess* the email address by the student id
                string email = uriString.Substring(uriString.IndexOf("=") + 1).Replace("#", "");

                var createdAccount = await AccountProvider.AddOrUpdateAccountAsync(email, sessionID, cookies);
                await AccountProvider.SetActiveAccountAsync(createdAccount);
            }
        }
    }
}
