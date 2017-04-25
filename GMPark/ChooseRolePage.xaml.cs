/* Author : Phyllis Jin
 * List all role names in selected campus
 */
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace GMPark
{
	public partial class ChooseRolePage : ContentPage
	{
		//variable
		private string name;

		// consturctor
		public ChooseRolePage(string campusName)
		{
			// initialze map
			var map = (GMTEMap)Application.Current.Properties["map"];
			// get all role names
			List<Role> roles = map.GetRoles(campusName);
			//UI
			this.BackgroundColor = Color.FromRgb(104, 151, 243);
			NavigationPage.SetBackButtonTitle(this, "");
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
		// button click function
		async void OnClicked(object sender, EventArgs args)
		{
			Button button = (Button)sender;
			var buildings = (List<Building>)button.CommandParameter;
			await Navigation.PushAsync(new WhereAreYouGoingPage(button.Text,this.name));
		}
	}
}
