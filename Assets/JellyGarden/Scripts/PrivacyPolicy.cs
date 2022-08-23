using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyPolicy : MonoBehaviour {

	public void PolicyButton()
	{
		Application.OpenURL(
			"https://docs.google.com/document/d/1EVNevGFTiZEzPnPoItmyY7p6duhcEEC9iO8d0bjF-qo/edit?usp=sharing");
	}
}
