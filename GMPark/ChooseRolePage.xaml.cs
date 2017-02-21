﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace GMPark
{
	public partial class ChooseRolePage : ContentPage
	{
		public ChooseRolePage()
		{
			InitializeComponent();

			this.BackgroundColor = Color.FromRgb(104, 151, 243);
			NavigationPage.SetBackButtonTitle(this, "");

			List<string> roles = new List<string>();
			roles.Add("Employee");
			roles.Add("Executive");
			roles.Add("Visitor");
			var grid = new Grid();
			int i = 0;
			foreach (string role in roles)
			{
				grid.RowDefinitions.Add(new RowDefinition { Height = 100 });
				var click = new Button()
				{
					Text = role,
					Font = Font.SystemFontOfSize(NamedSize.Large),
					TextColor = Color.White,
					BackgroundColor=Color.Transparent,
					FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
					CommandParameter = role,
					BorderWidth = 1,
					BorderColor = Color.White,
					Margin = new Thickness(8,8,8,8)
				};
				click.Clicked += OnClicked;

				grid.Children.Add(click, 0, i);
				i += 1;
			};
			Title = "Select a Role";
			Content = grid;
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
