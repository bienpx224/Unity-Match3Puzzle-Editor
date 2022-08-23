using UnityEngine;
using System.Collections;

public class ItemAnimEvents : MonoBehaviour {


    public Item item;

    public void SetAnimationDestroyingFinished()
    {
        item.SetAnimationDestroyingFinished();
    }
}
