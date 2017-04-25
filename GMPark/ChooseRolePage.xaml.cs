using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace GMPark
{
	public partial class ChooseRolePage : ContentPage
	{
		private string name;
		public ChooseRolePage(string campusName)
		{
			var map = (GMTEMap)Application.Current.Properties["map"];
			this.BackgroundColor = Color.FromRgb(104, 151, 243);

			NavigationPage.SetBackButtonTitle(this, "");

			List<Role> roles = map.GetRoles(campusName);

			this.name = campusName;
			var grid = new Grid();
			int i = 0;
			foreach (Role role in roles)
			{
				grid.RowDefinitions.Add(new RowDefinition { Height = 100 });
				var click = new Button()
				{
					Text = role.GetName(),
					Font = Font.SystemFontOfSize(NamedSize.Large),
					TextColor = Color.White,
					BackgroundColor=Color.Transparent,
					FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
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
			var buildings = (List<Building>)button.CommandParameter;
			await Navigation.PushAsync(new WhereAreYouGoingPage(button.Text,this.name));
		}
	}
}
