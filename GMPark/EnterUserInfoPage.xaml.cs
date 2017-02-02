using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace GMPark
{
	public partial class EnterUserInfoPage : ContentPage
	{
		public EnterUserInfoPage()
		{
			InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, "");

			Title = "User Info";

			var next = new ToolbarItem
			{
				Text = "Next",
				Command = new Command(GoToNextPage)
			};

			ToolbarItems.Add(next);
		}

		public void GoToNextPage()
		{
			Navigation.PushAsync(new WhereAreYouGoingPage());
		}
	}
}
