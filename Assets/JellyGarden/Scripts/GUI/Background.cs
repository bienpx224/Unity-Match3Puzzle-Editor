using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
	public Sprite[] pictures;
	[Header("How often to change background per levels")]
	public int changeBackgoundEveryLevels = 20 ; //2.2.2
	// Use this for initialization
	void OnEnable ()
	{
		if (LevelManager.THIS != null)
			GetComponent<Image> ().sprite = pictures [Mathf.Clamp( (int)((float)LevelManager.Instance.currentLevel / (float)changeBackgoundEveryLevels - 0.01f),0, pictures.Length)];//2.2.2

//			GetComponent<Image> ().sprite = pictures [(int)((float)LevelManager.Instance.currentLevel / 20f - 0.01f)];


	}


}
