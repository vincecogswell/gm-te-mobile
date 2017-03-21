using System;
using System.Net;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GMPark
{
	public partial class Main : ContentPage
	{
		private string name;
		private List<Campus> campuses;
		//Campus campus;
		//bool onCampus = false;
		//GeoPoly campusGeofence = new GeoPoly();
		public Main(string name, Position pos)
		{
			this.name = name;
			var assembly = typeof(Main).GetTypeInfo().Assembly;
			Stream stream = assembly.GetManifestResourceStream("GMPark.campuses.json");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}

			this.campuses = JsonConvert.DeserializeObject<List<Campus>>(text);
			Map map = new Map(
				MapSpan.FromCenterAndRadius(
						pos, Distance.FromMiles(0.7)))
			{
				IsShowingUser = true,
				HeightRequest = 100,
				WidthRequest = 960,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HasZoomEnabled = true
			};

			// Assigns title of page to building that is to be going to
			this.Title = "Select a Campus";


			var nd = new Button()
			{
				Text = "New Destination",
				//TextColor = Color.Red,
				Font = Font.SystemFontOfSize(NamedSize.Large),
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS")
			};
			nd.Clicked += newdes;

			var pref = new Button()
			{
				Text = "Preference",
				Font = Font.SystemFontOfSize(NamedSize.Large),
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS")
			};

			pref.Clicked += prf;

			var go = new Button()
			{
				Text = "Go!",
				TextColor = Color.Red,
				Font = Font.SystemFontOfSize(NamedSize.Large),
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS")
			};

			var stack = new StackLayout { Spacing = 0, VerticalOptions = LayoutOptions.FillAndExpand };


			stack.Children.Add(map);
			stack.Children.Add(nd);
			stack.Children.Add(pref);
			stack.Children.Add(go);


			this.Content = stack;

			NavigationPage.SetBackButtonTitle(this, "");
		}
		async void newdes(object sender, EventArgs args)
		{
			int i = 0;
			foreach (Campus c in this.campuses){
				if (c.Name == this.name)
				{
					break;
				}
				i += 1;
			}
			var campus = this.campuses[i];
			await Navigation.PushAsync(new ChooseRolePage(campus));
		}

		async void prf(object sender, EventArgs args)
		{
			await Navigation.PushAsync(new EnterUserInfoPage());
		}
	}
}
