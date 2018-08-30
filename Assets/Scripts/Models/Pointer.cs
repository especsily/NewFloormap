using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour {
	public SpriteRenderer arrowSprite;
	public TrailRenderer trail;

	public void TurnOffMesh()
	{
		arrowSprite.enabled = false;
		StartCoroutine(WaitThenDestroy(trail.time));
	}

	IEnumerator WaitThenDestroy(float duration)
    {
        yield return new WaitForSeconds(duration);
		trail.Clear();
		gameObject.SetActive(false);
        // Destroy(this.gameObject);
    }
}
