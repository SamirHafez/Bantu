using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Bantu.Notification;
using Bantu.ViewModel;
using System.IO.IsolatedStorage;

namespace Bantu
{
    public partial class Settings : PhoneApplicationPage
    {
		public SettingsVM SettingsVM { get; set; }

        public Settings()
        {
            InitializeComponent();
			DataContext = this;

			SettingsVM = new SettingsVM();
        }

		public void Setup(object sender, EventArgs args) 
		{
			var settings = IsolatedStorageSettings.ApplicationSettings;

			if (settings.Contains("settings"))
				tsNotification.IsChecked = (settings["settings"] as SettingsVM).Notifications;
		}

        public void EnableNotifications(object sender, RoutedEventArgs routedEventArgs) 
        {
			SettingsVM.Notifications = tsNotification.IsChecked.Value;

			var settings = IsolatedStorageSettings.ApplicationSettings;
			settings["settings"] = SettingsVM;
        }

        public void DisableNotifications(object sender, RoutedEventArgs routedEventArgs)
        {
			Manager.DisableNotification();
			SettingsVM.Notifications = tsNotification.IsChecked.Value;

			var settings = IsolatedStorageSettings.ApplicationSettings;
			settings["settings"] = SettingsVM;
        }
    }
}