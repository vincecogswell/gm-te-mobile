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
			Stream stream = assembly.GetManifestResourceStream("GMPark.Structures.json");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}

			List<structure> structures = JsonConvert.DeserializeObject<List<structure>>(text);

			var grid = new Grid();
			int i = 0;

			foreach (structure building in structures)
			{
				grid.RowDefinitions.Add(new RowDefinition { Height = 100 });

					var click = new Button()
					{
						Text = building.Name,
						Font = Font.SystemFontOfSize(NamedSize.Large)
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
			Button button = (Button)sender;
			await Navigation.PushAsync(new MapPage(this.role,button.Text));
		}
	}
}
