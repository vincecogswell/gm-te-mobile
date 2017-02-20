using System;
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
		public MapPage(string role, Building building)
		{
			InitializeComponent();
			//message.Text = "You are a" + role + ", and going to" + building "
<<<<<<< HEAD
			Device.BeginInvokeOnMainThread(async() =>
			{
				await DisplayAlert("Welcome", "You Are In MSU Campus", "OK");

				string option = await DisplayActionSheet("EGR Parking Lot", "Cancel", null, "Navigate", "Next");
				if (option == "Navigate")
				{
					// Go to the direction page
				}
				else if (option == "Next")
				{
					// Display another action sheet
				};
			});


=======
>>>>>>> e9984b5... Added json parsing for full campuses

			var map = new Xamarin.Forms.GoogleMaps.Map(
				MapSpan.FromCenterAndRadius(
						new Position(0, 0), Distance.FromMiles(0.3)))
			{
				IsShowingUser = true,
				HeightRequest = 100,
				WidthRequest = 960,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HasZoomEnabled = true
			};

			Device.BeginInvokeOnMainThread(() =>
				{
					DisplayAlert("Welcome", "To the GM Technical Center", "Okay");
				});

			List<Campus> campuses = AddCampuses(map);

			foreach (Campus campus in campuses)
			{
				Task addBuild = AddBuildings(map, campus, building.Name);
				AddLots(map, campus);
			}

			foreach (Pin pin in map.Pins)
			{
				continue;
			}

			var label = new Label
			{
				Text = "Closest Parking Lot Is: 39, it is 50% full",
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS"),
				TextColor = Color.Blue
			};

			var button = new Button()
			{
				Text = "Start Directions!",
				Font = Font.SystemFontOfSize(NamedSize.Large),
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS")
			};

			var stack = new StackLayout { Spacing = 0, VerticalOptions = LayoutOptions.FillAndExpand };
			stack.Children.Add(label);
			stack.Children.Add(map);
			stack.Children.Add(button);
			this.Content = stack;

			foreach (Pin pin in map.Pins)
			{
				if (building.Name == pin.Label)
				{
					map.MoveToRegion(MapSpan.FromCenterAndRadius(pin.Position, Distance.FromMeters(150)));
				}
			}

			/*var locator = CrossGeolocator.Current;
			locator.PositionChanged += (sender, e) =>
			{
				var position = e.Position;
			};*/

		}


		public async Task PlaceBuildingPin(Building building, Map map, bool current)
		{
			var geocoder = new Xamarin.Forms.GoogleMaps.Geocoder();
			var positions = await geocoder.GetPositionsForAddressAsync(building.Address);

			if (positions.Count() > 0)
			{
				building.Position = new Location()
				{
					Lat = positions.First().Latitude,
					Long = positions.First().Longitude
				};

				Pin pin = new Pin
				{
					Type = PinType.Place,
					Label = building.Name,
					Address = building.Address,
					Position = positions.First()
				};

				if (current)
				{
					map.MoveToRegion(MapSpan.FromCenterAndRadius(pin.Position, Distance.FromMeters(150)));
				}

				map.Pins.Add(pin);
				//var format = "0.00";
				//this.Text = $"Center = {reg.Center.Latitude.ToString(format)}, {reg.Center.Longitude.ToString(format)}";
			}
			else
			{
				await this.DisplayAlert("Not found", "Geocoder returns no results", "Close");
			}
		}

		/** Gets the campuses from the campuses.json file, converts them to a campus object, then creates
		 * a polygon object to visually represent it and adds it to the map.
		 * Map map map that campuses are to be added to.
		 */
		public List<Campus> AddCampuses(Map map)
		{
			var assembly = typeof(MapPage).GetTypeInfo().Assembly;
			Stream stream = assembly.GetManifestResourceStream("GMPark.campuses.json");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}

			List<Campus> campuses = JsonConvert.DeserializeObject<List<Campus>>(text);
			///double maxLat = -91, maxLong = -181, minLat = 91, minLong = 181;

			foreach (Campus campus in campuses)
			{
				var polygon = new Polygon();
				polygon.IsClickable = true;
				polygon.StrokeColor = Color.Blue;
				polygon.StrokeWidth = 1f;
				polygon.FillColor = Color.FromRgba(0, 0, 255, 64);

				foreach (Location location in campus.Locations)
				{
					polygon.Positions.Add(new Position(location.Lat, location.Long));

					/*if (location.Lat > maxLat)
					{
						maxLat = location.Lat;
					}

					if (location.Long > maxLong)
					{
						maxLong = location.Long;
					}

					if (location.Lat < minLat)
					{
						minLat = location.Lat;
					}

					if (location.Long < minLong)
					{
						minLong = location.Long;
					}*/

				}

				map.Polygons.Add(polygon);
			}

			/*var mapView = new List<Position>();
			mapView.Add(new Position(maxLat, maxLong));
			mapView.Add(new Position(minLat, minLong));
			//map.MoveToRegion(MapSpan.FromPositions(mapView));*/
			
			return campuses;
		}

		public async Task AddBuildings(Map map, Campus campus, string name)
		{
			foreach (Building building in campus.Buildings)
			{
				var curr = false;

				if (name == building.Name)
				{
					curr = true;
				}

				await PlaceBuildingPin(building, map, curr);

			}
		}

		public void AddLots(Map map, Campus campus)
		{
			foreach (Lot lot in campus.Lots)
			{
				var polygon = new Polygon();
				polygon.IsClickable = true;
				polygon.StrokeWidth = 3f;

				foreach (Location pos in lot.Locations)
				{
					polygon.Positions.Add(new Position(pos.Lat, pos.Long));
				}

				if (lot.Percentage < .26)
				{
					polygon.FillColor = Color.FromRgba(0, 255, 0, 64);
					polygon.StrokeColor = Color.FromRgba(0, 255, 0, 128);

				}

				else if (lot.Percentage < .51)
				{
					polygon.FillColor = Color.FromRgba(0, 128, 0, 64);
					polygon.StrokeColor = Color.FromRgba(0, 128, 0, 128);
				}

				else if (lot.Percentage < .76)
				{
					polygon.FillColor = Color.FromRgba(128, 128, 0, 64);
					polygon.StrokeColor = Color.FromRgba(128, 128, 0, 128);

				}

				else
				{
					polygon.FillColor = Color.FromRgba(128, 0, 0, 64);
					polygon.StrokeColor = Color.FromRgba(128, 0, 0, 128);
				}

				map.Polygons.Add(polygon);
			}
		}

		/*public Lot FindClosestLot(Building building, Campus campus)
		{
			foreach(Building build in campus)
			{
				if(building.Name == build.Name)
				{
					var buildPos = buil*/

	}
}
