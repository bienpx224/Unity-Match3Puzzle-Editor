using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class GUIEvents : MonoBehaviour {

	void Update () {
		if (name == "FaceBook" || name == "Share" || name == "FaceBookLogout") {
			if (!LevelManager.THIS.FacebookEnable)
				gameObject.SetActive (false);
		}
	}

	public void Settings () {
		SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);

		GameObject.Find ("CanvasGlobal").transform.Find ("Settings").gameObject.SetActive (true);

	}

	public void Play () {
		SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);

		transform.Find ("Loading").gameObject.SetActive (true);
		SceneManager.LoadScene ("game");
	}

	public void Pause () {
		SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);

		if (LevelManager.THIS.gameStatus == GameState.Playing)
			GameObject.Find ("CanvasGlobal").transform.Find ("MenuPause").gameObject.SetActive (true);

	}

	public void FaceBookLogin () {
#if FACEBOOK

		FacebookManager.THIS.CallFBLogin ();
#endif
	}

	public void FaceBookLogout () {
		#if FACEBOOK
		FacebookManager.THIS.CallFBLogout ();

		#endif
	}

	public void Share () {
#if FACEBOOK

		FacebookManager.THIS.Share ();
#endif
	}

}
