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

namespace Bantu.ViewModel
{
	public class CupVM : INotifyPropertyChanged, INotifyPropertyChanging
	{
		public int Index { get; set; }

		private int _stones;
		public int Stones
		{
			get { return _stones; }
			set
			{
				if (PropertyChanging != null)
					PropertyChanging(this, new PropertyChangingEventArgs("Stones"));
				_stones = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("Stones"));
			}
		}

		public bool IsScore { get; set; }

		public PlayerVM Owner { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public event PropertyChangingEventHandler PropertyChanging;
	}
}
