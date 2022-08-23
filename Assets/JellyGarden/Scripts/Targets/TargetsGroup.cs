using System;
using System.Collections.Generic;
using UnityEngine;

namespace JellyGarden.Scripts.Targets
{
    public class TargetsGroup : MonoBehaviour
    {
        public TargetIcon prefab;
        private int levelNum;
        private List<TargetIcon> TargetIcons = new List<TargetIcon>();
        private void OnEnable()
        {
            levelNum = PlayerPrefs.GetInt("OpenLevel");
            var targetObj = Resources.Load<TargetLevel>("Targets/Level" + levelNum);
            foreach (var target in targetObj.targets)
            {
                var obj = Instantiate<TargetIcon>(prefab, transform);
                obj.SetTarget(target);
                TargetIcons.Add(obj);
                target.guiObj = obj;
            }
        }

        private void OnDisable()
        {
            foreach (var child in TargetIcons)
            {
                Destroy(child.gameObject);
            }
            TargetIcons.Clear();
        }
    }
}