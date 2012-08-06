using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using Bantu.TableStorage;

namespace Bantu.ViewModel
{
	public class PlayerVM : INotifyPropertyChanged
	{
		public string Name { get; set; }
		public string Identifier { get; set; }

		private long _score;
		public long Score
		{
			get { return _score; }
			set
			{
				_score = value;
				if (PropertyChanged != null)
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

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
