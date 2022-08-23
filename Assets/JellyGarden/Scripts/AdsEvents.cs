using UnityEngine;
using System.Collections;

public enum AdType
{
	AdmobInterstitial,
	ChartboostInterstitial,
	UnityAdsVideo
}

[System.Serializable]
public class AdEvents
{
	public GameState gameEvent;
	public AdType adType;
	public int everyLevel;
	//1.6
	public int calls;

}