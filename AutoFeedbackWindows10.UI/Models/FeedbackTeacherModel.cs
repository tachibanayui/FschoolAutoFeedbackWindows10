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
        public List<int> FeedbackChoice { get; set; }
        public string Comment { get; set; }


        // View states
        [PropertyChanged.DoNotNotify]
        public string __ViewState { get; set; }
        [PropertyChanged.DoNotNotify]
        public string __VIEWSTATEGENERATOR { get; set; }
        [PropertyChanged.DoNotNotify]
        public string __EVENTVALIDATION { get; set; }  

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

