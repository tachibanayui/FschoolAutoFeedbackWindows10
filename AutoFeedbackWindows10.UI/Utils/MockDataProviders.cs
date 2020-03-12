using AutoFeedbackWindows10.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFeedbackWindows10.UI.Utils
{
    class MockDataProviders
    {
        private static Random Random = new Random();

        private static List<string> Names = new List<string> { "Josh", "Nguyễn", "Thành", "Tachibana", "Yui", "Makoto", "Peppy" };

        private static List<string> ClassName { get; set; } = new List<string>() { "Toán đại cương", "Toán nâng cao", "Ngữ Văn", "Lí", "Hóa", "Sinh", "Sử", "Địa" };
        private static List<string> Year { get; set; } = new List<string>() { "2019.2020", "2018.2019", "2018.2019" };
        private static List<string> Terms { get; set; } = new List<string>() { "Terms1", "Terms2", "Terms3" };


        public static List<FeedbackTeacherModel> GetBatchedMockTeachers()
        {
            List<FeedbackTeacherModel> res = new List<FeedbackTeacherModel>();

            for (int i = 0; i < 25; i++)
            {
                var t = new FeedbackTeacherModel();
                t.TeacherName = GetRandomNames();
                t.AcademicYear = Year[Random.Next(0, 3)];
                t.ClassName = ClassName[Random.Next(0, ClassName.Count)];
                t.FeedbackFor = "Giáo viên bộ môn";
                t.OpenDate = DateTime.Today.ToString();
                t.Term = Terms[Random.Next(0, Terms.Count)];

                res.Add(t);
            }

            return res;
        }

        public static string GetRandomNames()
        {
            StringBuilder bd = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                bd.Append(Names[Random.Next(0, Names.Count)] + " ");
            }

            return bd.ToString().Trim();
        }
    }
}
