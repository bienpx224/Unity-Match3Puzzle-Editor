using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
//using UnityEditor;

public class LevelDebugger {

    public static void SaveMap(int[] items, int maxCols, int maxRows) {
        string saveString = "";
        string filename = "levelstate.txt";


        //set map data
        for (int row = 0; row < maxRows; row++) {
            for (int col = 0; col < maxCols; col++) {
                saveString += (int)items[row * maxCols + col];
                saveString += " ";
            }
        }
        //Write to file
        string activeDir = Application.dataPath + @"/JellyGarden/Resources/";
        string newPath = System.IO.Path.Combine(activeDir, filename + ".txt");
        StreamWriter sw = new StreamWriter(newPath);
        sw.Write(saveString);
        sw.Close();
        //AssetDatabase.Refresh();

    }

    public static int[] LoadMap(int maxCols, int maxRows) {
        string filename = "levelstate.txt";
        int[] items = new int[99];
        int row = 0;
        TextAsset mapText = Resources.Load(filename) as TextAsset;
        string filetext = mapText.text;
        string[] lines = filetext.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines) {
            string[] st = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < st.Length; i++) {
                items[row * maxCols + i] = int.Parse(st[i].ToString());
            }
            row++;
        }
        return items;

    }
}
