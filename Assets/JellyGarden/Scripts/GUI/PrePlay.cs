using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrePlay : MonoBehaviour {
    public GameObject ingrObject;
    public GameObject blocksObject;
    public GameObject scoreTargetObject;

	// Use this for initialization
	void OnEnable () {
        InitTargets();
	}

    void InitTargets()
    {
        blocksObject.SetActive(false);
        ingrObject.SetActive(false);
        scoreTargetObject.SetActive(false);
        GameObject ingr1 = ingrObject.transform.Find("Ingr1").gameObject;
        GameObject ingr2 = ingrObject.transform.Find("Ingr2").gameObject;

        ingr1.SetActive(true);
        ingr2.SetActive(true);
        ingr1.GetComponent<RectTransform>().localPosition = new Vector3(-74.37f, ingr1.GetComponent<RectTransform>().localPosition.y, ingr1.GetComponent<RectTransform>().localPosition.z);
        ingr2.GetComponent<RectTransform>().localPosition = new Vector3(50.1f, ingr2.GetComponent<RectTransform>().localPosition.y, ingr2.GetComponent<RectTransform>().localPosition.z);

        if (LevelManager.THIS.ingrCountTarget[0] == 0 && LevelManager.THIS.ingrCountTarget[1] == 0) ingrObject.SetActive(false);
        else if (LevelManager.THIS.ingrCountTarget[0] > 0 || LevelManager.THIS.ingrCountTarget[1] > 0)
        {
            blocksObject.SetActive(false);
            ingrObject.SetActive(true);
            ingr1 = ingrObject.transform.Find("Ingr1").gameObject;
            ingr2 = ingrObject.transform.Find("Ingr2").gameObject;
            if (LevelManager.THIS.target == Target.INGREDIENT)
            {
                if (LevelManager.THIS.ingrCountTarget[0] > 0 && LevelManager.THIS.ingrCountTarget[1] > 0 && LevelManager.THIS.ingrTarget[0] == LevelManager.THIS.ingrTarget[1])
                {
                    LevelManager.THIS.ingrCountTarget[0] += LevelManager.THIS.ingrCountTarget[1];
                    LevelManager.THIS.ingrCountTarget[1] = 0;
                    LevelManager.THIS.ingrTarget[1] = Ingredients.None;
                }
                ingr1.GetComponent<Image>().sprite = LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.ingrTarget[0]];
                ingr2.GetComponent<Image>().sprite = LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.ingrTarget[1]];
            }
            else if (LevelManager.THIS.target == Target.COLLECT)
            {
                if (LevelManager.THIS.ingrCountTarget[0] > 0 && LevelManager.THIS.ingrCountTarget[1] > 0 && LevelManager.THIS.collectItems[0] == LevelManager.THIS.collectItems[1])
                {
                    LevelManager.THIS.ingrCountTarget[0] += LevelManager.THIS.ingrCountTarget[1];
                    LevelManager.THIS.ingrCountTarget[1] = 0;
                    LevelManager.THIS.collectItems[1] = CollectItems.None;
                }
                ingr1.GetComponent<Image>().sprite = LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.collectItems[0] + 2];
                ingr2.GetComponent<Image>().sprite = LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.collectItems[1]+2];

            }
            if (LevelManager.THIS.ingrCountTarget[0] == 0 && LevelManager.THIS.ingrCountTarget[1] > 0)
            {
                ingr1.SetActive(false);
                ingr2.GetComponent<RectTransform>().localPosition = new Vector3(0, ingr2.GetComponent<RectTransform>().localPosition.y, ingr2.GetComponent<RectTransform>().localPosition.z);
            }
            else if (LevelManager.THIS.ingrCountTarget[0] > 0 && LevelManager.THIS.ingrCountTarget[1] == 0)
            {
                ingr2.SetActive(false);
                ingr1.GetComponent<RectTransform>().localPosition = new Vector3(0, ingr1.GetComponent<RectTransform>().localPosition.y, ingr1.GetComponent<RectTransform>().localPosition.z);
            }
        }
        if (LevelManager.THIS.targetBlocks > 0)
        {
            blocksObject.SetActive(true);
        }
        else if (LevelManager.THIS.ingrCountTarget[0] == 0 && LevelManager.THIS.ingrCountTarget[1] == 0)
        {
            ingrObject.SetActive(false);
            blocksObject.SetActive(false);
            scoreTargetObject.SetActive(true);
        }
    }
}
