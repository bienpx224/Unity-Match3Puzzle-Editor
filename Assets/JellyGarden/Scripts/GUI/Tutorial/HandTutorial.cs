using UnityEngine;
using System.Collections;

public class HandTutorial : MonoBehaviour
{
	public TutorialManager tutorialManager;

	void OnEnable ()
	{
		PrepareAnimateHand ();
	}

	void PrepareAnimateHand ()
	{
		Vector3[] positions = tutorialManager.GetItemsPositions ();
		StartCoroutine (AnimateHand (positions));
	}

	IEnumerator AnimateHand (Vector3[] positions)
	{
		float speed = 1;
		int posNum = 0;

//		for (int i = 0; i < 2; i++) {
		int i = 0;
		if (AI.THIS.combineType == CombineType.VShape)
			i = 1;
		transform.position = AI.THIS.tipItem.transform.position;
		posNum++;
		Vector3 offset = new Vector3 (0.5f, -1f);
		Vector2 startPos = transform.position + offset;
		Vector2 endPos = transform.position + AI.THIS.vDirection + offset;
		float distance = Vector3.Distance (startPos, endPos);
		float fracJourney = 0;
		float startTime = Time.time;

		while (fracJourney < 1) {
			float distCovered = (Time.time - startTime) * speed;
			fracJourney = distCovered / distance;
			transform.position = Vector2.Lerp (startPos, endPos, fracJourney);
			yield return new WaitForFixedUpdate ();
		}
//		}
		yield return new WaitForFixedUpdate ();
		PrepareAnimateHand ();
	}
}
