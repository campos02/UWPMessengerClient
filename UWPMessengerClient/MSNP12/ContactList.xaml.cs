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
using System.Threading.Tasks;

namespace UWPMessengerClient.MSNP12
{
    public sealed partial class ContactList : Page
    {
        private NotificationServerConnection notificationServerConnection;

        public ContactList()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            notificationServerConnection = (NotificationServerConnection)e.Parameter;
            string status = notificationServerConnection.CurrentUserPresenceStatus;
            string fullStatus = null;
            switch (status)
            {
                case "NLN":
                    fullStatus = "Available";
                    break;
                case "BSY":
                    fullStatus = "Busy";
                    break;
                case "AWY":
                    fullStatus = "Away";
                    break;
            }
            Presence.SelectedItem = fullStatus;
            base.OnNavigatedTo(e);
        }

        private async void Presence_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (notificationServerConnection != null)
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

        private async Task StartChat()
        {
            if (notificationServerConnection.ContactIndexToChat != contactListView.SelectedIndex || notificationServerConnection.SBConnection == null)
            {
                notificationServerConnection.ContactIndexToChat = contactListView.SelectedIndex;
                await notificationServerConnection.InitiateSB();
            }
            this.Frame.Navigate(typeof(ChatPage), notificationServerConnection);
        }

        private async void start_chat_button_Click(object sender, RoutedEventArgs e)
        {
            await StartChat();
        }

        private async void ChangeUserDisplayNameConfirmationButton_Click(object sender, RoutedEventArgs e)
        {
            await notificationServerConnection.ChangeUserDisplayName(ChangeUserDisplayNameTextBox.Text);
            ChangeUserDisplayNameTextBox.Text = "";
            ChangeFlyout.Hide();
        }

        private void TextBlock_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private async void StackPanel_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            await StartChat();
        }

        private async void addContactButton_Click(object sender, RoutedEventArgs e)
        {
            await notificationServerConnection.AddContact(contactEmailBox.Text, contactDisplayNameBox.Text);
            contactDisplayNameBox.Text = "";
            contactEmailBox.Text = "";
            addContactAppBarButton.Flyout.Hide();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            notificationServerConnection.Exit();
            notificationServerConnection = null;
            this.Frame.Navigate(typeof(LoginPage));
        }

        private void addContactAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            addContactAppBarButton.Flyout.ShowAt((FrameworkElement)sender);
        }

        private async void removeContactAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (contactListView.SelectedIndex >= 0)
            {
                await notificationServerConnection.RemoveContact(notificationServerConnection.contacts_in_forward_list[contactListView.SelectedIndex]);
            }
        }
    }
}