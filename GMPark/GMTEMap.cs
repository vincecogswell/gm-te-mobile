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
	public class GMTEMap : Map
	{
		List<GeoPoly> mCampusGeofences = new List<GeoPoly>();
		List<Campus> mCampuses = new List<Campus>();

		public GMTEMap()
		{
		}

		public GMTEMap(MapSpan span)
		{
			MoveToRegion(span);
		}

		/** Gets the campuses from the campuses.json file, converts them to a campus object, then creates
		 * a polygon object to visually represent it and adds it to the map.
		 * Map map map that campuses are to be added to.
		 */
		public Campus AddCampus(string name)
		{
			var assembly = typeof(MapPage).GetTypeInfo().Assembly;
			Stream stream = assembly.GetManifestResourceStream("GMPark.campuses.json");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}

			List<Campus> campuses = JsonConvert.DeserializeObject<List<Campus>>(text);

			foreach (Campus campus in campuses)
			{
				if (name == campus.GetName())
				{
					var polygon = new Polygon();
					polygon.IsClickable = true;
					polygon.StrokeColor = Color.Blue;
					polygon.StrokeWidth = 1f;
					polygon.FillColor = Color.FromRgba(0, 0, 255, 64);
					var campusGeofence = new GeoPoly(campus.GetName());

					for (int i = 0; i < campus.Locations.Count(); i++)
					{
						polygon.Positions.Add(new Position(campus.Locations[i].Lat, campus.Locations[i].Long));

						int i2 = (i + 1) % campus.Locations.Count();
						campusGeofence.AddGeoLine(campus.Locations.ElementAt(i), campus.Locations.ElementAt(i2));
					}

					Polygons.Add(polygon);
					mCampusGeofences.Add(campusGeofence);
					mCampuses.Add(campus);
					return campus;
				}
			}

			return null;
		}

		public List<Campus> AddCampuses()
		{
			var assembly = typeof(MapPage).GetTypeInfo().Assembly;
			Stream stream = assembly.GetManifestResourceStream("GMPark.campuses.json");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}

			List<Campus> campuses = JsonConvert.DeserializeObject<List<Campus>>(text);

			foreach (Campus campus in campuses)
			{
				var polygon = new Polygon();
				polygon.IsClickable = true;
				polygon.StrokeColor = Color.Blue;
				polygon.StrokeWidth = 1f;
				polygon.FillColor = Color.FromRgba(0, 0, 255, 64);
				var campusGeofence = new GeoPoly(campus.GetName());

				for (int i = 0; i < campus.Locations.Count(); i++)
				{
					polygon.Positions.Add(new Position(campus.Locations[i].Lat, campus.Locations[i].Long));

					int i2 = (i + 1) % campus.Locations.Count();
					campusGeofence.AddGeoLine(campus.Locations.ElementAt(i), campus.Locations.ElementAt(i2));
				}

				Polygons.Add(polygon);
				mCampusGeofences.Add(campusGeofence);
				mCampuses.Add(campus);
			}

			return campuses;
		}

		public void AddCampuses(List<Campus> campuses)
		{
			/*var assembly = typeof(MapPage).GetTypeInfo().Assembly;
			Stream stream = assembly.GetManifestResourceStream("GMPark.campuses.json");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}

			List<Campus> campuses = JsonConvert.DeserializeObject<List<Campus>>(text);*/

			foreach (Campus campus in campuses)
			{
				var polygon = new Polygon();
				polygon.IsClickable = true;
				polygon.StrokeColor = Color.Blue;
				polygon.StrokeWidth = 1f;
				polygon.FillColor = Color.FromRgba(0, 0, 255, 64);
				var campusGeofence = new GeoPoly(campus.GetName());

				for (int i = 0; i < campus.Locations.Count(); i++)
				{
					polygon.Positions.Add(new Position(campus.Locations[i].Lat, campus.Locations[i].Long));

					int i2 = (i + 1) % campus.Locations.Count();
					campusGeofence.AddGeoLine(campus.Locations.ElementAt(i), campus.Locations.ElementAt(i2));
				}

				Polygons.Add(polygon);
				mCampusGeofences.Add(campusGeofence);
				mCampuses.Add(campus);
			}
		}

		public async Task AddBuildings(string name, Campus campus)
		{
			foreach (Building building in campus.Buildings)
			{
				var curr = false;

				if (name == building.Name)
				{
					curr = true;
				}
				await PlaceBuildingPin(building, this, curr);

			}
		}

		public void AddLots(Campus campus)
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

				Polygons.Add(polygon);
			}
		}

		public async Task PlaceBuildingPin(Building building, Map map, bool current)
		{
			var geocoder = new Xamarin.Forms.GoogleMaps.Geocoder();
			var positions = await geocoder.GetPositionsForAddressAsync(building.Address);

			if (positions.Count() > 0)
			{
				if (building.Position == null)
				{
					building.Position = new Location()
					{
						Lat = positions.First().Latitude,
						Long = positions.First().Longitude
					};
				}

				Pin pin = new Pin
				{
					Type = PinType.Place,
					Label = building.Name,
					Address = building.Address,
					Position = new Position(building.Position.Lat, building.Position.Long)
				};

				if (current)
				{
					map.MoveToRegion(MapSpan.FromCenterAndRadius(pin.Position, Distance.FromMeters(200)));
				}

				map.Pins.Add(pin);
			}
		}

		public async Task<Lot> FindClosestLot(Task addBuild, Building building, Campus campus)
		{
			await addBuild;
			Lot closest = null;
			double dist = -1.0;

			foreach (Building build in campus.Buildings)
			{
				if (building.Name == build.Name)
				{
					foreach (Lot lot in campus.Lots)
					{
						foreach (Location location in lot.Locations)
						{
							double currDist =
								distance(build.Position.Lat, build.Position.Long, location.Lat, location.Long, 'M');

							if (dist < 0)
							{
								dist = currDist;
								closest = lot;
							}

							else if (currDist < dist)
							{
								dist = currDist;
								closest = lot;
							}
						}
					}
					break;
				}
			}
			return closest;
		}

		public string InWhichGeofences(Plugin.Geolocator.Abstractions.Position pos)
		{
			foreach (GeoPoly fence in mCampusGeofences)
			{
				if (fence.InFence(pos))
				{
					return fence.getName();
				}

			}

			return "";
		}

		public bool CheckInGeofences(Plugin.Geolocator.Abstractions.Position pos)
		{
			foreach (GeoPoly fence in mCampusGeofences)
			{
				if (fence.InFence(pos))
				{
					return true;
				}

			}

			return false;
		}

		public async void SpanToCampus(string name)
		{
			foreach (Campus campus in mCampuses)
			{
				if (name == campus.GetName())
				{
					double avgLat = 0, avgLong = 0, minLat = 90, minLong = 180, maxLat = -180, maxLong = -90, count = campus.Locations.Count();
					foreach (Location loc in campus.Locations)
					{
						avgLat += loc.Lat;
						avgLong += loc.Long;

						if (loc.Lat < minLat)
						{
							minLat = loc.Lat;
						}

						if (loc.Lat > maxLat)
						{
							maxLat = loc.Lat;
						}

						if (loc.Long < minLong)
						{
							minLong = loc.Long;
						}

						if (loc.Long > maxLong)
						{
							maxLong = loc.Long;
						}
					}

					MoveToRegion(new MapSpan(new Position(avgLat / count, avgLong / count), maxLat - minLat, maxLong - minLong));
				}
			}
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
