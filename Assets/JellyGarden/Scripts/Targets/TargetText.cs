using System;
using UnityEngine;
using UnityEngine.UI;

namespace JellyGarden.Scripts.Targets
{
    public class TargetText : MonoBehaviour
    {
        public Text textObject;
        public Action<string> updateText;
        public delegate string del();
        public del TextUpdate;
        private void Update()
        {
            if (TextUpdate != null) textObject.text = TextUpdate();
        }
    }
}