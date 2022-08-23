using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MovesLabel : MonoBehaviour {
	public Sprite[] sprites;
	// Use this for initialization
	void OnEnable () {
		LevelManager.OnLevelLoaded += Reset;
	}

	void OnDisable () {//2.1.2
		LevelManager.OnLevelLoaded -= Reset;
	}


	void Reset () {//2.1.2
		if (LevelManager.THIS != null) {
			if (LevelManager.THIS.limitType == LIMIT.MOVES)
				GetComponent<Image> ().sprite = sprites [0];
			else
				GetComponent<Image> ().sprite = sprites [1];
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
