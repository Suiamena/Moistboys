using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerData
{
	public class Bounce : Data<Vector3>
	{
		public override DataPriorities dataPriority
		{
			get {
				return DataPriorities.Medium;
			}
		}



		public override Vector3 YieldData (Vector3 source, DataPriorities highestPriority)
		{
			if (highestPriority <= dataPriority) {
				return source + Vector3.forward;
			} else {
				return Vector3.zero;
			}
		}
	}
}