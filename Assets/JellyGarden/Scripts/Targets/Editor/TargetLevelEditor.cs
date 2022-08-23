using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Malee.Editor;
using UnityEditor;

namespace JellyGarden.Scripts.Targets.Editor
{
    [CustomEditor(typeof(TargetLevel))]
    public class TargetLevelEditor : UnityEditor.Editor
    {
        private int star1;
        private int Blocks;
        private TargetLevel targetLevel;
        private Ingredients[] ingr;
        private CollectItems[] collectItems;
        private int[] ingrCount;
        private Target _target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            targetLevel = (TargetLevel) target;
            if (GUILayout.Button("Load from level"))
            {
                Blocks = 0;
                
                LoadLevel(int.Parse(targetLevel.name.Replace("Level", "")));
                if (_target == Target.COLLECT)
                {
                    targetLevel.targets.Clear();
                    for (int i = 0; i < targetLevel.targets.Length; i++)
                    {
                        var el = targetLevel.targets[i];
                        if (el.type == _target) targetLevel.targets.Remove(el);
                    }
                    for (int i = 0; i < collectItems.Length; i++)
                    {
                        if (ingrCount[i] > 0)
                        {
                            var myTargetTarget = new TargetObject {type = _target};
                            myTargetTarget.icon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/JellyGarden/Textures/Items/item_0" + (int) collectItems[i] + ".png");
                            myTargetTarget.color = (int) collectItems[i] - 1;
                            myTargetTarget.targetCount = ingrCount[i];
                            targetLevel.targets.Add(myTargetTarget);
                        }
                    }
                }
                if (_target == Target.INGREDIENT)
                {
                    targetLevel.targets.Clear();
                    for (int i = 0; i < targetLevel.targets.Length; i++)
                    {
                        var el = targetLevel.targets[i];
                        if (el.type == _target) targetLevel.targets.Remove(el);
                    }
                    for (int i = 0; i < ingr.Length; i++)
                    {
                        if(ingrCount[i]>0)
                        {
                            var myTargetTarget = new TargetObject {type = _target};
                            myTargetTarget.icon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/JellyGarden/Textures/Items/ingredient_0" + (int) ingr[i] + ".png");
                            myTargetTarget.targetCount = ingrCount[i];
                            targetLevel.targets.Add(myTargetTarget);
                        }
                    }
                }
                CheckAddTarget(Target.SCORE);
                CheckAddTarget(Target.BLOCKS,_target);
                foreach (var myTargetTarget in targetLevel.targets)
                {
                    if (myTargetTarget.type == Target.SCORE)
                    {
                        myTargetTarget.targetCount = star1;
                        myTargetTarget.icon = AssetDatabase.LoadAssetAtPath<Sprite>( "Assets/JellyGarden/Textures/map/star2.png");
                    }
                    else if (myTargetTarget.type == Target.BLOCKS)
                    {
                        myTargetTarget.targetCount = Blocks;
                        myTargetTarget.icon = AssetDatabase.LoadAssetAtPath<Sprite>( "Assets/JellyGarden/Textures/Blocks/block.png");
                    }

                }
   

                EditorUtility.SetDirty(targetLevel);
            }
        }

        protected TargetObject CheckAddTarget(Target target, Target fromFileTarget = Target.SCORE)
        {
            if (targetLevel.targets.All(i => i.type != target) && target == fromFileTarget)
            {
                var targetObject = new TargetObject {type = target};
                targetLevel.targets.Add(targetObject);
                return targetObject;
            }

            if (target == fromFileTarget) return targetLevel.targets.First(i => i.type == target);
            return null;
        }

        public bool LoadLevel(int currentLevel)
        {
            //Read data from text file
            TextAsset mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
            if (mapText == null)
            {
                return false;
            }

            ProcessGameDataFromString(mapText.text);
            return true;
        }

        void ProcessGameDataFromString(string mapText)
        {
            string[] lines = mapText.Split(new string[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);

            int mapLine = 0;
            Blocks = 0;  
            _target = Target.SCORE;
            foreach (string line in lines)
            {
                if (line.StartsWith("STARS "))
                {
                    string blocksString = line.Replace("STARS", string.Empty).Trim();
                    string[] blocksNumbers = blocksString.Split(new string[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
                    star1 = int.Parse(blocksNumbers[0]);
                }
                else if (line.StartsWith("MODE "))
                {
                    string modeString = line.Replace("MODE", string.Empty).Trim();
                    _target = (Target)int.Parse(modeString);
                }
                else
                {
                    if (line.StartsWith("COLLECT COUNT "))
                    {
                        ingrCount = new int[2];
                        string blocksString = line.Replace("COLLECT COUNT", string.Empty).Trim();
                        string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < blocksNumbers.Length; i++)
                        {
                            ingrCount[i] = int.Parse(blocksNumbers[i]);
                        }
                    }
                    else if (line.StartsWith("COLLECT ITEMS "))
                    {
                        string blocksString = line.Replace("COLLECT ITEMS", string.Empty).Trim();
                        string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        ingr = new Ingredients[2];
                        collectItems = new CollectItems[2];
                        for (int i = 0; i < blocksNumbers.Length; i++)
                        {
                            if (_target == Target.INGREDIENT)
                            {
                                ingr[i] = (Ingredients)int.Parse(blocksNumbers[i]);
                            }
                            else
                            {
                                if (_target == Target.COLLECT)
                                {
                                    collectItems[i] = (CollectItems)int.Parse(blocksNumbers[i]);
                                }
                            }
                        }
                    }
                    else if(!line.StartsWith("STARS ") && !line.StartsWith("MODE ") && !line.StartsWith("SIZE ") && !line.StartsWith("LIMIT ") && !line.StartsWith("COLOR LIMIT ") &&
                            !line.StartsWith("COLLECT COUNT ") && !line.StartsWith("COLLECT ITEMS "))
                    {
                        //Maps
                        //Split lines again to get map numbers
                        string[] st = line.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < st.Length; i++)
                        {
                            if ((SquareTypes) int.Parse(st[i][0].ToString()) == SquareTypes.BLOCK)
                                Blocks++;
                            else if ((SquareTypes) int.Parse(st[i][0].ToString()) == SquareTypes.DOUBLEBLOCK)
                                Blocks+=2;
                        }

                        mapLine++;
                    }
                }
            }
        }
    }
}