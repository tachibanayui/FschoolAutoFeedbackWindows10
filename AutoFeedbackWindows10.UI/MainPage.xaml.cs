using AutoFeedbackWindows10.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
    }
}
