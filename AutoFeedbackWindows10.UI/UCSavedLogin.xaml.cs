using AutoFeedbackWindows10.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AutoFeedbackWindows10.UI
{
    public sealed partial class UCSavedLogin : UserControl
    {
        public ObservableCollection<AccountModel> SavedAccounts { get; set; } = new ObservableCollection<AccountModel>();

        public void SetAccountPool(IEnumerable<AccountModel> pool)
        {
            SavedAccounts.Clear();
            foreach (var item in pool)
            {
                SavedAccounts.Add(item);
            }
        }

        public UCSavedLogin()
        {
            this.InitializeComponent();
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            OnAccountSelected((AccountModel)e.ClickedItem);
        }

        private void Context_DeleteAccount(object sender, RoutedEventArgs e)
        {
            AccountModel model = (sender as MenuFlyoutItem).DataContext as AccountModel;
            OnAccountDelete(model);
            SavedAccounts.Remove(model);
        }

        public event EventHandler<ValueEventArgs<AccountModel>> AccountSelected;
        public event EventHandler<ValueEventArgs<AccountModel>> AccountDelete;
        private void OnAccountSelected(AccountModel model) => AccountSelected?.Invoke(this, new ValueEventArgs<AccountModel>(model));
        private void OnAccountDelete(AccountModel model) => AccountDelete?.Invoke(this, new ValueEventArgs<AccountModel>(model));


    }
}
