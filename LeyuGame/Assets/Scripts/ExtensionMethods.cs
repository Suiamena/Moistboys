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
}