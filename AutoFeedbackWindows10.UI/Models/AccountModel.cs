using AutoFeedbackWindows10.UI.Utils;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AutoFeedbackWindows10.UI.Models
{
    public class AccountModel
    {
        public const string GoogleLoginLink = @"https://accounts.google.com/o/oauth2/auth?scope=profile%20email&state=%2Fprofile&redirect_uri=http://fschool.fpt.edu.vn/LoginPage/Login.aspx&response_type=code&client_id=699800698114-ahs58mmqlmscei6tvhdcabidotspimif.apps.googleusercontent.com&approval_prompt=auto&access_type=offline";
        public const string FschoolDomain = "http://fschool.fpt.edu.vn";
        private const string GoogleAccountDomain = "https://accounts.google.com";
        public const string SessionIDCookieName = "ASP.NET_SessionId";

        // General and login info
        public string Email { get; set; }
        public string SessionID { get; set; }
        public List<CookieModel> Cookies { get; set; }

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

        //Internally
        public DateTime LastUpdated { get; set; }

        public static async Task<bool> IsLoginPage(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            // We gonna search for "Please fill out the following fields to login" node and vertify
            var textNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"p\"]/div[1]/div[2]/p");
            return textNode != null && textNode.InnerText.Contains("Please fill out the following fields to login");
        }

        public static async Task<string> GetAccountName(string sessionID)
        {
            // we will create or own request to avoid loading scripts and image that might cause performance overhead
            try
            {
                HttpWebRequest req = WebRequest.CreateHttp($"{FschoolDomain}/DefaultPage/StudentDefaultPage.aspx");
                req.CookieContainer = new CookieContainer();
                req.CookieContainer.Add(new Cookie(SessionIDCookieName, sessionID) { Domain = req.Host });
                using (var resp = await req.GetResponseAsync())
                {
                    using (var stream = resp.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string data = reader.ReadToEnd();
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(data);
                            return doc.DocumentNode.SelectSingleNode("/html/body/div/header/nav/div/ul/li[2]/ul/li[1]/p")
                                .InnerText.Replace("\"", "").Trim()
                                .Split("\r")[0];
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public void AddSessionIdCookie(HttpWebRequest req, bool initCookieContainer = true)
        {
            if (initCookieContainer)
                req.CookieContainer = new CookieContainer();

            req.CookieContainer.Add(new Cookie("ASP.NET_SessionId", SessionID) { Domain = req.Host });
        }

        public async Task UpdateStudentInfo()
        {
            try
            {
                HttpWebRequest req = WebRequest.CreateHttp(@"http://fschool.fpt.edu.vn/User/StudentDetails.aspx");
                AddSessionIdCookie(req);
                using (var resp = await req.GetResponseAsync())
                {
                    using (var stream = resp.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string respString = await reader.ReadToEndAsync();
                            // Throw exception if we get login page, indicate that our session id is invalid
                            if (await IsLoginPage(respString))
                                throw new InvalidSessionIDException($"{Email} session id: {SessionID} is invalid or expired, please request a new one and try again");

                            AnalyzeHtml(respString);
                        }
                    }
                }
            }
            catch (InvalidSessionIDException) // we don't wrap this exception
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Failed to analyze data from fschool.fpt.edu.vn", e);
            }
        }


        private void AnalyzeHtml(string respString)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(respString);
            Name =  doc.GetElementbyId("ContentPlaceHolder1_lblFullname").InnerText.Trim();
            DoB = DateTime.ParseExact(doc.GetElementbyId("ContentPlaceHolder1_lblDateOfBirth").InnerText.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            Gender = doc.GetElementbyId("ContentPlaceHolder1_lblGender").InnerText.Trim();
            Address = doc.GetElementbyId("ContentPlaceHolder1_lblAddress").InnerText.Trim();
            PhoneNo = doc.GetElementbyId("ContentPlaceHolder1_lblPhoneNumber").InnerText.Trim();
            Email = doc.GetElementbyId("ContentPlaceHolder1_lblEmail").FirstChild.InnerText.Trim();
            ParentName = doc.GetElementbyId("ContentPlaceHolder1_lblParentName").InnerText.Trim();
            ParentPhoneNo = doc.GetElementbyId("ContentPlaceHolder1_lblParentPhone").InnerText.Trim();
            ParentAddress = doc.GetElementbyId("ContentPlaceHolder1_lblParentEmail").FirstChild.InnerText.Trim();
            Job = doc.GetElementbyId("ContentPlaceHolder1_lblParentJob").InnerText.Trim();
            PlaceOfWork = doc.GetElementbyId("ContentPlaceHolder1_lblPlaceOfWork").InnerText.Trim();
            Campus = doc.GetElementbyId("ContentPlaceHolder1_lblCampus").InnerText.Trim();
            RollNumber = doc.GetElementbyId("ContentPlaceHolder1_lblRollNumber").InnerText.Trim();
            MemberCode = doc.GetElementbyId("ContentPlaceHolder1_lblMemberCode").InnerText.Trim();
            EnrolDate = DateTime.ParseExact(doc.GetElementbyId("ContentPlaceHolder1_lblEnrolDate").InnerText.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            Status = doc.GetElementbyId("ContentPlaceHolder1_lblStatus").InnerText.Trim();

            LastUpdated = DateTime.Now;
        }
    }
}
