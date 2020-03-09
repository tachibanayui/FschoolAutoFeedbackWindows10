using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFeedbackWindows10.UI.Utils
{
    class LoginHelper
    {
        public const string GoogleLoginLink = @"https://accounts.google.com/o/oauth2/auth?scope=profile%20email&state=%2Fprofile&redirect_uri=http://fschool.fpt.edu.vn/LoginPage/Login.aspx&response_type=code&client_id=699800698114-ahs58mmqlmscei6tvhdcabidotspimif.apps.googleusercontent.com&approval_prompt=auto&access_type=offline";
        public const string FschoolDomain = "http://fschool.fpt.edu.vn";
        private const string GoogleAccountDomain = "https://accounts.google.com";
        public const string SessionIDCookieName = "ASP.NET_SessionId";


    }
}
