using System;
using System.Net.Http;
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
		HttpClient client;

		private string name;
		private List<Campus> campuses;
		//Campus campus;
		bool onCampus = false;
		string mCurrentCampus = "";
		public Main(string name, Position pos)
		{
			InitializeComponent();

			client = new HttpClient();
			client.MaxResponseContentBufferSize = 256000;
			Task<ServerJSON> thing = GetCampuses();

			this.name = name;
			var assembly = typeof(Main).GetTypeInfo().Assembly;
			Stream stream = assembly.GetManifestResourceStream("GMPark.campuses.json");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}

			this.campuses = JsonConvert.DeserializeObject<List<Campus>>(text);
			GMTEMap map = new GMTEMap(
				MapSpan.FromCenterAndRadius(
						pos, Distance.FromMiles(0.7)))
			{
				IsShowingUser = true,
				HeightRequest = 100,
				WidthRequest = 960,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HasZoomEnabled = true
			};

			map.AddCampuses();

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

			StartGeoLocation();

			CrossGeolocator.Current.PositionChanged += (o, args) =>
			{
				if ((map.CheckInGeofences(args.Position))
					&& (onCampus == false))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						mCurrentCampus = map.InWhichGeofences(args.Position);
						DisplayAlert("Welcome to " + mCurrentCampus + "!", "We hope you find your way around!", "Okay");
						onCampus = true;
						var newthing = thing.Result;
					});
				}

				else if ((map.CheckInGeofences(args.Position) == false)
					&& (onCampus == true))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						DisplayAlert("Now leaving " + mCurrentCampus, "Did you mean to do that?", "Maybe?");
						onCampus = false;
						mCurrentCampus = "";
					});
				}
			};
		}


		async void newdes(object sender, EventArgs args)
		{
			int i = 0;
			foreach (Campus c in this.campuses)
			{
				if (c.GetName() == this.name)
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

		public void StartGeoLocation()
		{
			if (CrossGeolocator.Current.IsGeolocationEnabled)
			{
				if (!CrossGeolocator.Current.IsListening)
				{
					CrossGeolocator.Current.StartListeningAsync(1, 1, false);
				}
			}

			else
			{
				DisplayAlert("Geolocation", "Is NOT enabled", "Okay");
			}
		}

		public async Task<ServerJSON> GetCampuses()
		{
			var uri = new Uri("http://35.9.22.105/campuses");
			var response = await client.GetAsync(uri);
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<ServerJSON>(content);
			}
			else
			{
				return null;
			}
		}

	}
}
