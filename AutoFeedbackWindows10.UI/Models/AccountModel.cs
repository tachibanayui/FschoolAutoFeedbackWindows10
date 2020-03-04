using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFeedbackWindows10.UI.Models
{
    public class AccountModel
    {
        // General and login info
        public string Email { get; set; }
        public string SessionID { get; set; }
        public Dictionary<string, string> Cookies { get; set; }

        // Personal Profile
        public string Name { get; set; }
        public DateTime DoB { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }

        // Parent
        public string ParentName { get; set; }
        public string ParentPhoneNo { get; set; }
        public string ParentAddress { get; set; }
        public string ParentEmail { get; set; }
        public string Job { get; set; }
        public string PlaceOfWork { get; set; }

        // Academic
        public string Campus { get; set; }
        public string RollNumber { get; set; }
        public string MemberCode { get; set; }
        public DateTime EnrolDate { get; set; }
        public string Status { get; set; }
    }   
}
