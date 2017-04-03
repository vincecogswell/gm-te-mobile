using System;
using System.Net;
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
	public partial class MapPage : ContentPage
	{
		public static double MPH = 2.2352;
		public static double TimerMax = 300;
		HttpClient client;
		GMTEMap map = new GMTEMap()
		{
			IsShowingUser = true,
			HeightRequest = 100,
			WidthRequest = 960,
			VerticalOptions = LayoutOptions.FillAndExpand,
			HasZoomEnabled = true
		};

		Campus campus;
		bool onCampus = false;
		string mCurrentCampus = "";
		string cName, bName;
		string mCurrentLot = "";
		string mLotParked = "";
		bool mTimerStarted = false;
		double mTimerLength = 0;
		bool mParked = false;

		public MapPage(string selectedRole, string buildingName, string campusName)
		{
			InitializeComponent();

			cName = campusName;
			bName = buildingName;

			if (Application.Current.Properties.ContainsKey("map"))
			{
				map = (GMTEMap)Application.Current.Properties["map"];
			}

			else
			{
				// Creates campuses objects and draws them
				map.AddCampus(campusName);

				// Assigns title of page to building that is to be going to
				this.Title = buildingName;

				Task addBuild = map.DrawBuildings(campusName);
				map.DrawLots(campusName);
			}

			var lotTask = GetLotOrder();

			client = new HttpClient();
			client.MaxResponseContentBufferSize = 256000;

			var lotLabel = new Label
			{
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
				TextColor = Color.Blue
			};

			var percentLabel = new Label
			{
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
				TextColor = Color.Blue,
			};

			lotLabel.SetBinding(Label.TextProperty, new Binding("LotID", stringFormat: "The closest parking lot is Lot {0}"));
			percentLabel.SetBinding(Label.TextProperty, new Binding("Percent", stringFormat: "The lot is {0}% full"));

			var button = new Button()
			{
				Text = "Start Directions!",
				Font = Font.SystemFontOfSize(NamedSize.Large),
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
				CommandParameter = new double()
			};

			button.Clicked += OnClicked;
			button.SetBinding(Button.CommandParameterProperty, new Binding("Pos"));

			var stack = new StackLayout { Spacing = 0, VerticalOptions = LayoutOptions.FillAndExpand };

			stack.Children.Add(lotLabel);
			stack.Children.Add(percentLabel);
			stack.Children.Add(map);
			stack.Children.Add(button);
			stack.BindingContext = new { LotID = "36" };

			stack.BindingContext = stack.Children[0].BindingContext;

			this.Content = stack;

			UpdateLotInStack(lotTask, stack);

			map.SpanToBuilding(buildingName, campusName);

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

				if ((map.CheckInLotGeofences(args.Position, mCurrentCampus) != null) && (mParked == false))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						mCurrentLot = map.CheckInLotGeofences(args.Position, mCurrentCampus);
						DisplayAlert("You are in lot " + mCurrentLot + "!", "We hope you find a spot!", "Okay");
					});
				}

				else if ((map.CheckInLotGeofences(args.Position, mCurrentCampus) == null) && (mParked == false))
				{
					mCurrentLot = "";
				}

				else if (mParked)
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						DisplayAlert("You Parked!", "We detected that you parked in " + mLotParked, "Okay");
					});
				}

				else if ((map.CheckInLotGeofences(args.Position, mCurrentCampus) == null) && (mCurrentLot != "") 
				         && (mTimerStarted == false) && (mParked == false))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						DisplayAlert("Now leaving " + mCurrentLot, "Start parking-detection algorithm", "Start");
					});
					mLotParked = mCurrentLot;
					mCurrentLot = "";
					mTimerStarted = true;
					Device.StartTimer(TimeSpan.FromSeconds(.5), new Func<bool>(() => CheckSpeed(args.Position)));
				}
			};
		}

		async void OnClicked(object sender, EventArgs args)
		{
			var button = (Button)sender;
			var pos = (Position)button.CommandParameter;

			switch (Device.OS)
			{
				case TargetPlatform.iOS:
					Device.OpenUri(
						new Uri(string.Format("http://maps.apple.com/?q={0}",
						                      WebUtility.UrlEncode(pos.Latitude.ToString() + " " + pos.Longitude.ToString()))));
					break;
					
				case TargetPlatform.Android:
					Device.OpenUri(
						new Uri(string.Format("geo:0,0?q={0}", WebUtility.UrlEncode(pos.Latitude.ToString() + ", " + pos.Longitude.ToString()))));
					break;
			};
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

		public async Task UpdateLotInStack(Task<List<int>> lots, StackLayout stack)
		{
			await lots;
			int i = 0;
			double lat = 0;
			double lon = 0;

			var lot = map.GetLotById(cName, lots.Result.First());
			stack.Children[0].BindingContext = new { LotID = lot.GetName() };
			//stack.Children[1].BindingContext = new { Percent = lot.Result.Percentage };

			/*foreach (Location loc in lot.Result.Locations)
			{
				lat +=loc.Lat;
				lon += loc.Long;
				i += 1;
			}
			stack.Children[3].BindingContext = new { Pos = new Position(lat / i, lon / i) };*/
		}

		public async Task<List<int>> GetLotOrder()
		{
			var text = "http://35.9.22.105/predictive-parking/" + map.GetCampusId(cName) + "/" +
																	 map.GetBuildingId(cName, bName);
			var uri = new Uri(text);
			var response = await client.GetAsync(uri);
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var server = JsonConvert.DeserializeObject<ServerJSONLotOrder>(content);

				return server.lot_order;
			}
			else
			{
				return null;
			}
		}

		public bool CheckSpeed(Plugin.Geolocator.Abstractions.Position pos)
		{
			if (pos.Speed > MPH)
			{
				mTimerStarted = false;
				mTimerLength = 0;
				mLotParked = "";
				return false;
			}

			else
			{
				mTimerLength += .5;

				if (mTimerLength > TimerMax)
				{
					mParked = true;
					return false;
				}
					
				return true;
			}
		}
	}
}