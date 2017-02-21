using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
//using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GMPark
{
	public partial class MapPage : ContentPage
	{
		Map map = new Map(
				MapSpan.FromCenterAndRadius(
						new Position(0, 0), Distance.FromMiles(0.3)))
		{
			IsShowingUser = true,
			HeightRequest = 100,
			WidthRequest = 960,
			VerticalOptions = LayoutOptions.FillAndExpand,
			HasZoomEnabled = true
		};

		List<Campus> campuses;

		public MapPage(string role, Building building)

		{
			InitializeComponent();
			//message.Text = "You are a" + role + ", and going to" + building "
			/*<<<<<<< HEAD
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
			>>>>>>> e9984b5... Added json parsing for full campuses*/

			AddCampuses();

			Device.BeginInvokeOnMainThread(() =>
				{
					DisplayAlert("Welcome", "To the GM Technical Center", "Okay");
				});

			Task addBuild = AddBuildings(building.Name);
			AddLots();

				/*lot.ContinueWith((Task<Lot> obj) => Device.BeginInvokeOnMainThread(() =>
				{
					DisplayAlert("Closest Lot Is", obj.Result.ID, "Okay");
				}));*/

			Task<Lot> lot = FindClosestLot(addBuild, building);

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
				FontFamily = Device.OnPlatform("AppleSDGothicNeo-UltraLight", "Droid Sans Mono", "Comic Sans MS")
			};

			var stack = new StackLayout { Spacing = 0, VerticalOptions = LayoutOptions.FillAndExpand };

			stack.Children.Add(lotLabel);
			stack.Children.Add(percentLabel);
			stack.Children.Add(map);
			stack.Children.Add(button);
			stack.BindingContext = new { LotID = "36" };

			stack.BindingContext = stack.Children[0].BindingContext;

			this.Content = stack;

			UpdateLotInStack(lot, stack);

			stack.BindingContextChanged += (sender, e) => Device.BeginInvokeOnMainThread(() =>
			{
				DisplayAlert("Closest Lot Is", "THIS WORKS", "Okay");
			});

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
		public List<Campus> AddCampuses()
		{
			var assembly = typeof(MapPage).GetTypeInfo().Assembly;
			Stream stream = assembly.GetManifestResourceStream("GMPark.campuses.json");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}

			campuses = JsonConvert.DeserializeObject<List<Campus>>(text);
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

		public async Task AddBuildings(string name)
		{
			foreach (Campus campus in campuses)
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
		}

		public void AddLots()
		{
			foreach (Campus campus in campuses)
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

					if (lot.Percentage < 26)
					{
						polygon.FillColor = Color.FromRgba(0, 255, 0, 64);
						polygon.StrokeColor = Color.FromRgba(0, 255, 0, 128);

					}

					else if (lot.Percentage < 51)
					{
						polygon.FillColor = Color.FromRgba(0, 128, 0, 64);
						polygon.StrokeColor = Color.FromRgba(0, 128, 0, 128);
					}

					else if (lot.Percentage < 76)
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
		}

		public async Task<Lot> FindClosestLot(Task addBuild, Building building)
		{
			await addBuild;
			Lot closest = null;
			double dist = 0;

			foreach (Campus campus in campuses)
			{
				foreach (Building build in campus.Buildings)
				{
					if (building.Name == build.Name)
					{
						foreach (Lot lot in campus.Lots)
						{
							foreach (Location location in lot.Locations)
							{
								if (closest == null)
								{
									closest = lot;
								}

								double currDist =
									distance(build.Position.Lat, build.Position.Long, location.Lat, location.Long, 'M');

								if (currDist < dist)
								{
									dist = currDist;
									closest = lot;
								}
							}
						}
						break;
					}
				}
			}
			return closest;
		}

		public async Task UpdateLotInStack(Task<Lot> lot, StackLayout stack)
		{
			await lot;
			stack.Children[0].BindingContext = new { LotID = lot.Result.ID };
			stack.Children[1].BindingContext = new { Percent = lot.Result.Percentage };
			
		}

		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		//:::                                                                         :::
		//:::  This routine calculates the distance between two points (given the     :::
		//:::  latitude/longitude of those points). It is being used to calculate     :::
		//:::  the distance between two locations using GeoDataSource(TM) products    :::
		//:::                                                                         :::
		//:::  Definitions:                                                           :::
		//:::    South latitudes are negative, east longitudes are positive           :::
		//:::                                                                         :::
		//:::  Passed to function:                                                    :::
		//:::    lat1, lon1 = Latitude and Longitude of point 1 (in decimal degrees)  :::
		//:::    lat2, lon2 = Latitude and Longitude of point 2 (in decimal degrees)  :::
		//:::    unit = the unit you desire for results                               :::
		//:::           where: 'M' is statute miles (default)                         :::
		//:::                  'K' is kilometers                                      :::
		//:::                  'N' is nautical miles                                  :::
		//:::                                                                         :::
		//:::  Worldwide cities and other features databases with latitude longitude  :::
		//:::  are available at http://www.geodatasource.com                          :::
		//:::                                                                         :::
		//:::  For enquiries, please contact sales@geodatasource.com                  :::
		//:::                                                                         :::
		//:::  Official Web site: http://www.geodatasource.com                        :::
		//:::                                                                         :::
		//:::           GeoDataSource.com (C) All Rights Reserved 2015                :::
		//:::                                                                         :::
		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

		private double distance(double lat1, double lon1, double lat2, double lon2, char unit)
		{
			double theta = lon1 - lon2;
			double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
			dist = Math.Acos(dist);
			dist = rad2deg(dist);
			dist = dist * 60 * 1.1515;
			if (unit == 'K')
			{
				dist = dist * 1.609344;
			}
			else if (unit == 'N')
			{
				dist = dist * 0.8684;
			}
			return (dist);
		}

		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		//::  This function converts decimal degrees to radians             :::
		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

		private double deg2rad(double deg)
		{
			return (deg * Math.PI / 180.0);
		}

		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		//::  This function converts radians to decimal degrees             :::
		//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

		private double rad2deg(double rad)
		{
			return (rad / Math.PI * 180.0);
		}
	}
}
