using AutoFeedbackWindows10.UI.DataProviders;
using AutoFeedbackWindows10.UI.Models;
using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
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
    public sealed partial class BatchFeedbackPage : Page, INotifyPropertyChanged
    {
        [DoNotNotify]
        private ObservableCollection<BatchedFeedbackModel> Drafts => (Application.Current as App).BatchedFeedBacks;
        [DoNotNotify]
        private FeedbackEntryProvider Provider => (Application.Current as App).FeedbackProvider;

        public ObservableCollection<FeedbackTeacherModel> IncludedTeachers { get; set; } = new ObservableCollection<FeedbackTeacherModel>();
        public BatchedFeedbackModel CurrentBatch { get; set; }

        public string FBStatusCount { get; set; }
        public bool IsLoading = false;
        private Task LoadTeacherTask;

        private const int ClampBreakPoint = 150;

        public BatchFeedbackPage()
        {
            this.InitializeComponent();
        }

        #region Animation
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
        #endregion

        #region Drag n Drop
        private IEnumerable<FeedbackTeacherModel> DragOperationTemp;

        private void GaDTeacher_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            DragOperationTemp = e.Items.Cast<FeedbackTeacherModel>();
            e.Data.RequestedOperation = DataPackageOperation.Move;
        }

        private void GaDTeacher_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
        }

        private void GaDTeacher_Drop(object sender, DragEventArgs e)
        {
            GridView grdView = sender as GridView;

            var deferal = e.GetDeferral();
            Point posFromRoot = e.GetPosition(grdView.ItemsPanelRoot);

            int index = 0;
            var itemContainer = grdView.ContainerFromIndex(0) as GridViewItem;
            if (itemContainer != null)
            {
                Size itemSize = new Size(
                    itemContainer.ActualWidth + itemContainer.Margin.Left + itemContainer.Margin.Right,
                    itemContainer.ActualHeight + itemContainer.Margin.Top + itemContainer.Margin.Bottom);

                int xCount = (int)Math.Max(1, grdView.ItemsPanelRoot.ActualWidth / itemSize.Width);
                int yCount = (int)Math.Max(1, grdView.ItemsPanelRoot.ActualHeight / itemSize.Height);

                int indexX = (int)Math.Min(xCount, posFromRoot.X / itemSize.Width);
                int indexY = (int)Math.Min(yCount, posFromRoot.Y / itemSize.Height);

                index = Math.Min(grdView.Items.Count, indexY * xCount + indexX);
            }

            if (grdView.Name == "grdViewExcludedTeacher")
            {
                foreach (var item in DragOperationTemp)
                {
                    CurrentBatch.ExcludeTeachers.Insert(index, item);
                    IncludedTeachers.Remove(item);
                }
            }
            else
            {
                foreach (var item in DragOperationTemp)
                {
                    CurrentBatch.ExcludeTeachers.Remove(item);
                    IncludedTeachers.Insert(index, item);
                }
            }

            FBStatusCount = IncludedTeachers.Count.ToString();
        }

        private void GaDTeacher_DragEnter(object sender, DragEventArgs e)
        {
            e.DragUIOverride.IsGlyphVisible = false;
        }
        #endregion

        #region Data fetching
        private async Task FetchTeachers()
        {
            IsLoading = true;
            CurrentBatch.Teachers = await Provider.GetFeedbackEntries();
            IsLoading = false;
            Bindings.Update();
        }

        private async Task UpdateIncludedTeacher()
        {
            IncludedTeachers.Clear();
            var filter = await GetFilteredTeacher();
            if (filter != null)
            {
                foreach (var item in filter)
                {
                    IncludedTeachers.Add(item);
                    await Task.Delay(10);
                }
            }

            FBStatusCount = IncludedTeachers?.Count.ToString();
        }

        private async Task<List<FeedbackTeacherModel>> GetFilteredTeacher()
        {
            return await Task.Run(() =>
                CurrentBatch?.Teachers?.Where(x =>
                (x.TeacherName == CurrentBatch.FeedbackData.TeacherName || CurrentBatch.FeedbackData.TeacherName == "All teachers") &&
                (x.ClassName == CurrentBatch.FeedbackData.ClassName || CurrentBatch.FeedbackData.ClassName == "All class names") &&
                (x.AcademicYear == CurrentBatch.FeedbackData.AcademicYear || CurrentBatch.FeedbackData.AcademicYear == "All academic years") &&
                (x.Term == CurrentBatch.FeedbackData.Term || CurrentBatch.FeedbackData.Term == "All terms") &&
                (x.FeedbackFor == CurrentBatch.FeedbackData.FeedbackFor || CurrentBatch.FeedbackData.FeedbackFor == "All feedback fors") &&
                (x.OpenDate == CurrentBatch.FeedbackData.OpenDate || CurrentBatch.FeedbackData.OpenDate == "All open dates") &&
                (CurrentBatch.FeedbackStatusFilterValue == "All feedback status" ? true :
                    CurrentBatch.FeedbackStatusFilterValue == "Edit feedback only" ? x.IsFeedback : !x.IsFeedback)).ToList());
        } 
        #endregion

        #region Feedback sending
        private async void btnSend_Click(SplitButton sender, SplitButtonClickEventArgs args)
        {
            IsEnabled = false;
            IsLoading = true;

            await Task.Run(async () =>
            {
                // Figure out who will be feedbacked
                if (CurrentBatch.Teachers.Count == 0)  // If user didn't load teachers we gonna 
                {
                    CurrentBatch.Teachers = await Provider.GetFeedbackEntries();
                }

                var filtered = await GetFilteredTeacher();
                var includedTeacher = filtered.Where(x => !CurrentBatch.ExcludeTeachers.Any(p => p.ID == x.ID)).Select(x => SendFeedback(x)).ToList();

                int count = includedTeacher.Count;
                int ftcount = 0;
                while (includedTeacher.Count() > 0)
                {
                    var finishedTask = await Task.WhenAny(includedTeacher);
                    ftcount++;
                    await Dispatcher.RunIdleAsync(x => FBStatusCount = $"{ftcount}/{count}");
                    includedTeacher.Remove(finishedTask);
                }
            });

            IsEnabled = true;
            IsLoading = false;
        }

        private async Task SendFeedback(FeedbackTeacherModel item)
        {
            await Provider.GetFeedbackData(item);
            item.FeedbackChoice = new List<int>(CurrentBatch.FeedbackData.FeedbackChoice);
            item.Comment = CurrentBatch.FeedbackData.Comment;

            await Provider.SendFeedbackAsync(item);
        } 
        #endregion

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (Drafts.Count == 0)
                Drafts.Add(new BatchedFeedbackModel() { FeedbackerAccountEmail = Provider.CurrentAccount.Email });

            CurrentBatch = Drafts.Last(x => x.FeedbackerAccountEmail == Provider.CurrentAccount.Email);
            LoadTeacherTask = FetchTeachers().ContinueWith(x => UpdateIncludedTeacher());
            await Task.Delay(1000);
            await LoadTeacherTask;
            FBStatusCount = IncludedTeachers.Count.ToString();
            DataContext = this;
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(pivotRoot.SelectedIndex == 2 || pivotRoot.SelectedIndex == 1)
            {
                // Get feedback entries
                await LoadTeacherTask;

                if(pivotRoot.SelectedIndex == 2)
                {
                    await UpdateIncludedTeacher();
                }
            }
        }

        private async void Event_FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            if(IsLoaded)
                await UpdateIncludedTeacher();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
