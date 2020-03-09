using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFeedbackWindows10.UI.Models
{
    public class BatchedFeedbackModel : INotifyPropertyChanged
    {
        // Contains feedback data and filters
        public string FeedbackerAccountEmail { get; set; }
        public FeedbackTeacherModel FeedbackData { get; set; } = new FeedbackTeacherModel();
        public string FeedbackStatusFilterValue { get; set; } = "Do feedback only";

        public List<FeedbackTeacherModel> Teachers { get; set; }

        public ObservableCollection<FeedbackTeacherModel> ExcludeTeachers { get; set; } = new ObservableCollection<FeedbackTeacherModel>();


        [DependsOn(nameof(Teachers))]
        public IEnumerable<string> TeacherNameFilter => Teachers?.GroupBy(x => x.TeacherName).Select(x => x.Key).Prepend("All teachers");

        [DependsOn(nameof(Teachers))]
        public IEnumerable<string> ClassNameFilter => Teachers?.GroupBy(x => x.ClassName).Select(x => x.Key).Prepend("All class names");

        [DependsOn(nameof(Teachers))]
        public IEnumerable<string> AcademicYearFilter => Teachers?.GroupBy(x => x.AcademicYear).Select(x => x.Key).Prepend("All academic years");

        [DependsOn(nameof(Teachers))]
        public IEnumerable<string> TermFilter => Teachers?.GroupBy(x => x.Term).Select(x => x.Key).Prepend("All terms");

        [DependsOn(nameof(Teachers))]
        public IEnumerable<string> FeedbackForFilter => Teachers?.GroupBy(x => x.FeedbackFor).Select(x => x.Key).Prepend("All feedback fors");

        [DependsOn(nameof(Teachers))]
        public IEnumerable<string> OpenDateFilter => Teachers?.GroupBy(x => x.OpenDate).Select(x => x.Key).Prepend("All open dates");

        [DoNotNotify]
        public List<string> FeedbackStatusFilter => new List<string>() { "All feedback status", "Edit feedback only", "Do feedback only" };

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
