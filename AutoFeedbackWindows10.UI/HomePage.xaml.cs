using AutoFeedbackWindows10.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                Description = "A wibu programmer :) that happened to like Sword Art Online and start coding :/",
                Link = "https://github.com/quangaming2929",
            };

            ContributorModel ga = new ContributorModel()
            {
                Name = "Tachibana Yui",
                ProfilePicure = "https://avatars3.githubusercontent.com/u/33594017",
                Role = "Project Owner",
                Description = "A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/",
                Link = "https://github.com/quangaming2929",
            };

            ContributorModel gaa = new ContributorModel()
            {
                Name = "Tachibana Yui",
                ProfilePicure = "https://avatars3.githubusercontent.com/u/33594017",
                Role = "Project Owner",
                Description = "A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/A wibu programmer :) that happened to like Sword Art Online and start coding :/",
                Link = "https://github.com/quangaming2929",
            };

            Contributors.Add(tachiProfile);
            Contributors.Add(ga);
            Contributors.Add(gaa);
        }

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Button btn = sender as Button;
            var vec3dAnim = CurrentCompositor.CreateSpringVector3Animation();
            vec3dAnim.FinalValue = new Vector3(2, 2, 2);
            vec3dAnim.Target = "Scale";
            btn.StartAnimation(vec3dAnim);
        }


    }
}
