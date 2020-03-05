using AutoFeedbackWindows10.UI.DataProviders;
using AutoFeedbackWindows10.UI.Models;
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
            ["Home"] = typeof(LoginForm),
            ["Batch Feedback"] = typeof(BatchFeedbackPage),
            ["Individual Feedback"] = typeof(IndividualFeedbackPage)
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
                navFrame.Navigate(Pages[(string)args.InvokedItem], null, args.RecommendedNavigationTransitionInfo);
                LastPageName = (string)args.InvokedItem;
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
            Frame.Navigate(typeof(LoginForm), null, new DrillInNavigationTransitionInfo());
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
    }
}
