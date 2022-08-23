using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
	public List<Item> items = new List<Item> ();
	public GameObject tutorial;
	public GameObject text;
	public GameObject canvas;
	bool showed;

	void OnEnable ()
	{
		LevelManager.OnLevelLoaded += CheckNewTarget;
		LevelManager.OnStartPlay += DisableTutorial;
	}

	void OnDisable ()
	{
		LevelManager.OnLevelLoaded -= CheckNewTarget;
		LevelManager.OnStartPlay -= DisableTutorial;
	}

	void DisableTutorial ()
	{
		if (!showed && LevelManager.THIS.currentLevel == 1) {
			ChangeLayerNum (0);
			tutorial.SetActive (false);
			showed = true;
		}
	}


	void CheckNewTarget ()
	{
		if (LevelManager.THIS.currentLevel == 1 && !showed)
			StartCoroutine (WaitForCombine ());

	}

	void ShowStarsTutorial ()
	{
		//canvas.transform.position = Vector3.down * FindMaxY(items);
		tutorial.SetActive (true);
		ChangeLayerNum (4);
	}

	IEnumerator WaitForCombine ()
	{
		yield return new WaitUntil (() => AI.THIS.GetCombine () != null);
//		bool keepWaiting = true;
//		while (keepWaiting) {
//			yield return new WaitForFixedUpdate ();
//			if (AI.THIS.GetCombine () != null)
//				keepWaiting = false;
//		}
		items = AI.THIS.GetCombine ();


		if (items.Count == 0)
			yield break;
		items.Sort (SortByDistance);
		if (LevelManager.THIS.currentLevel == 1 && !showed) {
			ShowStarsTutorial ();
		}
	}

	public Vector3[] GetItemsPositions ()
	{
		Vector3[] positions = new Vector3[items.Count];
		for (int i = 0; i < items.Count; i++) {
			positions [i] = items [i].transform.position + new Vector3 (1, -1, 0);
		}
		return positions;
	}

	private int SortByDistance (Item item1, Item item2)
	{
		Item itemFirst = items [0];
		float x = Vector3.Distance (itemFirst.transform.position, item1.transform.position);
		float y = Vector3.Distance (itemFirst.transform.position, item2.transform.position);
		int retval = y.CompareTo (x);

		if (retval != 0) {
			return retval;
		} else {
			return y.CompareTo (x);
		}
	}

	public int FindMaxY (List<Item> list)
	{
		int max = int.MinValue;
		foreach (Item type in list) {
			if (type.transform.position.y > max) {
				max = (int)type.transform.position.y + 2;
			}
		}
		return max;
	}

	void ChangeLayerNum (int num)
	{
		foreach (var item in items) {
			if (item) {

				item.square.GetComponent<SpriteRenderer> ().sortingOrder = num;
				item.sprRenderer.sortingOrder = num + 2;
			}
		}
	}

}
