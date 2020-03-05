using AutoFeedbackWindows10.UI.Models;
using AutoFeedbackWindows10.UI.Utils;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AutoFeedbackWindows10.UI.DataProviders
{
    class FeedbackEntryProvider
    {
        private AccountModel _acct;
        private List<FeedbackTeacherModel> _cachedFeedbackEntries;
        private DateTime _lastGetFeedbackEntries;
        public const string FschoolDomain = "http://fschool.fpt.edu.vn";

        public FeedbackEntryProvider(AccountModel account)
        {
            _acct = account;
        }

        public async Task<List<FeedbackTeacherModel>> GetFeedbackEntries()
        {
            try
            {
                // Check did we request feedback entry and cool down at least 10 mins
                if (_cachedFeedbackEntries == null || DateTime.Now - _lastGetFeedbackEntries > TimeSpan.FromMinutes(10))
                {
                    _cachedFeedbackEntries = await GetFeedbackEntriesOrigin();
                    _lastGetFeedbackEntries = DateTime.Now;
                }

                return _cachedFeedbackEntries;
            }
            catch (InvalidSessionIDException) //we don't wrap this exception
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Failed to request feedback entries", e);
            }
        }

        public async Task<FeedbackTeacherModel> GetFeedbackData(FeedbackTeacherModel model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.ID))
                {
                    string val = await GetFeedbackDataOrigin(model);
                    // Validate session id
                    if(await AccountModel.IsLoginPage(val))
                        throw new InvalidSessionIDException($"{_acct.Email} session id: {_acct.SessionID} is invalid or expired, please request a new one and try again");

                    HtmlDocument doc = new HtmlDocument();

                    await Task.Run(() =>
                    {
                        doc.LoadHtml(val);
                        ParseFeedbackChoices(model, doc);

                        // parse feedback comment
                        var commentTextBox = doc.DocumentNode.Descendants("textarea").FirstOrDefault();
                        if (commentTextBox != null)
                        {
                            model.Comment = LatinEncodingHelper.Decode(commentTextBox.InnerText.Trim());
                        }

                        // Get ViewState and related values
                        model.__ViewState = doc.DocumentNode.SelectSingleNode("/html/body/div/div[1]/section[2]/form/input[1]").Attributes["value"].Value.Trim();
                        model.__VIEWSTATEGENERATOR = doc.DocumentNode.SelectSingleNode("/html/body/div/div[1]/section[2]/form/input[2]").Attributes["value"].Value.Trim();
                        model.__EVENTVALIDATION = doc.DocumentNode.SelectSingleNode("/html/body/div/div[1]/section[2]/form/input[3]").Attributes["value"].Value.Trim();
                    });
                

                    return model;
                }
                else
                {
                    throw new InvalidOperationException("Invalid ID");
                }
            }
            catch (InvalidSessionIDException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Failed to get feedback data", e);
            }
        }

        public async Task<bool> SendFeedbackAsync(FeedbackTeacherModel model)
        {
            try
            {
                Dictionary<string, string> formData = new Dictionary<string, string>()
                {
                    ["__VIEWSTATE"] = model.__ViewState,
                    ["__VIEWSTATEGENERATOR"] = model.__VIEWSTATEGENERATOR,
                    ["__EVENTVALIDATION"] = model.__EVENTVALIDATION,
                    ["ctl00$ContentPlaceHolder1$reload$ctl00$chkList"] = model.FeedbackChoice[0].ToString(),
                    ["ctl00$ContentPlaceHolder1$reload$ctl01$chkList"] = model.FeedbackChoice[1].ToString(),
                    ["ctl00$ContentPlaceHolder1$reload$ctl02$chkList"] = model.FeedbackChoice[2].ToString(),
                    ["ctl00$ContentPlaceHolder1$reload$ctl03$chkList"] = model.FeedbackChoice[3].ToString(),
                    ["ctl00$ContentPlaceHolder1$reload$ctl04$chkList"] = model.FeedbackChoice[4].ToString(),
                    ["ctl00$ContentPlaceHolder1$reload$ctl05$chkList"] = model.FeedbackChoice[5].ToString(),
                    ["ctl00$ContentPlaceHolder1$txtComment"] = model.Comment,
                    ["ctl00$ContentPlaceHolder1$btSendFeedback"] = "Gửi ý kiến"
                };

                HttpWebRequest req = WebRequest.CreateHttp($"{FschoolDomain}{model.ID}");
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                _acct.AddSessionIdCookie(req);

                using (var reqStream = await req.GetRequestStreamAsync())
                {
                    using (var urlEncoded = new FormUrlEncodedContent(formData))
                    {
                        await urlEncoded.CopyToAsync(reqStream);
                        var test = await urlEncoded.ReadAsStringAsync();
                    }

                    using (var resp = await req.GetResponseAsync())
                    {
                        using (var stream = resp.GetResponseStream())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                string res = await reader.ReadToEndAsync();


                                if (await AccountModel.IsLoginPage(res))
                                    throw new InvalidSessionIDException($"{_acct.Email} session id: {_acct.SessionID} is invalid or expired, please request a new one and try again");

                                // Validate feedback 
                                HtmlDocument doc = new HtmlDocument();
                                doc.LoadHtml(res);
                                return doc.DocumentNode.SelectSingleNode("/html/body/div/div[1]/section[2]/form/div/div/div/div[3]/div/div[1]/span/font") != null;
                            }
                        }
                    }
                }
            }
            catch (InvalidSessionIDException) // We don't wrap this exception
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Failed to send feedback", e);
            }
        }

        private static void ParseFeedbackChoices(FeedbackTeacherModel model, HtmlDocument doc)
        {
            var multipleChoices = doc.DocumentNode.Descendants("table").Where(x => x.Id.Contains("ContentPlaceHolder1_reload")).FirstOrDefault();
            if (multipleChoices != null)
            {
                if (model.FeedbackChoice == null)
                {
                    model.FeedbackChoice = new List<int>(6);
                }

                for (int choiceQ = 0; choiceQ < 6; choiceQ++)
                {
                    for (int choiceA = 0; choiceA < 4; choiceA++)
                    {
                        // get the options
                        var opt = multipleChoices.Descendants("input").FirstOrDefault(x => x.Id == $"ContentPlaceHolder1_reload_chkList_{choiceQ}_{choiceA}_{choiceQ}");
                        if (opt != null && opt.Attributes.Contains("checked"))
                        {
                            model.FeedbackChoice.Add(4 - choiceA);
                            break;
                        }
                    }
                    if (model.FeedbackChoice.Count < choiceQ)
                        model.FeedbackChoice.Add(4);
                }
            }
            else
            {
                throw new InvalidDataException("multiple choice feedback malformed");
            }
        }

        private async Task<string> GetFeedbackDataOrigin(FeedbackTeacherModel model)
        {
            HttpWebRequest req = WebRequest.CreateHttp($"{FschoolDomain}{model.ID}");
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

        private async Task<List<FeedbackTeacherModel>> GetFeedbackEntriesOrigin()
        {
            var httpResp = await RequestFeedbackEntries();
            // Validate session id
            if(await AccountModel.IsLoginPage(httpResp))
                throw new InvalidSessionIDException($"{_acct.Email} session id: {_acct.SessionID} is invalid or expired, please request a new one and try again");

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(httpResp);
            var entries = doc.DocumentNode.Descendants("table").FirstOrDefault()?.Descendants("tbody").FirstOrDefault()?.Descendants("tr");

            var res = new List<FeedbackTeacherModel>();
            foreach (var item in entries)
            {
                FeedbackTeacherModel feedbackEntry = new FeedbackTeacherModel();
                feedbackEntry.ClassName = item.ChildNodes[3].InnerText.Replace("\"", "").Trim();
                feedbackEntry.AcademicYear = item.ChildNodes[5].InnerText.Replace("\"", "").Trim();
                feedbackEntry.Term = item.ChildNodes[7].InnerText.Replace("\"", "").Trim();
                feedbackEntry.TeacherName = item.ChildNodes[9].InnerText.Replace("\"", "").Trim();
                feedbackEntry.FeedbackFor = item.ChildNodes[11].InnerText.Replace("\"", "").Trim();
                feedbackEntry.OpenDate = item.ChildNodes[13].InnerText.Replace("\"", "").Trim();
                feedbackEntry.ID = item.ChildNodes[17].ChildNodes[1].Attributes["href"].Value.Trim();

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
