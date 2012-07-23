using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Bantu.ViewModel;
using System.Threading;
using System.ComponentModel;

namespace Bantu.Controls
{
	public partial class CupControl : UserControl
	{
		public static int AnimationDelayIndex;
		private bool _resetOnce;
		public CupControl()
		{
			InitializeComponent();
			AnimationDelayIndex = 0;
			_resetOnce = true;
			BlinkWhite.Completed += (s, a) => tbStones.Text = ((CupVM)DataContext).Stones.ToString();
		}

		public void Preset(object sender, PropertyChangingEventArgs args)
		{
			if (_resetOnce)
			{
				var stones = ((CupVM)DataContext).Stones;
				tbStones.ClearValue(TextBlock.TextProperty);
				tbStones.Text = stones.ToString();
				_resetOnce = false;
			}
		}

		public void Animate(object sender, PropertyChangedEventArgs args)
		{
			BlinkWhite.Children[0].BeginTime = new TimeSpan(0, 0, 0, 0, AnimationDelayIndex++ * 400);
			BlinkWhite.Begin();
		}
	}
}
