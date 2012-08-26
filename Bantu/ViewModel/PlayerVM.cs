using System.Collections.Generic;
using System.ComponentModel;
using Bantu.TableStorage;

namespace Bantu.ViewModel
{
    public class PlayerVM : INotifyPropertyChanged, IEqualityComparer<PlayerVM>
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public string Name { get; set; }
        public string Identifier { get; set; }

        private long _score;
        public long Score
        {
            get { return _score; }
            set
            {
                _score = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Score"));
            }
        }

        public PlayerVM()
        { }

        public PlayerVM(Player player)
        {
            Name = player.RowKey;
            Identifier = player.Identifier;
            Score = player.Score;
        }

        public bool Equals(PlayerVM x, PlayerVM y)
        {
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(PlayerVM obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
