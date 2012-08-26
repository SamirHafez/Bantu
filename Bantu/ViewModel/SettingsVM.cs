using System.ComponentModel;

namespace Bantu.ViewModel
{
    public class SettingsVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private bool _notifications;
        public bool Notifications
        {
            get { return _notifications; }
            set
            {
                _notifications = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Notifications"));
            }
        }
    }
}
