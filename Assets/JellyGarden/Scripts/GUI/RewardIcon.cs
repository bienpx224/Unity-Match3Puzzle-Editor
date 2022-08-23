using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RewardIcon : MonoBehaviour
{
    public Sprite[] sprites;
    public Image icon;
    // Use this for initialization
    void Start()
    {

    }

    public void SetIconSprite(int i)
    {
        icon.sprite = sprites[i];
    }
}
