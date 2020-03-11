using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AutoFeedbackWindows10.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void btnSumbit_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/quangaming2929/FschoolAutoFeedbackWindows10/issues/new");
        }

        private void btnGithub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/quangaming2929/FschoolAutoFeedbackWindows10");    
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            switch (ApplicationData.Current.LocalSettings.Values["theme"].ToString())
            {
                case "0":
                    rbSys.IsChecked = true;
                    break;
                case "1":
                    rbLight.IsChecked = true;
                    break;
                case "2":
                    rbDark.IsChecked = true;
                    break;
            }
        }

        private void rbThemes_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rdb = sender as RadioButton;
            if(Window.Current.Content is FrameworkElement fe)
            {
                switch (rdb.Name)
                {
                    case "rbLight":
                        fe.RequestedTheme = ElementTheme.Light;
                        break;
                    case "rbDark":
                        fe.RequestedTheme = ElementTheme.Dark;
                        break;
                    case "rbSys":
                        fe.RequestedTheme = ElementTheme.Default;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
