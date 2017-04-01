using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace GMPark
{
	public partial class ChooseCampus : ContentPage
	{
		public ChooseCampus()
		{
			var map = (GMTEMap)Application.Current.Properties["map"];
			this.BackgroundColor = Color.FromRgb(104, 151, 243);
			var scroll = new ScrollView();

			var grid = new Grid();
			int i = 0;

			List<string> campuses = map.GetCampusList();

			foreach (string campusName in campuses)
			{
				grid.RowDefinitions.Add(new RowDefinition { Height = 100 });
				var click = new Button()
				{
					Text = campusName,
					Font = Font.SystemFontOfSize(NamedSize.Large),
					TextColor = Color.White,
					FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
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

			Title = "Choose a Campus";
			scroll.Content = grid;
			Content = scroll;
		}
		async void OnClicked(object sender, EventArgs args)
		{
			var button = (Button)sender;
			var campus = (Campus)button.CommandParameter;
			await Navigation.PushAsync(new ChooseRolePage("GM Tech Center"));
		}
	}
}
