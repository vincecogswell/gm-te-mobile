/* Main page for this project
 */
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

		private List<Campus> campuses;
		private List<int> mLotOrder;
		private int mLotToGo;
		public Campus campus;
		bool onCampus = false;
		string mCurrentCampus = "";
		bool refresh = false;
		private Label l;

		// create the map
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

			client = new HttpClient();
			client.MaxResponseContentBufferSize = 256000;

			mCurrentCampus = campusName;
			Title = campusName;
			Application.Current.Properties["map"] = map;
			Refresh();

			var scroll = new ScrollView();
			string font;

			switch (Device.RuntimePlatform)
			{
				case "iOS":
					font = "AppleSDGothicNeo-UltraLight";
					break;
				case "Android": 
					font = "Droid Sans Mono";
					break;
				default:
					font = "Comic Sans MS";
					break;
			}

			// Assigns title of page to building that is to be going to

			//display user prefernece info
			Label cName = new Label
			{
				Text = "Campus: N/A",
				TextColor = Color.White,
				BackgroundColor = Color.FromRgb(104, 151, 243),
				FontFamily = font
			};
			Label r = new Label
			{
				Text = "Role: N/A",
				TextColor = Color.White,
				BackgroundColor = Color.FromRgb(104, 151, 243),
				FontFamily = font
			};
			Label b = new Label
			{
				Text = "Building: N/A",
				TextColor = Color.White,
				BackgroundColor = Color.FromRgb(104, 151, 243),
				FontFamily = font
			};
			l = new Label
			{
				Text = "Lot: N/A",
				TextColor = Color.White,
				BackgroundColor = Color.FromRgb(104, 151, 243),
				FontFamily = font
			};

			// All function buttons
			var nd = new Button()
			{
				Text = "New Destination",
				//TextColor = Color.Red,
				Font = Font.SystemFontOfSize(NamedSize.Large),
				FontFamily = font
			};
			nd.Clicked += newdes;

			var go = new Button()
			{
				Text = "Go!",
				TextColor = Color.Red,
				Font = Font.SystemFontOfSize(NamedSize.Large),
				FontFamily = font
			};
			go.IsEnabled = false;

			var call = new Button()
			{
				Text = "Call",
				Font = Font.SystemFontOfSize(NamedSize.Large),
				FontFamily = font
			};
			call.Clicked += callnum;

			// display the user's preference that stored in their phone
			if (Application.Current.Properties.ContainsKey(campusName + "campus"))
			{
				cName.Text = "Campus: " + Application.Current.Properties[campusName + "campus"];
			}
			if (Application.Current.Properties.ContainsKey(campusName + "role"))
			{
				r.Text = "Role: " + Application.Current.Properties[campusName + "role"];
			}
			if (Application.Current.Properties.ContainsKey(campusName + "building"))
			{
				b.Text = "Building: " + Application.Current.Properties[campusName + "building"];
			}

			// Go button is not clickable if their no user's preference stored
			if ((Application.Current.Properties.ContainsKey(campusName + "building")) && 
			    (Application.Current.Properties.ContainsKey(campusName + "role"))
			    && (Application.Current.Properties.ContainsKey(campusName + "campus")) && 
			    (campusName == (string)Application.Current.Properties[campusName + "campus"]))
			{
				go.IsEnabled = true;
			}

			else
			{
				go.IsEnabled = false;
			}

			// all elements
			var stack = new StackLayout { Spacing = 0, VerticalOptions = LayoutOptions.FillAndExpand };

			stack.Children.Add(cName);
			stack.Children.Add(r);
			stack.Children.Add(b);
			stack.Children.Add(l);
			stack.Children.Add(map);
			stack.Children.Add(nd);
			stack.Children.Add(go);
			stack.Children.Add(call);

			scroll.Content = stack;
			Content = scroll;

			NavigationPage.SetBackButtonTitle(this, "");

			// Geofencing
			AwaitSingle(StartGeoLocation());

			CrossGeolocator.Current.PositionChanged += (o, args) =>
			{
				if ((map.CheckInGeofences(args.Position))
				    && (onCampus == false))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						mCurrentCampus = map.InWhichGeofences(args.Position);
						DisplayAlert("Welcome to " + mCurrentCampus + "!", "We hope you find your way around!", "Okay");
						DependencyService.Get<ITextToSpeech>().Speak("Welcome to " + mCurrentCampus);
						onCampus = true;
						Application.Current.Properties["notification"] = 1;
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
						Application.Current.Properties["notification"] = 0;
					});
				}
			};

			// Preference button
			ToolbarItems.Add(new ToolbarItem("Preference", "preference.png", () =>
			{
				Navigation.PushAsync(new EnterUserInfoPage(this.Title));
			}));

			map.SpanToCampus(campusName);

			go.Clicked += async (sender, e) =>
			{
				map.NavigateToLot(this.Title, mLotOrder[mLotToGo]);
			};
		}


		// click funtion for New Destination button
		async void newdes(object sender, EventArgs args)
		{
			
			if (Application.Current.Properties.ContainsKey(this.Title + "role"))
			{
				await Navigation.PushAsync(new WhereAreYouGoingPage((string)Application.Current.Properties[this.Title + "role"], this.Title));
			}
			else
			{
				await Navigation.PushAsync(new ChooseRolePage(this.Title));
			}

		}

		// click function for calling a shuttle
		async void callnum(object sender, EventArgs args)
		{
			var ans = await DisplayAlert("Call", "123-123-1234", "Yes", "No");
			if (ans == true)
			{
				Device.OpenUri(new Uri(String.Format("tel:{0}", "+1231231234")));
			}
		}

		// start geofencing
		public async Task StartGeoLocation()
		{
			if (CrossGeolocator.Current.IsGeolocationEnabled)
			{
				if (!CrossGeolocator.Current.IsListening)
				{
					var listen = await CrossGeolocator.Current.StartListeningAsync(1, 1, false);
					if (!listen)
					{
						Device.BeginInvokeOnMainThread(() =>
						{
							DisplayAlert("Geolocation", "Is NOT listening", "Okay");
						});
					}
				}
			}

			else
			{
				DisplayAlert("Geolocation", "Is NOT enabled", "Okay");
			}
		}

		// get all campuses from database API
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

		// get all buildings from database API
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

		// get all lots from database API
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
		// get all roles from database API
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
				}
			}
		}

		// get all gates from database API
		public async Task GetGates(Task<List<Campus>> converted)
		{
			List<Campus> campuses = await converted;

			foreach (Campus campus in campuses)
			{
				string text = "http://35.9.22.105/campuses/" + campus.GetId() + "/gates";
				var uri = new Uri(text);
				var response = await client.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					campus.AddGates(JsonConvert.DeserializeObject<ServerJSONGates>(content));
					map.DrawGates(campus.GetName());
				}
			}
		}

		// get all top 3 best lots from database API
		public async Task GetLotOrder(string buildName, Task<List<Campus>> converted)
		{
			List<Campus> campuses = await converted;

			foreach (Campus campus in campuses)
			{
				if (campus.GetName() == this.Title)
				{
					string text = "http://35.9.22.105/predictive-parking/" + campus.GetId() + "/" 
					                                                               + campus.GetBuildingId(buildName);
					var uri = new Uri(text);
					var response = await client.GetAsync(uri);
					if (response.IsSuccessStatusCode)
					{
						var content = await response.Content.ReadAsStringAsync();
						mLotOrder = map.PurgeLotList(campus.GetName(),
						                             (string)Application.Current.Properties[campus.GetName() + "role"],
						                             JsonConvert.DeserializeObject<ServerJSONLotOrder>(content).lot_order);
						mLotToGo = 0;
						l.Text = "Lot: " + campus.GetLotName(mLotOrder[mLotToGo].ToString());
					}

				}
			}
		}
				
		// await tasks
		public async Task AwaitAll(Task thing, Task thing2, Task thing3, Task thing4)
		{
			await thing;
			await thing2;
			await thing3;
			await thing4;
			refresh = false;
		}

		// awit one task
		public async Task AwaitSingle(Task thing1)
		{
			await thing1;
		}

		// convert json file to campus ojects
		public async Task<List<Campus>> ConvertCampuses(Task<ServerJSON> json)
		{
			var res = await json;
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

		// refresh the page
		public async void Refresh()
		{
			refresh = true;
			Task buildTask, lotTask, roleTask, gateTask;

			Task<ServerJSON> thing = GetCampuses();
			var campusTask = ConvertCampuses(thing);
			buildTask = GetBuildings(campusTask);
			lotTask = GetLots(campusTask);
			roleTask = GetRoles(campusTask);
			gateTask = GetGates(campusTask);

			await AwaitAll(buildTask, lotTask, roleTask, gateTask);

			if ((Application.Current.Properties.ContainsKey(this.Title + "building")) &&
			    Application.Current.Properties.ContainsKey(this.Title + "role")
			&& (Application.Current.Properties.ContainsKey(this.Title + "campus")))
			{
				await GetLotOrder((string)Application.Current.Properties[this.Title + "building"], campusTask);
			}

		}

	}
}