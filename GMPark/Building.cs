/* Buidling class inheritant from Struct
 */
using System;
using System.Collections.Generic;

namespace GMPark
{
	public class Building : Struct
	{
		// constructor
		public Building()
		{
		}

		public Building(string name)
		{
			SetName(name);
		}

	}
}
