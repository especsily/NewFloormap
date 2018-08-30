using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BeginArrow : MonoBehaviour {
	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>

	private Color color;
	[SerializeField] private float animTime;
	[SerializeField] private float scale;
	void Start()
	{
		color = GetComponent<SpriteRenderer>().color;
		var sequence = DOTween.Sequence();
		sequence.Append(transform.DOMoveX(21.5f, animTime).SetEase(Ease.Linear));	
		sequence.Join(transform.DOScale(new Vector2(scale, scale), animTime).SetEase(Ease.Linear));
		sequence.Join(GetComponent<SpriteRenderer>().DOColor(new Color(color.r, color.g, color.b, 100f/255f), animTime));
		sequence.SetLoops(-1, LoopType.Restart);
		sequence.Play();
	}
}
