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
	public partial class MapPage : ContentPage
	{
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
		public MapPage(string role, Building building, string name)
		{
			InitializeComponent();

			// Creates campuses objects and draws them
			campus = map.AddCampus(name);

			// Assigns title of page to building that is to be going to
			this.Title = building.Name;

			Task addBuild = map.AddBuildings(building.Name, campus);
			map.AddLots(campus);

			Task<Lot> lot = map.FindClosestLot(addBuild, building, campus);

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

			UpdateLotInStack(lot, stack);

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
			};
		}

		async void OnClicked(object sender, EventArgs args)
		{
			var button = (Button)sender;
			var pos = (Position)button.CommandParameter;

			var geocoder = new Xamarin.Forms.GoogleMaps.Geocoder();
			var addresses = await geocoder.GetAddressesForPositionAsync(pos);

			if (addresses.Count() > 0)
			{
				switch (Device.OS)
				{
					case TargetPlatform.iOS:
						Device.OpenUri(
							new Uri(string.Format("http://maps.apple.com/?q={0}",
							                      WebUtility.UrlEncode(pos.Latitude.ToString() + ", " + pos.Longitude.ToString()))));
						break;
					case TargetPlatform.Android:
						Device.OpenUri(
							new Uri(string.Format("geo:0,0?q={0}", WebUtility.UrlEncode(addresses.First()))));
						break;
				};
			}
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

		public async Task UpdateLotInStack(Task<Lot> lot, StackLayout stack)
		{
			await lot;
			int i = 0;
			double lat = 0;
			double lon = 0;
			stack.Children[0].BindingContext = new { LotID = lot.Result.ID };
			stack.Children[1].BindingContext = new { Percent = lot.Result.Percentage };
			foreach (Location loc in lot.Result.Locations)
			{
				lat +=loc.Lat;
				lon += loc.Long;
				i += 1;
			}
			stack.Children[3].BindingContext = new { Pos = new Position(lat / i, lon / i) };
		}
	}
}