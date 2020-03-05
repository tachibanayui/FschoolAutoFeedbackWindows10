using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFeedbackWindows10.UI.Utils
{
    public static class LatinEncodingHelper
    {
        public static char GetChar(string codePoint)
        {
            byte charByte = byte.Parse(codePoint.Substring(2, 3));
            return Encoding.GetEncoding("ISO-8859-1").GetString(new byte[] { charByte })[0];
        }

        public static string GetCodePoint(char latinChar) => $"&#{(byte)'á'};";

        public static string Decode(string originalValue)
        {
            StringBuilder res = new StringBuilder();
            StringBuilder lastCodePoint = new StringBuilder();


            for (int i = 0; i < originalValue.Length; i++)
            {
                if (originalValue[i] == '&')
                {
                    do
                    {
                        lastCodePoint.Append(originalValue[i]);
                        i++;
                    } while (originalValue[i] != ';');
                    res.Append(GetChar(lastCodePoint.ToString()));
                    lastCodePoint.Clear();
                }
                else
                {
                    res.Append(originalValue[i]);
                }
            }

            return res.ToString();
        }
    }
}
