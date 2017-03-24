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
		private Campus campus;
		private Position pos;
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
			this.pos = pos;
			var assembly = typeof(Main).GetTypeInfo().Assembly;
			Stream stream = assembly.GetManifestResourceStream("GMPark.campuses.json");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}
			var scroll = new ScrollView();

			this.campuses = JsonConvert.DeserializeObject<List<Campus>>(text);
			int i = 0;
			foreach (Campus c in this.campuses)
			{
				if (c.Name == this.name)
				{
					break;
				}
				i += 1;
			}
			this.campus = this.campuses[i];
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
			this.Title = name;
			Label cName = new Label
			{
				Text = "Campus: N/A",
				TextColor = Color.White,
				BackgroundColor = Color.FromRgb(104, 151, 243),
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
			};
			Label r = new Label
			{
				Text = "Role: N/A",
				TextColor = Color.White,
				BackgroundColor = Color.FromRgb(104, 151, 243),
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
			};
			Label b = new Label
			{
				Text = "Building: N/A",
				TextColor = Color.White,
				BackgroundColor = Color.FromRgb(104, 151, 243),
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
			};
			if (Application.Current.Properties.ContainsKey("campus"))
			{
				cName.Text = "Campus: " + Application.Current.Properties["campus"];
			}
			if (Application.Current.Properties.ContainsKey("role"))
			{
				r.Text = "Role: " + Application.Current.Properties["role"];
			}
			if (Application.Current.Properties.ContainsKey("building"))
			{
				b.Text = "Building: " + Application.Current.Properties["building"];
			}


			var nd = new Button()
			{
				Text = "New Destination",
				//TextColor = Color.Red,
				Font = Font.SystemFontOfSize(NamedSize.Large),
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS")
			};
			nd.Clicked += newdes;

			var go = new Button()
			{
				Text = "Go!",
				TextColor = Color.Red,
				Font = Font.SystemFontOfSize(NamedSize.Large),
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS")
			};

			var stack = new StackLayout { Spacing = 0, VerticalOptions = LayoutOptions.FillAndExpand };

			stack.Children.Add(cName);
			stack.Children.Add(r);
			stack.Children.Add(b);
			stack.Children.Add(map);
			stack.Children.Add(nd);
			stack.Children.Add(go);


			scroll.Content = stack;
			Content = scroll;

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
			ToolbarItems.Add(new ToolbarItem("Preference", "preference.png", () =>
			{
				Navigation.PushAsync(new EnterUserInfoPage(this.campus, this.pos));
			}));

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
			await Navigation.PushAsync(new ChooseRolePage(this.campuses[i]));
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