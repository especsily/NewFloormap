using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface INavigation {
	void ShowNavigation(string roomID);
	void StopNavigation();

	void ShowHighlightFloor(string roomID, Color floorColor);
	void StopHighlightFloor();
}
