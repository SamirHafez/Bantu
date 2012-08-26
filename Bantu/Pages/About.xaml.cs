using System;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace Bantu.Pages
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