using AutoFeedbackWindows10.UI.DataProviders;
using AutoFeedbackWindows10.UI.Models;
using AutoFeedbackWindows10.UI.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Web;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AutoFeedbackWindows10.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string LastPageName = "Home";

        private Dictionary<String, Type> Pages { get; set; } = new Dictionary<string, Type>()
        {
            ["Home"] = typeof(HomePage),
            ["Batch Feedback"] = typeof(BatchFeedbackPage),
            ["Individual Feedback"] = typeof(IndividualFeedbackPage),
            ["Settings"] = typeof(SettingsPage),
        }; 

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            navFrame.Navigate(typeof(HomePage));
            navRoot.SelectedItem = navRoot.MenuItems.FirstOrDefault();
        }

        private async void navRoot_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (Pages.ContainsKey(args.InvokedItem.ToString()))
            {
                if(tutPointer != 0)
                {
                    if(args.InvokedItem.ToString() == "Batch Feedback" && tutPointer == 1)
                    {
                        navFrame.Navigate(typeof(BatchFeedbackPage), "Tut", args.RecommendedNavigationTransitionInfo);
                    }
                    else if(args.InvokedItem.ToString() == "Individual Feedback" && tutPointer == 2)
                    {
                        navFrame.Navigate(typeof(IndividualFeedbackPage), "Tut", args.RecommendedNavigationTransitionInfo);
                    }
                    else
                    {
                        return;
                    }
                }

                navFrame.Navigate(Pages[(string)args.InvokedItem], null, args.RecommendedNavigationTransitionInfo);
                LastPageName = (string)args.InvokedItem;
            }
            else if (args.IsSettingsInvoked)
            {
                navFrame.Navigate(typeof(SettingsPage), null, args.RecommendedNavigationTransitionInfo);
                LastPageName = "Settings";
            }
            else
            {
                await new ContentDialog()
                {
                    Content = "Can't navigate to that page because it doesn't exist!",
                    Title = "Unable to navigate",
                    PrimaryButtonText = "Ok"
                }.ShowAsync();

                navRoot.SelectedItem = LastPageName;
            }
        }

        private async void Account_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavigationViewItem item = sender as NavigationViewItem;
            var account = await AccountProvider.GetActiveAccount();

            if (account != null)
            {
                txblDisplayNameFO.Text = account.Name;
                spAccountFO.DisplayName = account.Name;
                txblEmailFO.Text = account.Email;

                var availAccounts = await GetAvaibleAccounts();
                if (availAccounts.FirstOrDefault() != null) // there is an available account
                {
                    saFlyout.SetAccountPool(availAccounts);
                    pnlAvailableAccounts.Visibility = Visibility.Visible;
                }
                else
                {
                    pnlAvailableAccounts.Visibility = Visibility.Collapsed;
                }

                foAccount.ShowAt(item);
            }
            else
            {
                Frame.Navigate(typeof(LoginForm), null, new DrillInNavigationTransitionInfo());
            }
        }

        private async void Signout_Click(object sender, RoutedEventArgs e)
        {
            foAccount.Hide();
            await AccountProvider.SetActiveAccountAsync(null);
            Frame.Navigate(typeof(LoginForm), CommonPageCommand.CannotGoBackLoginPage, new DrillInNavigationTransitionInfo());
        }

        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginForm), null, new DrillInNavigationTransitionInfo());
        }

        private async Task<IEnumerable<AccountModel>> GetAvaibleAccounts()
        {
            string email = txblEmailFO.Text;
            var allAccounts = await AccountProvider.GetAccounts();
            return allAccounts.Where(x => !x.Email.Contains(email));
        }
        
        private async void SavedAccounts_Delete(object sender, ValueEventArgs<AccountModel> e)
        {
            await AccountProvider.RemoveAccountAsync(e.Value);
        }

        private static bool isAutoLogined;
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(!isAutoLogined)
            {
                // Auto-Login
                isAutoLogined = true;
                var activeAccount = await AccountProvider.GetActiveAccount();
                if (activeAccount != null)
                {
                    prSigningIn.IsActive = true;
                    LoginHelper.SetLoginCookies(activeAccount.Cookies);
                    wvAutoLogin.Navigate(new Uri(LoginHelper.GoogleLoginLink));
                }
                else
                {
                    await Task.Delay(100);
                    Frame.Navigate(typeof(LoginForm), CommonPageCommand.CannotGoBackLoginPage, new DrillInNavigationTransitionInfo());
                }
            }

            // Show tutorial
            if(string.IsNullOrEmpty(ApplicationData.Current.LocalSettings.Values["tut"]?.ToString()))
            {
                ttWelcomeTut.IsOpen = true;
            }
        }

        private async void wvAutoLogin_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri.ToString().Contains($"{LoginHelper.FschoolDomain}/DefaultPage/StudentDefaultPage.aspx"))
            {
                args.Cancel = true;
                string uriString = args.Uri.ToString();

                string sessionID = LoginHelper.GetSessionID();
                wvAutoLogin.Stop();

                var cookies = LoginHelper.GetAndDeleteLoginCookie();

                // We can kinda *guess* the email address by the student id
                string email = uriString.Substring(uriString.IndexOf("=") + 1).Replace("#", "") + "@fpt.edu.vn";
                string name = await AccountModel.GetAccountName(sessionID);

                IsEnabled = true;
                if (!string.IsNullOrEmpty(name))
                {
                    var createdAccount = await AccountProvider.AddOrUpdateAccountAsync(name, email, sessionID, cookies);
                    await AccountProvider.SetActiveAccountAsync(createdAccount);
                    txblLoginGranted.Text = $"Currently logged is as {name}";
                    foLoginGranted.ShowAt(nviAccount, new FlyoutShowOptions() { Placement = FlyoutPlacementMode.Top });
                }
                else
                {
                    Frame.Navigate(typeof(LoginForm), CommonPageCommand.CannotGoBackLoginPage, new DrillInNavigationTransitionInfo());
                }

                prSigningIn.IsActive = false;
            }
        }

        private void wvAutoLogin_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            Frame.Navigate(typeof(LoginForm), CommonPageCommand.CannotGoBackLoginPage, new DrillInNavigationTransitionInfo());
        }




        private void navRoot_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            Frame.GoBack();
        }



        private async void ttWelcomeTut_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            ttWelcomeTut.IsOpen = false;
            await Task.Delay(1000);
            tutPointer = 1;
            ttBatchFeedbackIntro.IsOpen = true;
        }

        private void ttTutorial_CloseBtnClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            ApplicationData.Current.LocalSettings.Values["tut"] = "Played or dismissed";
        }

        int tutPointer = 0;
        private async void btnfbI_Click(object sender, RoutedEventArgs e)
        {
            tutPointer = 2;
            ttBatchFeedbackIntro.IsOpen = false;
            await Task.Delay(1000);
            ttIndividualFeedbackIntro.IsOpen = true;
        }

        private async void btnfbB_Click(object sender, RoutedEventArgs e)
        {
            tutPointer = 1;
            ttIndividualFeedbackIntro.IsOpen = false;
            await Task.Delay(1000);
            ttBatchFeedbackIntro.IsOpen = true;
        }
    }
}
