using System;
using System.Collections;
using System.Linq;
using JellyGarden.Scripts.Targets;
using UnityEngine;

public class MapTargetIcon : MonoBehaviour
{
    private int num;
    public Sprite[] targetSprite;
    private Target tar;
    private LIMIT limitType;
    void OnEnable()
    {
        StartCoroutine(loadTarget());
    }

    IEnumerator loadTarget()
    {
        num = int.Parse(transform.parent.name.Replace("Level", ""));
        LoadLevel(num);
        var targetLevel = Resources.Load<TargetLevel>("Targets/Level" + num);
        int targetIndex;
        if (targetLevel)
        {
            if(targetLevel.targets.Length == 1)
                targetIndex = (int) targetLevel.targets.First().type;
            else targetIndex = (int) targetLevel.targets.First(i => i.type != Target.SCORE).type;
        }
        else targetIndex = (int) tar;
        yield return new WaitForSeconds(0.1f);
        if (limitType == LIMIT.TIME)
            GetComponent<SpriteRenderer>().sprite = targetSprite[4];
        else
            GetComponent<SpriteRenderer>().sprite = targetSprite[targetIndex];

    }

    void LoadLevel(int n)
    {
        TextAsset map = Resources.Load("Levels/" + n) as TextAsset;
        if (map != null)
        {
            string mapText = map.text;
            string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int mapLine = 0;
            foreach (string line in lines)
            {
                //check if line is game mode line
                if (line.StartsWith("MODE"))
                {
                    //Replace GM to get mode number, 
                    string modeString = line.Replace("MODE", string.Empty).Trim();
                    //then parse it to interger
                    tar = (Target)int.Parse(modeString);
                    //Assign game mode
                }
                else if (line.StartsWith("LIMIT"))
                {
                    string blocksString = line.Replace("LIMIT", string.Empty).Trim();
                    string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    limitType = (LIMIT)int.Parse(sizes[0]);
                }

            }
        }

    }

    void Update()
    {

    }
}
