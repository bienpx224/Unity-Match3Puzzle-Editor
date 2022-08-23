﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvatarManager : MonoBehaviour {
	public List<GameObject> avatars = new List<GameObject> ();

	void OnEnable () {//1.3.3
		#if PLAYFAB || GAMESPARKS
		NetworkManager.OnFriendsOnMapLoaded += CheckFriendsList;

#endif
	}

	void OnDisable () {//1.3.3
		#if PLAYFAB || GAMESPARKS
		NetworkManager.OnFriendsOnMapLoaded -= CheckFriendsList;
		#endif
	}

	void CheckFriendsList () {
		List<FriendData> Friends = FacebookManager.Friends;

		for (int i = 0; i < Friends.Count; i++) {
			CreateAvatar (Friends [i]);
		}
	}

	/// <summary>
	/// Creates the friend's avatar.
	/// </summary>
	void CreateAvatar (FriendData friendData) {
		GameObject friendAvatar = friendData.avatar;
		if (friendAvatar == null) {
			friendAvatar = Instantiate (Resources.Load ("Prefabs/FriendAvatar")) as GameObject;
			avatars.Add (friendAvatar);
			friendData.avatar = friendAvatar;
			friendAvatar.transform.SetParent (transform);
		}
		friendAvatar.GetComponent<FriendAvatar> ().FriendData = friendData;
	}

}
