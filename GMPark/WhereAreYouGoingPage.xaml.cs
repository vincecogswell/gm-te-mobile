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

		public WhereAreYouGoingPage(string role)
		{
			InitializeComponent();

			var scroll = new ScrollView();

			var assembly = typeof(WhereAreYouGoingPage).GetTypeInfo().Assembly;
			Stream stream = assembly.GetManifestResourceStream("GMPark.campuses.json");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}

			List<Campus> campuses = JsonConvert.DeserializeObject<List<Campus>>(text);

			var grid = new Grid();
			int i = 0;
			foreach (Campus campus in campuses)
			{
				grid.RowDefinitions.Add(new RowDefinition { Height = 50 });

				var label = new Label()
				{
					Text = campus.Name,
					TextColor = Color.Black
				};

				grid.Children.Add(label, 0, i);
				i += 1;

				foreach (Building building in campus.Buildings)
				{
					grid.RowDefinitions.Add(new RowDefinition { Height = 100 });

					var click = new Button()
					{
						Text = building.Name,
						Font = Font.SystemFontOfSize(NamedSize.Large),
						FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
						CommandParameter = building
					};
					click.Clicked += OnClicked;

					grid.Children.Add(click, 0, i);
					i += 1;
				}
			}

			NavigationPage.SetBackButtonTitle(this, "");

			Title = "Where are you going?";

			this.role = role;
			scroll.Content = grid;
			Content = scroll;
		}

		async void OnClicked(object sender, EventArgs args)
		{
			var button = (Button)sender;
			var building = (Building)button.CommandParameter;
			await Navigation.PushAsync(new MapPage(this.role, building));
		}
	}
}
