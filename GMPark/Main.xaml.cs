using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Geolocator;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GMPark
{
	public partial class Main : ContentPage
	{
		HttpClient client;

		private string name;
		private Position pos;
		private List<Campus> campuses;
		public Campus campus;
		bool onCampus = false;
		string mCurrentCampus = "";

		GMTEMap map = new GMTEMap()
		{
			IsShowingUser = true,
			HeightRequest = 100,
			WidthRequest = 960,
			VerticalOptions = LayoutOptions.FillAndExpand,
			HasZoomEnabled = true
		};

		public Main(string campusName)
		{
			InitializeComponent();

			mCurrentCampus = campusName;

			Application.Current.Properties["map"] = map;

			client = new HttpClient();
			client.MaxResponseContentBufferSize = 256000;

			Task<ServerJSON> thing = GetCampuses();
			var campusTask = ConvertCampuses(thing);
			var buildingTask = GetBuildings(campusTask);
			var lotTask = GetLots(campusTask);
			var roleTask = GetRoles(campusTask);

			var scroll = new ScrollView();

			// Assigns title of page to building that is to be going to
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
			go.IsEnabled = false;

			if ((Application.Current.Properties.ContainsKey("building")) && (Application.Current.Properties.ContainsKey("role"))
				&& (Application.Current.Properties.ContainsKey("campus")) && (name == (string)Application.Current.Properties["campus"]))
			{
				go.IsEnabled = true;
			}

			else
			{
				go.IsEnabled = false;
			}

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
				if (!Application.Current.Properties.ContainsKey("notification"))
				{
					Application.Current.Properties["notification"] = 0;
				}

				if ((map.CheckInGeofences(args.Position))
				    && (onCampus == false) && ((int)Application.Current.Properties["notification"] == 0))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						mCurrentCampus = map.InWhichGeofences(args.Position);
						DisplayAlert("Welcome to " + mCurrentCampus + "!", "We hope you find your way around!", "Okay");
						onCampus = true;
						Application.Current.Properties["notification"] = 1;
					});
				}

				else if ((map.CheckInGeofences(args.Position) == false)
				         && (onCampus == true) && ((int)Application.Current.Properties["notification"] == 1))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						DisplayAlert("Now leaving " + mCurrentCampus, "Did you mean to do that?", "Maybe?");
						onCampus = false;
						mCurrentCampus = "";
						Application.Current.Properties["notification"] = 0;
					});
				}
			};
			ToolbarItems.Add(new ToolbarItem("Preference", "preference.png", () =>
			{
				Navigation.PushAsync(new EnterUserInfoPage(this.campus, this.pos));
			}));

			/*go.Clicked += async (sender, e) =>
			{
				var lotTask = await map.FindClosestLot(addBuild, (string)(Application.Current.Properties["building"]),
										 (string)(Application.Current.Properties["campus"]));

				if (lotTask == null)
				{
					//do nothing
				}

				else
				{
					var lot = lotTask;

					double avgLat = 0, avgLon = 0, count = lot.Locations.Count();
					foreach (Location loc in lot.Locations)
					{
						avgLat += loc.Lat;
						avgLon += loc.Long;
					}

					var lotPos = new Position(avgLat / count, avgLon / count);

					switch (Device.OS)
					{
						case TargetPlatform.iOS:
							Device.OpenUri(
								new Uri(string.Format("http://maps.apple.com/?q={0}",
													  WebUtility.UrlEncode(lotPos.Latitude.ToString() + " " + lotPos.Longitude.ToString()))));
							break;

						case TargetPlatform.Android:
							Device.OpenUri(
								new Uri(string.Format("geo:0,0?q={0}", WebUtility.UrlEncode(lotPos.Latitude.ToString() + ", " + lotPos.Longitude.ToString()))));
							break;
					};
				}
			};*/
		}


		async void newdes(object sender, EventArgs args)
		{
			await Navigation.PushAsync(new ChooseRolePage(this.Title));
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

		public async Task GetBuildings(Task<List<Campus>> converted)
		{
			List<Campus> campuses = await converted;

			foreach (Campus campus in campuses)
			{
				string text = "http://35.9.22.105/campuses/" + campus.GetId() + "/buildings";
				var uri = new Uri(text);
				var response = await client.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					campus.ConvertBuildings(JsonConvert.DeserializeObject<ServerJSONBuildings>(content));
					await map.DrawBuildings(campus.GetName());
				}
			}
		}

		public async Task GetLots(Task<List<Campus>> converted)
		{
			List<Campus> campuses = await converted;

			foreach (Campus campus in campuses)
			{
				string text = "http://35.9.22.105/campuses/" + campus.GetId() + "/lots";
				var uri = new Uri(text);
				var response = await client.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					campus.ConvertLots(JsonConvert.DeserializeObject<ServerJSONLots>(content));
					map.DrawLots(campus.GetName());
				}
			}
		}

		public async Task GetRoles(Task<List<Campus>> converted)
		{
			List<Campus> campuses = await converted;

			foreach (Campus campus in campuses)
			{
				string text = "http://35.9.22.105/campuses/" + campus.GetId() + "/roles";
				var uri = new Uri(text);
				var response = await client.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					campus.AddRoles(JsonConvert.DeserializeObject<ServerJSONRoles>(content));
					map.DrawLots(campus.GetName());
				}
			}
		}

		public async Task<List<Campus>> ConvertCampuses(Task<ServerJSON> json)
		{
			await json;
			var res = json.Result;
			campuses = new List<Campus>();
			string first = mCurrentCampus, firstKey = "";

			foreach (KeyValuePair<string, SCampus> entry in res.campuses)
			{
				var campus = new Campus();
				campus.ConvertToCampus(entry.Value);
				campus.SetId(entry.Key);
				campuses.Add(campus);

				if (first == "")
				{
					first = entry.Value.name;
					firstKey = entry.Key;
				}

				else if (first == campus.Name)
				{
					firstKey = entry.Key;
				}
			}

			map.AddCampuses(campuses);
			map.SpanToCampus(json.Result.campuses[firstKey].name);
			this.Title = first;

			App.Menu.AddButtons();

			return campuses;
		}

	}
}