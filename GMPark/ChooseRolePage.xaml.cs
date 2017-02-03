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

		async void EmployeeClicked(object sender, EventArgs args)
		{
			Button button = (Button)sender;
			await Navigation.PushAsync(new EnterUserInfoPage());
		}

		async void VisitorClicked(object sender, EventArgs args)
		{
			await Navigation.PushAsync(new WhereAreYouGoingPage());
		}
	}
}
