﻿using HandSchool.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageBoxPage : ContentPage
    {
        public MessageBoxPage()
        {
            InitializeComponent();
            BindingContext = MessageViewModel.Instance;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            var a = e.Item as IMessageItem;
            Task.Run(async () => { await App.Current.Message.SetReadState(a.Id, true); a.Unread = false; });
            await (new MessageDetailPage(e.Item as IMessageItem)).ShowAsync(Navigation);

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
