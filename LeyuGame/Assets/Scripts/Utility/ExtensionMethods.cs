using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
	public static Vector3 Multiply (this Vector3 lhs, Vector3 rhs)
	{
		Vector3 result;
		result = new Vector3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
		return result;
	}

	public static Vector2 Divide (this Vector2 lhs, Vector2 rhs)
	{
		Vector2 result = new Vector2(lhs.x / rhs.x, lhs.y / rhs.y);
		return result;
	}

	public static Vector2 Rotate (this Vector2 lhs, Quaternion rhs)
	{
		Vector3 vec = rhs * new Vector3(lhs.x, 0, lhs.y);
		return new Vector2(vec.x, vec.z);
	}

	public static float SquareDistance (this Vector3 from, Vector3 to)
	{
		float result;
		result = (from - to).sqrMagnitude;
		return result;
	}

	public static int ToInt (this bool lhs)
	{
		if (lhs)
			return 1;
		else
			return 0;
	}
}