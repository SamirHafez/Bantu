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
        public bool Animated { get; set; }
        public CupControl()
        {
            InitializeComponent();
        }

        public void Animate(object sender, EventArgs e) 
        {
            Dispatcher.BeginInvoke(delegate 
            {
                BlinkRed.Completed += (o, args) => Animated = true;
                Animated = false;
                BlinkRed.Begin();
            });
        }
    }
}
