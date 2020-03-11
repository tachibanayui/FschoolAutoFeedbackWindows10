using AutoFeedbackWindows10.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace AutoFeedbackWindows10.UI.Utils
{
    class LoginHelper
    {
        static HttpBaseProtocolFilter httpBase = new HttpBaseProtocolFilter();

        public const string GoogleLoginLink = @"https://accounts.google.com/o/oauth2/auth?scope=profile%20email&state=%2Fprofile&redirect_uri=http://fschool.fpt.edu.vn/LoginPage/Login.aspx&response_type=code&client_id=699800698114-ahs58mmqlmscei6tvhdcabidotspimif.apps.googleusercontent.com&approval_prompt=auto&access_type=offline";
        public const string FschoolDomain = "http://fschool.fpt.edu.vn";
        private const string GoogleAccountDomain = "https://accounts.google.com";
        public const string SessionIDCookieName = "ASP.NET_SessionId";

        public static List<CookieModel> GetAndDeleteLoginCookie()
        {
            var cookies = new List<CookieModel>();
            foreach (var item in httpBase.CookieManager.GetCookies(new Uri(GoogleAccountDomain)))
            {
                var cookie = new CookieModel()
                {
                    Domain = item.Domain,
                    Path = item.Path,
                    Name = item.Name,
                    Value = item.Value
                };
                cookies.Add(cookie);
                httpBase.CookieManager.DeleteCookie(item);
            }

            return cookies;
        }

        public static void SetLoginCookies(List<CookieModel> cookies)
        {
            foreach (var item in cookies)
            {
                var cookie = new HttpCookie(item.Name, item.Domain, item.Path)
                {
                    Value = item.Value,
                    //Expires = new DateTimeOffset(DateTime.Now + TimeSpan.FromDays(1))
                };

                try { httpBase.CookieManager.SetCookie(cookie); } catch { }
            }
        }

        public static string GetSessionID()
        {
            return httpBase.CookieManager.GetCookies(new Uri(FschoolDomain)).FirstOrDefault(x => x.Name == SessionIDCookieName)?.Value;
        }
    }
}
