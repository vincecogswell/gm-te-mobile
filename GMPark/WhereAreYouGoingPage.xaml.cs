using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace GMPark
{
	public partial class WhereAreYouGoingPage : ContentPage
	{
		public WhereAreYouGoingPage()
		{
			InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, "");

			Title = "Where are you going?";

			var next = new ToolbarItem
			{
				Text = "Next",
				Command = new Command(GoToNextPage)
			};

			ToolbarItems.Add(next);

			listView.ItemsSource = new List<string>
			{
				"Cadillac", "Design Building", "Design Dome",
				"ID Center", "Man A", "Man B", "Research & Development", "Vehicle Engineering Center"
			};
		}

		public void GoToNextPage()
		{
			Navigation.PushAsync(new MapPage());
		}
	}
}
