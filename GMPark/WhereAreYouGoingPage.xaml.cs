using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
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
			Stream stream = assembly.GetManifestResourceStream("GMPark.msu-buildings.json");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}

			List<Building> buildings = JsonConvert.DeserializeObject<List<Building>>(text);

			var grid = new Grid();
			int i = 0;

			foreach (Building building in buildings)
			{
				grid.RowDefinitions.Add(new RowDefinition { Height = 100 });

					var click = new Button()
					{
						Text = building.Name,
						Font = Font.SystemFontOfSize(NamedSize.Large),
						CommandParameter = building
					};
				click.Clicked += OnClicked;

				grid.Children.Add(click, 0, i);
				i += 1;
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
