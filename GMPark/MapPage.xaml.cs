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
		StackLayout stack = new StackLayout() 
		{ 
			Spacing = 0, 
			VerticalOptions = LayoutOptions.FillAndExpand 
		};

		List<int> mLotOrder;
		int mGoingTo;
		bool onCampus = false;
		string mCurrentCampus = "";
		string mCampusName, mBuildingName;
		string mCurrentLot = "";
		string mLotParked = "";
		string mRole = "";
		bool mTimerStarted = false;
		double mTimerLength = 0;
		bool mParked = false;
		bool inLot = false;

		public MapPage(string selectedRole, string buildingName, string campusName)
		{
			InitializeComponent();

			mCampusName = campusName;
			mBuildingName = buildingName;
			mRole = selectedRole;
			this.Title = mBuildingName;

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

			client = new HttpClient();
			client.MaxResponseContentBufferSize = 256000;

			var lotTask = GetLotOrder();

			var lotLabel = new Label
			{
				FontFamily = font,
				TextColor = Color.Blue
			};

			var goingToLabel = new Label
			{
				FontFamily = font,
				TextColor = Color.Green,
			};

			lotLabel.SetBinding(Label.TextProperty, new Binding("Lots", stringFormat:"{0}"));
			goingToLabel.SetBinding(Label.TextProperty, new Binding("GoingTo", stringFormat: "Going to: {0}"));

			var button = new Button()
			{
				Text = "Start Directions!",
				Font = Font.SystemFontOfSize(NamedSize.Large),
				FontFamily = font,
				CommandParameter = new double()
			};

			button.Clicked += OnClicked;
			button.SetBinding(Button.CommandParameterProperty, new Binding("Pos"));

			stack.Children.Add(lotLabel);
			stack.Children.Add(goingToLabel);
			stack.Children.Add(map);
			stack.Children.Add(button);
			stack.BindingContext = new { Lots = "Retrieving closest lots..." };

			stack.BindingContext = stack.Children[0].BindingContext;

			this.Content = stack;

			Task stackTask = ConvertLots(lotTask);
			AwaitTask(stackTask);

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

				if ((map.CheckInLotGeofences(args.Position, mCurrentCampus) != null) && (mParked == false) && (inLot == false))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						mCurrentLot = map.CheckInLotGeofences(args.Position, mCurrentCampus);
						DisplayAlert("You are in lot " + mCurrentLot + "!", "We hope you find a spot!", "Okay");
					});

					inLot = true;
				}

				if (mParked)
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						DisplayAlert("You Parked!", "We detected that you parked in " + mLotParked, "Okay");
					});
				}

				if ((map.CheckInLotGeofences(args.Position, mCurrentCampus) == mCurrentLot) && (mCurrentLot != "") 
				         && (mTimerStarted == false) && (mParked == false))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						DisplayAlert("Now leaving " + mCurrentLot, "Start parking-detection algorithm", "Start");
					});
					mLotParked = mCurrentLot;
					mCurrentLot = "";
					mTimerStarted = true;
					inLot = false;
					Device.StartTimer(TimeSpan.FromSeconds(.5), new Func<bool>(() => CheckSpeed(args.Position)));
				}

				if ((map.CheckInLotGeofences(args.Position, mCurrentCampus) == null) && (mParked == false))
				{
					mCurrentLot = "";
					inLot = false;
				}
			};
		}

		async void OnClicked(object sender, EventArgs args)
		{
			map.NavigateToLot(mCampusName, mLotOrder[0]);
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

			var lot = map.GetLotById(mCampusName, lots.Result.First());
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
			string campId = map.GetCampusId(mCampusName);
			string buildId = map.GetBuildingId(mCampusName, mBuildingName);
			var text = "http://35.9.22.105/predictive-parking/" + campId + "/" + buildId;
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

		public async Task ConvertLots(Task<List<int>> server)
		{
			var lots = await server;
			mLotOrder = map.PurgeLotList(mCampusName,mRole,lots);
			int lotCount = mLotOrder.Count;
			string lotOrderString = "";
			mGoingTo = 0;

			map.SpanToLotsAndBuildings(mCampusName, mBuildingName, mLotOrder);

			if (lotCount > 3)
			{
				lotCount = 3;
			}

			for (int i = 0; i < lotCount; i++)
			{
				//.Add(map.GetLotById(mCampusName, lotId));

				if (i == 0)
				{
					lotOrderString += "The best lot is " + map.GetLotName(mCampusName, mLotOrder[i].ToString());
				}

				else if (i == 1)
				{
					lotOrderString += ", then " + map.GetLotName(mCampusName, mLotOrder[i].ToString());
				}

				else
				{
					lotOrderString += ", then " + map.GetLotName(mCampusName, mLotOrder[i].ToString());
				}

			}
			stack.Children[0].BindingContext = new { Lots = lotOrderString };
			stack.Children[1].BindingContext = new { GoingTo = map.GetLotName(mCampusName, mLotOrder[mGoingTo].ToString()) };
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

		public async void AwaitTask(Task task)
		{
			await task;
		}
	}
}