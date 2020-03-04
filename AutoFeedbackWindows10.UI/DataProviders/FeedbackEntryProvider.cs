using AutoFeedbackWindows10.UI.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AutoFeedbackWindows10.UI.DataProviders
{
    class FeedbackEntryProvider
    {
        private AccountModel _acct;
        private List<FeedbackTeacherModel> _cachedFeedbackEntries;
        public const string FschoolDomain = "http://fschool.fpt.edu.vn";

        public FeedbackEntryProvider(AccountModel account)
        {
            _acct = account;
        }

        public async Task<List<FeedbackTeacherModel>> GetFeedbackEntries()
        {
            // Check did we request feedback entry
            if (_cachedFeedbackEntries == null)
            {
                _cachedFeedbackEntries = await GetFeedbackEntriesOrigin();
            }

            return _cachedFeedbackEntries;
        }

        private async Task<List<FeedbackTeacherModel>> GetFeedbackEntriesOrigin()
        {
            var httpResp = await RequestFeedbackEntries();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(httpResp);
            var entries = doc.DocumentNode.Descendants("table").FirstOrDefault()?.Descendants("tbody").FirstOrDefault()?.Descendants("tr");

            var res = new List<FeedbackTeacherModel>();
            foreach (var item in entries)
            {
                FeedbackTeacherModel feedbackEntry = new FeedbackTeacherModel();
                feedbackEntry.ClassName = item.ChildNodes[1].InnerText.Replace("\"", "").Trim();
                feedbackEntry.AcademicYear = item.ChildNodes[2].InnerText.Replace("\"", "").Trim();
                feedbackEntry.Term = item.ChildNodes[3].InnerText.Replace("\"", "").Trim();
                feedbackEntry.TeacherName = item.ChildNodes[4].InnerText.Replace("\"", "").Trim();
                feedbackEntry.FeedbackFor = item.ChildNodes[5].InnerText.Replace("\"", "").Trim();
                feedbackEntry.OpenDate = item.ChildNodes[6].InnerText.Replace("\"", "").Trim();
                feedbackEntry.ID = item.ChildNodes[8].FirstChild.Attributes["href"].Value;

                res.Add(feedbackEntry);
            }

            return res;
        }

        private async Task<string> RequestFeedbackEntries()
        {
            HttpWebRequest req = WebRequest.CreateHttp($"{FschoolDomain}/Feedback/StudentFeedBack.aspx");
            _acct.AddSessionIdCookie(req);
            using (var resp = await req.GetResponseAsync())
            {
                using (var stream = resp.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }
        }
    }
}
