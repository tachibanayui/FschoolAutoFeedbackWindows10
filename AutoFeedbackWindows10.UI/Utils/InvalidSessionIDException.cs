using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFeedbackWindows10.UI.Utils
{
    [Serializable]
    public class InvalidSessionIDException : Exception
    {
        public InvalidSessionIDException() { }
        public InvalidSessionIDException(string message) : base(message) { }
        public InvalidSessionIDException(string message, Exception inner) : base(message, inner) { }
        protected InvalidSessionIDException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
