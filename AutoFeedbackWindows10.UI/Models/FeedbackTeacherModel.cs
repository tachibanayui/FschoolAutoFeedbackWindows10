using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFeedbackWindows10.UI.Models
{
    public class FeedbackTeacherModel : INotifyPropertyChanged
    {
        // Feedback info
        public string TeacherName { get; set; }
        public string FeedbackFor { get; set; }
        public string Term { get; set; }
        public string AcademicYear { get; set; }
        public string ClassName { get; set; }
        public string OpenDate { get; set; }
        public string ID { get; set; }

        // Feedback value
        public List<int> FeedbackChoice { get; set; }
        public string Comment { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

