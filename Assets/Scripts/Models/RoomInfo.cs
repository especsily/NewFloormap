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
	public Image border;

	public void MouseClick()
	{
		DOTween.CompleteAll();
		UImanager.ClickRoomInfo(id);
	}

	public void MouseOver()
	{
		border.gameObject.SetActive(true);
	}

	public void MouseExit()
	{
		border.gameObject.SetActive(false);
	}
}
