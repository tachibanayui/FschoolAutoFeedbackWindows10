using AutoFeedbackWindows10.UI.Models;
using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AutoFeedbackWindows10.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BatchFeedbackPage : Page
    {
        private const int ClampBreakPoint = 150;

        public ObservableCollection<FeedbackTeacherModel> Teachers { get; set; } = new ObservableCollection<FeedbackTeacherModel>()
        {
            new FeedbackTeacherModel()
            {
                AcademicYear = "2019-2020",
                ClassName = "11A3-11A3 GDQP 11.1",
                Term = "Kỳ 1 2019.2020",
                TeacherName = "luongdv5",
                FeedbackFor = "Giáo viên Bộ môn",
                OpenDate = "09/12/2019",
            },
        };

        public BatchFeedbackPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            clampTeacherCount.Translation = new Vector3(0, -100, 0);
            clampSendButton.Translation = new Vector3(0, -100, 0);

            var propSet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollRoot);
            var compositor = propSet.Compositor;
            var props = compositor.CreatePropertySet();
            props.InsertScalar("progress", 0);
            props.InsertScalar("clampSize", ClampBreakPoint);

            var propScroll = propSet.GetSpecializedReference<ManipulationPropertySetReferenceNode>();
            var propGet = props.GetReference();
            var progressionNode = propGet.GetScalarProperty("progress");
            var clampSizeNode = propGet.GetScalarProperty("clampSize");

            ExpressionNode progressAnim = ExpressionFunctions.Clamp(-propScroll.Translation.Y / clampSizeNode, 0, 1);
            props.StartAnimation("progress", progressAnim);

            ExpressionNode opacityNode = ExpressionFunctions.Lerp(0, 1, progressionNode);
            ElementCompositionPreview.GetElementVisual(fadeBlackHeader).StartAnimation("Opacity", opacityNode);
        }

        bool isClamped = false;

        private void scrollRoot_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var com = Window.Current.Compositor;
            var anim = com.CreateSpringVector3Animation();
            var anim2 = com.CreateSpringScalarAnimation();
            anim.Target = "Translation";
            anim2.Target = "Opacity";

            if (scrollRoot.VerticalOffset < ClampBreakPoint && isClamped)
            {
                grdHeaderAction.Translation = new Vector3(0, 0, 0);
                grdHeaderAction.Opacity = 1;

                anim.FinalValue = new Vector3(0, -100, 0);
                anim2.InitialValue = 1;
                anim2.FinalValue = 0;
                //clampTeacherCount.StartAnimation(anim2);
                clampTeacherCount.StartAnimation(anim);
                //clampSendButton.StartAnimation(anim2);
                clampSendButton.StartAnimation(anim);
                isClamped = false;

            }
            else if (scrollRoot.VerticalOffset >= ClampBreakPoint && !isClamped)
            {
                grdHeaderAction.Translation = new Vector3(0, 100, 0);
                grdHeaderAction.Opacity = 0;

                anim.FinalValue = new Vector3(0, 0, 0);
                anim2.InitialValue = 0;
                anim2.FinalValue = 1;

                //clampTeacherCount.StartAnimation(anim2);
                clampTeacherCount.StartAnimation(anim);
                anim.DelayTime = TimeSpan.FromMilliseconds(100);
                //clampSendButton.StartAnimation(anim2);
                clampSendButton.StartAnimation(anim);
                isClamped = true;
            }
        }
    }
}
