/* Struct class
 * Lots and buildings derived from this
 */
using System;
using Xamarin.Forms.GoogleMaps;
using System.Collections.Generic;

namespace GMPark
{
	public class Struct
	{
		private bool mActive = true;
		private string mName;
		private string mId;
		public List<Location> Entrances = new List<Location>();

		/* Setter for mActive
		 * Parameters: bool (sets mActive)
		 */
		public void SetActive(bool active)
		{
			mActive = active;
		}

		/* Getter for mActive
		 * Returns: bool (gets mActive)
		 */
		public bool IsActive()
		{
			return mActive;
		}

		/* Setter for mName
		 * Parameters: string (sets mName)
		 */
		public void SetName(string name)
		{
			mName = name;
		}

		/* Getter for mName
		 * Returns: string (gets mName)
		 */
		public string GetName()
		{
			return mName;
		}

		/* Getter for mId
		 * Returns: string (gets mId)
		 */
		public string GetId()
		{
			return mId;
		}
			
		/* Setter for mId
		 * Parameters: string (sets mId)
		 */	
		public void SetId(string id)
		{
			mId = id;
		}

		/* Adds an entrance
		 * Parameters: Location (entrance to be added)
		 */
		public void AddEntrance(Location loc)
		{
			Entrances.Add(loc);
		}

		/* Gets an entrance
		 * Parameters: int (index of entrance to be gotten)
		 * Returns: Location (entrance to be added)
		 */
		public Location GetEntrance(int index)
		{
			if (Entrances.Count > index)
			{
				return Entrances[index];
			}

			else
			{
				return null;
			}
		}

		/* Gets the entrances
		 * Returns: List<Position> (entrances)
		 */
		public List<Position> GetEntrances()
		{
			var ls = new List<Position>();
			foreach(Location loc in Entrances)
			{
				ls.Add(new Position(loc.Lat, loc.Long));
			}

			return ls;
		}

		/* Counts the amount of entrances in the struct
		 * Returns: int (number of entrances)
		 */
		public int GetEntranceCount()
		{
			return Entrances.Count;
		}
	}
}
