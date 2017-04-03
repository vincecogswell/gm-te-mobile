﻿using System;
using System.Collections.Generic;

namespace GMPark
{
	public class Struct
	{
		private bool mActive = true;
		private string mName;
		private string mId;
		public List<Location> Entrances = new List<Location>();

		public void SetActive(bool active)
		{
			mActive = active;
		}

		public bool IsActive()
		{
			return mActive;
		}

		public void SetName(string name)
		{
			mName = name;
		}

		public string GetName()
		{
			return mName;
		}

		public string GetId()
		{
			return mId;
		}
			
		public void SetId(string id)
		{
			mId = id;
		}

		public void AddEntrance(Location loc)
		{
			Entrances.Add(loc);
		}

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

		public int GetEntranceCount()
		{
			return Entrances.Count;
		}
	}
}
