using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PreFailed : MonoBehaviour
{
	public Sprite[] buyButtons;
	public Image buyButton;
	int FailedCost;
	// Use this for initialization
	void OnEnable ()
	{
		FailedCost = LevelManager.THIS.FailedCost;
		transform.Find ("Buy/Price").GetComponent<Text> ().text = "" + FailedCost;
		if (LevelManager.THIS.limitType == LIMIT.MOVES)
			buyButton.sprite = buyButtons [0];
		else if (LevelManager.THIS.limitType == LIMIT.TIME)
			buyButton.sprite = buyButtons [1];
		if (!LevelManager.THIS.enableInApps)
			transform.Find ("Buy").gameObject.SetActive (false);
		
		SetTargets ();
	}

	void SetTargets ()
	{
		Transform TargetCheck1 = transform.Find ("Banner/Targets/TargetCheck1");
		Transform TargetCheck2 = transform.Find ("Banner/Targets/TargetCheck2");
		Transform TargetUnCheck1 = transform.Find ("Banner/Targets/TargetUnCheck1");
		Transform TargetUnCheck2 = transform.Find ("Banner/Targets/TargetUnCheck2");
		if (LevelManager.Score < LevelManager.THIS.star1) {
			TargetCheck2.gameObject.SetActive (false);
			TargetUnCheck2.gameObject.SetActive (true);
		} else {
			TargetCheck2.gameObject.SetActive (true);
			TargetUnCheck2.gameObject.SetActive (false);
		}
		if (LevelManager.THIS.target == Target.BLOCKS) {
			if (LevelManager.THIS.TargetBlocks > 0) {
				TargetCheck1.gameObject.SetActive (false);
				TargetUnCheck1.gameObject.SetActive (true);
			} else {
				TargetCheck1.gameObject.SetActive (true);
				TargetUnCheck1.gameObject.SetActive (false);
			}
		} else if (LevelManager.THIS.target == Target.INGREDIENT || LevelManager.THIS.target == Target.COLLECT) {
			if (LevelManager.THIS.ingrCountTarget [0] > 0 || LevelManager.THIS.ingrCountTarget [1] > 0) {
				TargetCheck1.gameObject.SetActive (false);
				TargetUnCheck1.gameObject.SetActive (true);
			} else {
				TargetCheck1.gameObject.SetActive (true);
				TargetUnCheck1.gameObject.SetActive (false);
			}
		} else if (LevelManager.THIS.target == Target.SCORE) {
			if (LevelManager.Score < LevelManager.THIS.star1) {
				TargetCheck1.gameObject.SetActive (false);
				TargetUnCheck1.gameObject.SetActive (true);
			} else {
				TargetCheck1.gameObject.SetActive (true);
				TargetUnCheck1.gameObject.SetActive (false);
			}
		}


	}

}
