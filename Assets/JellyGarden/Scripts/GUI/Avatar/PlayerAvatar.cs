using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class PlayerAvatar : MonoBehaviour, IAvatarLoader {
	public Image image;

	void Start () {
		image.enabled = false;
	}

	#if PLAYFAB || GAMESPARKS
	void OnEnable () {
		NetworkManager.OnPlayerPictureLoaded += ShowPicture;
	}

	void OnDisable () {
		NetworkManager.OnPlayerPictureLoaded -= ShowPicture;
	}


	#endif
	public void ShowPicture () {
		image.sprite = InitScript.profilePic;
		image.enabled = true;
	}

}
