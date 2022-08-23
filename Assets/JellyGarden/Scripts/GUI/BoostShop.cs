using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public enum BoostType
{
    ExtraMoves,
    Packages,
    Stripes,
    ExtraTime,
    Bomb,
    Colorful_bomb,
    Hand,
    Random_color,
    None
}

public class BoostShop : MonoBehaviour {
    public Sprite[] icons;
    public string[] descriptions;
    public int[] prices;
    public Image icon;
    public Text description;

    BoostType boostType;

    public List<BoostProduct> boostProducts = new List<BoostProduct>();

	// Use this for initialization
	void Start () {
	
	}

    void Update()
    {
    }
	
	// Update is called once per frame
	public void SetBoost (BoostType _boostType ) 
    {
        boostType = _boostType;
        gameObject.SetActive( true );
        icon.sprite = boostProducts[(int)_boostType].icon;
        description.text = boostProducts[(int)_boostType].description;
        for (int i = 0; i < 3; i++)
        {
            transform.Find("Image/BuyBoost" + (i + 1) + "/Count").GetComponent<Text>().text = "x" + boostProducts[(int)_boostType].count[i];
            transform.Find("Image/BuyBoost" + (i + 1) + "/Price").GetComponent<Text>().text = "" + boostProducts[(int)_boostType].GemPrices[i];
        }
	}

    public void BuyBoost(GameObject button)
    {
        int count = int.Parse( button.transform.Find("Count").GetComponent<Text>().text.Replace("x",""));
        int price = int.Parse( button.transform.Find("Price").GetComponent<Text>().text);
        GetComponent<AnimationManager>().BuyBoost(boostType, price, count);
    }
}

[System.Serializable]
public class BoostProduct
{
    public Sprite icon;
    public string description;
    public int[] count;
    public int[] GemPrices;
}