using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace GMPark
{
	public partial class WhereAreYouGoingPage : ContentPage
	{
		private string role;
		
		public WhereAreYouGoingPage(string role)
		{
			InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, "");

			Title = "Where are you going?";

			this.role = role;
		}

		async void OnClicked(object sender, EventArgs args)
		{
			Button button = (Button)sender;
			await Navigation.PushAsync(new MapPage(this.role,button.Text));
		}
	}
}
