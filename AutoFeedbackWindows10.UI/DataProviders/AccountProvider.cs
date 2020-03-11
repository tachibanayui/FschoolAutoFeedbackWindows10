using AutoFeedbackWindows10.UI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace AutoFeedbackWindows10.UI.DataProviders
{
    class AccountProvider
    {
        private static AccountStorageModel StorageAccounts;


        private static async Task LoadUsersFromStorage()
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Accounts.json", CreationCollisionOption.OpenIfExists);
            string content = await FileIO.ReadTextAsync(file);
            if (!string.IsNullOrEmpty(content))
            {
                StorageAccounts = JsonConvert.DeserializeObject<AccountStorageModel>(content);
            }

            if (StorageAccounts == null)
            {
                StorageAccounts = new AccountStorageModel();
            }
        }

        public static async Task<AccountModel> GetActiveAccount()
        {
            if (StorageAccounts == null)
            {
                await LoadUsersFromStorage();
            }

            return StorageAccounts?.ActiveAccount;
        }

        public static async Task<IReadOnlyCollection<AccountModel>> GetAccounts()
        {
            if (StorageAccounts == null)
            {
                await LoadUsersFromStorage();
            }

            return StorageAccounts.Accounts.AsReadOnly();
        }

        public static async Task<AccountModel> GetAccountByName(string name)
        {
            if (StorageAccounts == null)
            {
                await LoadUsersFromStorage();
            }

            return StorageAccounts.Accounts.FirstOrDefault(x => x.Name == name);
        }

        public static async Task<AccountModel> GetAccountByEmail(string email)
        {
            if (StorageAccounts == null)
            {
                await LoadUsersFromStorage();
            }

            return StorageAccounts.Accounts.FirstOrDefault(x => x.Email == email);
        }

        public static async Task<AccountModel> AddOrUpdateAccountAsync(string name, string email, string sessionID, List<CookieModel> cookies)
        {
            if (StorageAccounts == null)
            {
                await LoadUsersFromStorage();
            }

            // we gonna find the account to be update by email
            var sModel = StorageAccounts.Accounts.FirstOrDefault(x => x.Email == email);
            if(sModel != null)
            {
                sModel.SessionID = sessionID;
                sModel.Name = name;
                sModel.Cookies = cookies;
                return sModel;
            }
            else
            {
                var item = new AccountModel() { SessionID = sessionID, Email = email, Name = name, Cookies = cookies };
                StorageAccounts.Accounts.Add(item);
                return item;
            }

        }

        public static async Task SetActiveAccountAsync(AccountModel model)
        {
            if (StorageAccounts == null)
            {
                await LoadUsersFromStorage();
            }

            if (StorageAccounts.Accounts.Contains(model))
            {
                StorageAccounts.ActiveAccount = model;
                OnActiveAccountChanged(model);
            }
        }

        public static async Task RemoveAccountAsync(AccountModel model)
        {
            if (StorageAccounts == null)
            {
                await LoadUsersFromStorage();
            }

            StorageAccounts.Accounts.Remove(model);
        }

        public static async Task SaveAccountToStorage()
        {
            if(StorageAccounts != null)
            {
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Accounts.json", CreationCollisionOption.OpenIfExists);
                await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(StorageAccounts));
            }
        }

        public static event EventHandler<ValueEventArgs<AccountModel>> ActiveAccountChanged;
        private static void OnActiveAccountChanged(AccountModel model) => ActiveAccountChanged?.Invoke(null, new ValueEventArgs<AccountModel>(model));
    }
}