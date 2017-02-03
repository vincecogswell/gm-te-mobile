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

			var next = new ToolbarItem
			{
				Text = "Next",
				Command = new Command(GoToNextPage)
			};

			ToolbarItems.Add(next);
		}

		public void GoToNextPage()
		{
			Navigation.PushAsync(new EnterUserInfoPage());
		}
	}
}
