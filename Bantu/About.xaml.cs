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
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace Bantu
{
	public partial class About : PhoneApplicationPage
	{
		public About()
		{
			InitializeComponent();
		}

		public void Rate(object sender, EventArgs e) 
		{
			new MarketplaceReviewTask().Show();
		}

		public void Email(object sender, EventArgs e) 
		{
			new EmailComposeTask 
			{
				To = "rhymecheat@hotmail.com",
				Subject = "Bantu v0.1"
			}.Show();
		}

		public void OtherApps(object sender, EventArgs e) 
		{
			new MarketplaceSearchTask 
			{
				ContentType = MarketplaceContentType.Applications,
				SearchTerms = "rhymecheat"
			}.Show();
		}
	}
}