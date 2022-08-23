using System;
using UnityEngine;

namespace JellyGarden.Scripts.GUI
{
    public class KeepUIInScreen : MonoBehaviour
    {
        private void OnEnable()
        {
            RectTransform wordRT = GetComponent<RectTransform>();
 
            Vector2 wordScreenPosition = Camera.main.WorldToScreenPoint(wordRT.position);
            float wordWidth = wordRT.rect.width;
            float wordLeftPos = wordScreenPosition.x - (wordWidth / 2f);
            float wordRightPos = wordScreenPosition.x + (wordWidth / 2f);
 
            if (wordLeftPos < 0)
            {
                wordScreenPosition.x = wordScreenPosition.x + ((wordWidth / 2f) - wordScreenPosition.x);
 
                Vector2 wordWorldPos = Camera.main.ScreenToWorldPoint(wordScreenPosition);
                wordRT.position = wordWorldPos;
            }
            else if (wordRightPos > Camera.main.pixelWidth)
            {
                float deltaPosX = Camera.main.pixelWidth - wordScreenPosition.x;
                float diff = (wordWidth / 3f) - deltaPosX;
                wordScreenPosition.x = wordScreenPosition.x - diff;
               
                Vector2 wordWorldPos = Camera.main.ScreenToWorldPoint(wordScreenPosition);
                wordRT.position = wordWorldPos;
            }
        }
    }
}