using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.WindowsAzure.Samples.Phone.Identity.AccessControl;
using Bantu.TableStorage;
using System.IO.IsolatedStorage;
using Bantu.ViewModel;
using Microsoft.Phone.Shell;

namespace Bantu.Pages
{
	public partial class LoginPage : PhoneApplicationPage
	{
		public LoginPage()
		{
			this.InitializeComponent();
		}

		public new void Loaded(object sender, EventArgs args)
		{
			while (NavigationService.BackStack.Any())
				NavigationService.RemoveBackEntry();

			var swtStore = Application.Current.Resources["swtStore"] as SimpleWebTokenStore;

			if (swtStore.SimpleWebToken == null)
			{
				this.SignInControl.RequestSimpleWebTokenResponseCompleted += (s, e) =>
					{
						swtStore = Application.Current.Resources["swtStore"] as SimpleWebTokenStore;
						RetrieveOrCreatePlayer(swtStore);
					};
				this.SignInControl.GetSimpleWebToken();
			}
			else
				RetrieveOrCreatePlayer(swtStore);
		}

        public void AlternativeName(object sender, EventArgs args) 
        {
            var swtStore = Application.Current.Resources["swtStore"] as SimpleWebTokenStore;

            var name = tbUsername.Text;
            var nameIdentifier = swtStore.SimpleWebToken.NameIdentifier;

            RetrieveOrCreatePlayer(name, nameIdentifier);
        }

        private void RetrieveOrCreatePlayer(SimpleWebTokenStore swtStore) 
        {
            var name = swtStore.SimpleWebToken.Claims[ClaimTypes.Name];
            var nameIdentifier = swtStore.SimpleWebToken.NameIdentifier;

            RetrieveOrCreatePlayer(name, nameIdentifier);
        }

		private void RetrieveOrCreatePlayer(string name, string nameIdentifier)
		{
			SystemTray.ProgressIndicator.IsVisible = true;

			Context.GetPlayerByIdentifier(nameIdentifier, player =>
				Dispatcher.BeginInvoke(delegate
				{
					var settings = IsolatedStorageSettings.ApplicationSettings;
					settings["player"] = new PlayerVM(player);
					SystemTray.ProgressIndicator.IsVisible = false;

					NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
				})
			, () =>
				Context.CreatePlayer(name, nameIdentifier, player =>
					Dispatcher.BeginInvoke(() =>
					{
						var settings = IsolatedStorageSettings.ApplicationSettings;
						settings["player"] = new PlayerVM(player);
						SystemTray.ProgressIndicator.IsVisible = false;

						MainPage.NewPlayer = true;
						NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
					})
				, () =>
                    Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("The declared username is already registered with Bantu (or it contains illegal characters). Consider choosing a new name.");
                        var swtStore = Application.Current.Resources["swtStore"] as SimpleWebTokenStore;

                        tbUsername.Text = swtStore.SimpleWebToken.Claims[ClaimTypes.Name];

						SignInControl.Visibility = System.Windows.Visibility.Collapsed;

                        spAlternate.Visibility = Visibility.Visible;

						tbUsername.Focus();
						tbUsername.SelectAll();
                    })
				)
			);
		}
	}
}