using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoomInfo : MonoBehaviour {
	public IAnimationUI UImanager;
	public string id;
	public Text orderText;
	public Text infoText;

	public void MouseClick()
	{
		DOTween.CompleteAll();
		UImanager.ClickRoomInfo(id);
	}
}
