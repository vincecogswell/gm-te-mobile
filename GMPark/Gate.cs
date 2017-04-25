/* Author : Rob
 * Gate class
 */
using System;
using System.Collections.Generic;
namespace GMPark
{
	public class Gate : Struct
	{
		// each gate has opening hours and instructions
		private string mStart, mEnd;
		private Dictionary<string, string> mInstructions;

		public Gate()
		{
			mInstructions = new Dictionary<string, string>();
		}

		// set gate accessing time
		public void SetTime(string start, string end)
		{
			mStart = start;
			mEnd = end;
		}

		// add insturction
		public void AddInstruction(string roleId, string instructions)
		{
			mInstructions[roleId] = instructions;
		}
	}
}
