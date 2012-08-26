using System.ComponentModel;

namespace Bantu.ViewModel
{
    public class CupVM : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public event PropertyChangingEventHandler PropertyChanging = delegate { };

        public int Index { get; set; }

        private int _stones;
        public int Stones
        {
            get { return _stones; }
            set
            {
                PropertyChanging(this, new PropertyChangingEventArgs("Stones"));
                _stones = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Stones"));
            }
        }

        public bool IsScore { get; set; }

        public PlayerVM Owner { get; set; }
    }
}
