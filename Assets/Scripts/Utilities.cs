using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour {
	public static Color ChangeColorAlpha(Color color, float a)
	{
		return new Color(color.r, color.g, color.b, a);
	}
}
