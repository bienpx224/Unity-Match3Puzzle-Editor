using UnityEngine;

public class RewardedButton : MonoBehaviour
{
    public RewardedAdsType type;
    public bool typeSet;

    private void OnEnable()
    {
        if (typeSet)
        {
            CheckLimit();
        }
    }

    private void CheckLimit()
    {
        if (InitScript.Instance.RewardedReachedLimit(type)) gameObject.SetActive(false);
    }

    public void SetEnabled()
    {
        typeSet = true;
        CheckLimit();
    }
}