using System;
using System.Collections.Generic;
namespace GMPark
{
	public class Gate : Struct
	{
		private string mStart, mEnd;
		private Dictionary<string, string> mInstructions;

		public Gate()
		{
			mInstructions = new Dictionary<string, string>();
		}

		public void SetTime(string start, string end)
		{
			mStart = start;
			mEnd = end;
		}

		public void AddInstruction(string roleId, string instructions)
		{
			mInstructions[roleId] = instructions;
		}
	}
}
