using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {
    public Sprite[] sprites;
    public Image image;
    int i;
    public void Next()
    {
        i++;
        i = Mathf.Clamp(i, 0, sprites.Length-1);
        SetImage();
    }
    public void Back()
    {
        i--;
        i = Mathf.Clamp(i, 0, sprites.Length-1);
        SetImage();
    }

    void SetImage()
    {
        image.sprite = sprites[i];
        image.SetNativeSize();
    }
}
