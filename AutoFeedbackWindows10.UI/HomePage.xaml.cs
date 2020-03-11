using AutoFeedbackWindows10.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
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
    public sealed partial class HomePage : Page
    {
        public ObservableCollection<ContributorModel> Contributors { get; set; } = new ObservableCollection<ContributorModel>();
        public ObservableCollection<FeatureModel> Features { get; set; } = new ObservableCollection<FeatureModel>();
        public Compositor CurrentCompositor => Window.Current.Compositor;

        public HomePage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ContributorModel tachiProfile = new ContributorModel()
            {
                Name = "Tachibana Yui",
                ProfilePicure = "https://avatars3.githubusercontent.com/u/33594017",
                Role = "Project Owner",
                Description = "Hello, my name is Tachibana Yui and thank you for choosing Fschool Auto Feedback Windows 10 Edition. I am a hobbyist programmer for some reason also love Japanese cultures. I came to the programming world after watching an anime series called \"Sword Art Online\" after that, I always want to create my dream game. I have 3.5 years of experience working with .NET and C# technology. In recent years, I have created a few programs, automated tools to improve overall watching anime experience. It comprises of 3 projects, but unfortunately, 2 of them are discontinued due to lack of maintainability. If you are interested in this application, please check out \"Universal Anime Downloader\" in my Github Page. In the past few months, I also curious about Minecraft mod development, so I also have a quick dive into Java programming language and created a program called \"MineCLIA\" or Minecraft Command Line Automation. It lets you send commands to Minecraft client via command line or socket. Check it out on my GitHub page.",
                Link = "https://github.com/quangaming2929",
            };

            Contributors.Add(tachiProfile);

            FeatureModel fbWoHassle = new FeatureModel()
            {
                Glyph = "\uE899",
                Title = "Quickly feedback without hassle",
                Detail = "Fschool auto feedback windows 10 edition help you feedback without the need to painstakingly go through each teacher. You can feedback all teacher with a few simple click"
            };

            FeatureModel fbSpeed = new FeatureModel()
            {
                Glyph = "\uEC4A",
                Title = "Blazing fast speed",
                Detail = "With batch feedback, you can feedback all teacher significantly faster when you feedback on the offical website. Fschool auto feedback windows 10 edition can feedback 10 teachers per second after you hit send (actual speed may vary)"
            };

            FeatureModel fbLogin = new FeatureModel()
            {
                Glyph = "\uE779",
                Title = "Login with ease",
                Detail = "Unlike fschool.fpt.edu.vn where you need to login each time you visit the site. This program only requires you to login once, all the authorization is done automatically until you sign out or revoke the program access"
            };

            FeatureModel fbUsability = new FeatureModel()
            {
                Glyph = "\uE8CB",
                Title = "Advanced filtering",
                Detail = "Fschool Autofeedback windows 10 edition is highly customizable. You can search teacher like in offical website but you also can filter, exclude who will be feedback. Save a draft to feedback later,...  "
            };

            Features.Add(fbWoHassle);
            Features.Add(fbSpeed);
            Features.Add(fbLogin);
            Features.Add(fbUsability);
        }

        private void btnWindow10_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/quangaming2929/FschoolAutoFeedbackWindows10");
        }

        private void btnLegacy_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/quangaming2929/AutoFeedbackFschool");
        }
    }
}
