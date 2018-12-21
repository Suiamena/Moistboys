using System.Collections.Generic;
using UnityEngine;

public abstract class Data<T>
{
	public enum DataPriorities { Low = 0, Medium, High };
	public abstract DataPriorities dataPriority
	{
		get;
	}
	public abstract T YieldData (T source, DataPriorities priority);
}