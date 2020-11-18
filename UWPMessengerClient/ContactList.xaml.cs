﻿using System;
using System.Collections.Generic;
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

namespace UWPMessengerClient
{
    public sealed partial class ContactList : Page
    {
        NotificationServerConnection notificationServerConnection;

        public ContactList()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            notificationServerConnection = (NotificationServerConnection)e.Parameter;
            base.OnNavigatedTo(e);
        }

        private async void Presence_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedStatus = e.AddedItems[0].ToString();
            switch (selectedStatus)
            {
                case "Available":
                    await notificationServerConnection.ChangePresence(PresenceStatuses.Available);
                    break;
                case "Busy":
                    await notificationServerConnection.ChangePresence(PresenceStatuses.Busy);
                    break;
                case "Away":
                    await notificationServerConnection.ChangePresence(PresenceStatuses.Away);
                    break;
            }
        }
    }
}