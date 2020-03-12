using Newtonsoft.Json;
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
        public string TeacherName { get; set; } = "All teachers";
        public string FeedbackFor { get; set; } = "All feedback fors";
        public string Term { get; set; } = "All terms";
        public string AcademicYear { get; set; } = "All academic years";
        public string ClassName { get; set; } = "All class names";
        public string OpenDate { get; set; } = "All open dates";

        public string ID { get; set; } = string.Empty;
        public bool IsFeedback => ID.Contains("EditDoFeedback");

        // Feedback value number reflect the option and the value send for post request
        /*  ctl00$ContentPlaceHolder1$reload$ctl00$chkList: 4
            ctl00$ContentPlaceHolder1$reload$ctl01$chkList: 4
            ctl00$ContentPlaceHolder1$reload$ctl02$chkList: 4
            ctl00$ContentPlaceHolder1$reload$ctl03$chkList: 4
            ctl00$ContentPlaceHolder1$reload$ctl04$chkList: 4
            ctl00$ContentPlaceHolder1$reload$ctl05$chkList: 4
            ctl00$ContentPlaceHolder1$txtComment: Giáo viên rất nhiệt tình
            ctl00$ContentPlaceHolder1$btSendFeedback: Gửi ý kiến
         */
        
        public List<int> FeedbackChoice { get; set; } = new List<int>() { 4, 4, 4, 4, 4, 4 };
        public string Comment { get; set; } = "Giáo viên rất nhiệt tình";


        // View states
        [PropertyChanged.DoNotNotify]
        [JsonIgnore]
        public string __ViewState { get; set; }
        [PropertyChanged.DoNotNotify]
        [JsonIgnore]
        public string __VIEWSTATEGENERATOR { get; set; }
        [PropertyChanged.DoNotNotify]
        [JsonIgnore]
        public string __EVENTVALIDATION { get; set; }  

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

