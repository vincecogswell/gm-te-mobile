/* Author : Phyllis Jin
 * List all building names in selected campus
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace GMPark
{
	public partial class WhereAreYouGoingPage : ContentPage
	{
		private string role;
		private string campus;

		public WhereAreYouGoingPage(string selectedRole, string campusName)
		{
			// initialize map
			var map = (GMTEMap)Application.Current.Properties["map"];
			//UI
			this.BackgroundColor = Color.FromRgb(104, 151, 243);
			var scroll = new ScrollView();

			var grid = new Grid();
			int i = 0;

			campus = campusName;

			var buildings = map.GetBuildingList(campusName);

			foreach (string building in buildings)
			{
				grid.RowDefinitions.Add(new RowDefinition { Height = 100 });

				var click = new Button()
				{
					Text = building,
					Font = Font.SystemFontOfSize(NamedSize.Large),
					TextColor = Color.White,
					FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
					CommandParameter = building,
					BorderWidth = 1,
					BorderColor = Color.White,
					Margin = new Thickness(8, 8, 8, 8),
					BackgroundColor = Color.Transparent,
				};
				click.Clicked += OnClicked;

				grid.Children.Add(click, 0, i);
				i += 1;
			}

			NavigationPage.SetBackButtonTitle(this, "");

			Title = "Building Selection";

			this.role = selectedRole;
			this.campus = campusName;
			scroll.Content = grid;
			Content = scroll;
		}

		// it will ask the user if they want to update the preference
		async void OnClicked(object sender, EventArgs args)
		{
			var button = (Button)sender;
			var ans = await DisplayAlert("Update Preference?", "Would you like to update your preference?", "Yes", "No");
			if (ans == true)
			{
				Application.Current.Properties[campus + "campus"] = this.campus;
				Application.Current.Properties[campus + "role"] = this.role;
				Application.Current.Properties[campus + "building"] = button.Text;
			}
			await Navigation.PushAsync(new MapPage(this.role, button.Text, this.campus));
		}
	}
}
