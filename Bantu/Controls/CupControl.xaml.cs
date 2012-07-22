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

namespace Bantu.Controls
{
    public partial class CupControl : UserControl
    {
        private static int AnimationDelayIndex;
        public CupControl()
        {
            InitializeComponent();
            AnimationDelayIndex = 0;
        }

        public void Animate(object sender, EventArgs e)
        {
            BlinkRed.Children[0].BeginTime = new TimeSpan(0, 0, 0, 0, AnimationDelayIndex++ * 100);
            BlinkRed.Begin();
        }
    }
}
