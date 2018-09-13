using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour {
	public static Color ChangeColorAlpha(Color color, float a)
	{
		return new Color(color.r, color.g, color.b, a);
	}

	public static Color ChangeHighlightColor(Color color)
	{
		return new Color(color.r + 20f/255f, color.g + 20f/255f, color.b + 20f/255f, color.a);
	}

	public static Vector3 CameraMoveToSide(Vector3 currentPos)
	{
		return new Vector3(currentPos.x + 5, 0, currentPos.z -5);
	}

	public static Vector3 CameraMoveBackToCurrent(Vector3 sidePos)
	{
		return new Vector3(sidePos.x - 5, 0, sidePos.z +5);
	}
}
