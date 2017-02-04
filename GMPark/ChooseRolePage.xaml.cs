using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace GMPark
{
	public partial class ChooseRolePage : ContentPage
	{
		public ChooseRolePage()
		{
			InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, "");

			Title = "Select a Role";
		}

		async void OnClicked(object sender, EventArgs args)
		{
			Button button = (Button)sender;
			await Navigation.PushAsync(new WhereAreYouGoingPage(button.Text));
		}

		//async void VisitorClicked(object sender, EventArgs args)
		//{
		//	await Navigation.PushAsync(new WhereAreYouGoingPage());
		//}
	}
}
