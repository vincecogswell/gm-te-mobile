/* Author : Rob
 * GMTEMap class
 * Inheritent from Map class
 */
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
	/* The core class of the whole GMTE application, this object keeps track of all the objects shown on the map
	 */
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

		/* Returns a list of the roles for the name of the campus that is passed
		 * as the parameter
		 * Parameters: string (name of the campus)
		 * Returns: List<Role> (List of roles of the campuses named)
		 */
		public List<Role> GetRoles(string campusName)
		{
			foreach (Campus campus in mCampuses)
			{
				if (campus.GetName() == campusName)
				{
					return campus.GetRoles();
				}
			}
			return null;
		}

		/* Returns a list of strings containing all the campus names in the map
		 * Returns: List<string> (the name of each campus that is on the map)
		 */
		public List<string> GetCampusList()
		{
			var ls = new List<string>();

			foreach (Campus campus in mCampuses)
			{
				ls.Add(campus.GetName());
			}

			return ls;
		}

		/* Gets the ID of the campus based on the name of the campus passed into the function
		 * Returns: string (the ID of the campus that was passed in)
		 */
		public string GetCampusId(string campusName)
		{
			foreach (Campus campus in mCampuses)
			{
				if (campus.GetName() == campusName)
				{
					return campus.GetId();
				}
			}
			return null;
		}

		/* Gets a lot object based on the lotId and campusName passed into the function
		 * Parameters: string (name of campus), int (lotId)
		 * Returns: Lot (the lot object that is determined by the lotId and campusName)
		 */
		public Lot GetLotById(string campusName, int lot)
		{
			var str = Convert.ToString(lot);

			foreach (Campus campus in mCampuses)
			{
				if (campus.GetName() == campusName)
				{
					return campus.GetLotById(str);
				}
			}
			return null;
		}

		/* Gets a buiding ID based on the buildingName and campusName passed into the function
		 * Parameters: string (name of campus), string (name of building)
		 * Returns: string (the building ID that is determined by the lotId and campusName)
		 */
		public string GetBuildingId(string campusName, string buildingName)
		{
			foreach (Campus campus in mCampuses)
			{
				if (campus.GetName() == campusName)
				{
					return campus.GetBuildingId(buildingName);
				}
			}
			return null;
		}

		/* Gets a lot name based on the lotId and campusName passed into the function
		 * Parameters: string (name of campus), string (ID of lot)
		 * Returns: string (the building ID that is determined by the lotId and campusName)
		 */
		public string GetLotName(string campusName, string lotId)
		{
			foreach (Campus campus in mCampuses)
			{
				if (campus.GetName() == campusName)
				{
					return campus.GetLotName(lotId);
				}
			}
			return null;
		}

		/* Returns a list of the building names for the name of the campus that is passed
		* as the parameter
		* Parameters: string (name of the campus)
		* Returns: List<string> (List of names of buildings of the campuses named	 */
		public List<string> GetBuildingList(string campusName)
		{
			foreach (Campus campus in mCampuses)
			{
				if (campus.GetName() == campusName)
				{
					return campus.GetBuildingList();
				}
			}
			return null;
		}

		/** DO NOT USE WITH API: ONLY TO BE USED WITH TEST .JSON FILES
		 * Gets the campuses from the campuses.json file, converts them to a campus object, then creates
		 * a polygon object to visually represent it and adds it to the map.
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

		/** DO NOT USE WITH API: ONLY TO BE USED WITH TEST .JSON FILES
		* Gets the campuses from the campuses.json file, converts them to a campus object, then creates
		* a polygon object to visually represent it and adds it to the map.
		* returns: List<Campus> (all the campus objects that are created	 
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

		/** DO NOT USE WITH API: ONLY TO BE USED WITH TEST .JSON FILES
		* Create a polygon object to visually represent it and adds it to the map.
		* Parameters: List<Campus> (all the campus objects that are to be created)	
		*/
		public void AddCampuses(List<Campus> campuses)
		{
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

		/* Finds the precreated campus that is specified by the parameter, then draws all the buildings on the campus
		 * for that campus
		 * Parameters: string (the name of the campus)
		 * Returns: Task (the task of placing the pins on the map)
		 * */
		public async Task DrawBuildings(string campusName)
		{
			foreach (Campus campus in mCampuses)
			{
				if (campusName == campus.GetName())
				{
					foreach (Building building in campus.Buildings)
					{
						await PlaceBuildingPins(building);
					}
				}

			}
		}

		/* Finds the precreated campus that is specified by the parameter, then draws all the lots on the campus
		 * for that campus
		 * Parameters: string (the name of the campus)
		 * */
		public void DrawLots(string campusName)
		{
			foreach (Campus campus in mCampuses)
			{
				if (campus.GetName() == campusName)
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

						foreach (Location ent in lot.Entrances)
						{
							Pin pin = new Pin
							{
								Type = PinType.Place,
								Label = lot.GetName(),
								Position = new Position(ent.Lat, ent.Long)
							};
							pin.Icon = BitmapDescriptorFactory.FromBundle("parkingpin.png");
							Pins.Add(pin);
						}
							
						if (lot.Percentage < 26)
						{
							// Light green for selected
							polygon.FillColor = Color.FromRgba(0, 255, 0, 64);
							polygon.StrokeColor = Color.FromRgba(0, 255, 0, 128);

							// Darker Green
							//polygon.FillColor = Color.FromRgba(0, 128, 0, 64);
							//polygon.StrokeColor = Color.FromRgba(0, 128, 0, 128);
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
					break;
				}
			}
		}

		/* Finds the precreated campus that is specified by the parameter, then draws all the gates on the campus
		 * for that campus
		 * Parameters: string (the name of the campus)
		 * */
		public void DrawGates(string campusName)
		{
			foreach (Campus campus in mCampuses)
			{
				if (campus.GetName() == campusName)
				{
					foreach (Gate gate in campus.Gates)
					{
						Pin pin = new Pin
						{
							Type = PinType.Place,
							Label = gate.GetName(),
							Position = new Position(gate.GetEntrance(0).Lat, gate.GetEntrance(0).Long)
						};
						pin.Icon = BitmapDescriptorFactory.FromBundle("gatepin.png");
						Pins.Add(pin);
					}
				}
			}
		}

		/* Places the building pin on the map given the specified building object
         * Parameters: Building (the building object to be drawn)
         */
		public async Task PlaceBuildingPins(Building building)
		{
			foreach (Location loc in building.Entrances)
			{
				Pin pin = new Pin
				{
					Type = PinType.Place,
					Label = building.GetName(),
					Position = new Position(loc.Lat, loc.Long)
				};
				pin.Icon = BitmapDescriptorFactory.FromBundle("buildingpin.png");
				Pins.Add(pin);
			}
		}

		/* Used primarily during the testing stage, found and returned the closest lot to the building
		 * specified given the campus name
		 * Parameters: Task (the task created during the AddBuildings function, string (name of the 
		 * building), string (the name of the campus that the building is on)
		 * Returns: Task<Lot> (the lot object that is the closest)
		 */
		public async Task<Lot> FindClosestLot(Task addBuild, string building, string campus)
		{
			await addBuild;
			Lot closest = null;
			double dist = -1.0;

			foreach (Campus campusIs in mCampuses)
			{
				if (campusIs.GetName() == campus)
				{
					break;
				}
			}
			return closest;
		}

		/* Cycles through the CampusGeofences to determine which campus the position is in
         * Parameters: Plugin.Geolocator.Abstractions.Position (the position)
         * Returns: string (the name of the campus object that the pos is in)
         */
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

		/* Cycles through the CampusGeofences to determine if the position is in a campus geofence
		 * Parameters: Plugin.Geolocator.Abstractions.Position (the position)
		 * Returns: bool (true if in a geofence, false if not)
		 */	
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

		/* Cycles through the lot geofences in the campus to determine if the position is in a lot geofence
		* Parameters: Plugin.Geolocator.Abstractions.Position (the position), string (name of the campus)
        * Returns: bool (true if in a lot geofence, false if not)  
		*/
		public bool CheckInLotGeofences(Plugin.Geolocator.Abstractions.Position pos, string campusName)
		{
			if (campusName == "")
			{
				return false;
			}

			foreach (Campus campus in mCampuses)
			{
				if (campus.GetName() == campusName)
				{
					return campus.CheckIfInLotGeoFences(pos);
				}
			}

			return false;
		}

		/* Cycles through the lot geofencesnin the campus to determine which lot the position is in
		* Parameters: Plugin.Geolocator.Abstractions.Position (the position), string (name of the campus)
		* Returns: string (the name of the lot object that the pos is in)
		*/
		public string InWhichLot(Plugin.Geolocator.Abstractions.Position pos, string campusName)
		{
			string lotName = "";

			if (campusName == "")
			{
				return null;
			}

			foreach (Campus campus in mCampuses)
			{
				if (campus.GetName() == campusName)
				{
					lotName = campus.CheckInLotGeoFences(pos);
				}
			}

			return lotName;
			
		}

		/* Spans the map over the campus that is selected by the parameter
		 * Parameters: string (the name of the campus to be spanned over)
		 */
		public void SpanToCampus(string name)
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

		/* Spans the map over the building that is selected by the parameters
        * Parameters: string (the name of the building to be spanned over),
        * 	string (the name of the campus the building is in) 
		*/
		public async void SpanToBuilding(string buildingName, string campusName)
		{
			foreach (Campus campus in mCampuses)
			{
				if (campus.GetName() == campusName)
				{
					foreach (Building building in campus.Buildings)
					{
						if (buildingName == building.GetName())
						{
							List<Position> ls = new List<Position>();

							foreach (Location loc in building.Entrances)
							{
								ls.Add(new Position(loc.Lat, loc.Long));
							}
								       
							MoveToRegion(MapSpan.FromPositions(ls));
							break;
						}
					}
					break;
				}
			}
		}

		/* Spans the map over the buildings and lots that are selected by the parameters
        * Parameters: string (the name of the campus the building is in),
        * 	string (the name of the building that is spanned over), List<int> (the list of lotIds to span over)
		*/
		public void SpanToLotsAndBuildings(string campusName, string buildingName, List<int> lotOrder)
		{
			var ls = new List<Position>();
			int lotOrderCount = lotOrder.Count;

			if (lotOrderCount > 3)
			{
				lotOrderCount = 3;
			}

			foreach (Campus campus in mCampuses)
			{
				if (campus.GetName() == campusName)
				{
					for (int i = 0; i < lotOrderCount; i++)
					{
						foreach (Position pos in campus.GetLotPoints(lotOrder[i].ToString()))
						{
							ls.Add(pos);
						}
					}

					foreach (Position pos in campus.GetBuildingEntrances(buildingName))
					{
						ls.Add(pos);
					}
				}
			}

			MoveToRegion(MapSpan.FromPositions(ls));		
		}

		/* Calls the NavigateTo function of a lot object
		 * Parameters: string (the name of the campus that the lot is in), int (the Id of the lot to be navigated to)
		 */
		public void NavigateToLot(string campusName, int lotId)
		{
			GetLotById(campusName, lotId).NavigateTo();
		}

		/* Gets rid of all the lots in the lotOrder that are not accessable by the role name passed
		 * Parameters: string (name of the campus), string (name of the role), List<int> (list of lot Ids)
		 * Returns: List<int> (list of lots that are accessible to the role)
		 */
		public List<int> PurgeLotList(string campusName, string roleName, List<int> lotList)
		{
			foreach (Campus campus in mCampuses)
			{
				if (campus.GetName() == campusName)
				{
					var newList = campus.PurgeLotOrder(Convert.ToInt32(campus.GetRoleId(roleName)), lotList);
					return newList;
				}
			}
			return null;
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
