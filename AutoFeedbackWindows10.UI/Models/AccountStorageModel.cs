using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFeedbackWindows10.UI.Models
{
    class AccountStorageModel
    {
        public List<AccountModel> Accounts { get; set; } = new List<AccountModel>();
        public AccountModel ActuveAccount { get; set; }
    }
}
