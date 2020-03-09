using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace AutoFeedbackWindows10.UI.ValueConverters
{
    class FeedbackChoiceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is int val)
            {
                Console.WriteLine(val);
                return Math.Abs(4 - val);
            }

            return 4;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is int val)
            {
                return Math.Abs(4 - val);
            }

            return 4;
        }
    }
}
