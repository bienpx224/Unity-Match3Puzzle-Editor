using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

[InitializeOnLoad]
public class Warning
{
    static Warning()
    {
        EditorApplication.update += RunOnce;
    }

    static void RunOnce()
    {
        //        Debug.Log(EditorSceneManager.GetSceneManagerSetup()[0].path);
        //        if (EditorSceneManager.GetSceneManagerSetup()[0].path != "Assets/WoodlandBubble/Scenes/game.unity")
        //            EditorSceneManager.OpenScene("Assets/WoodlandBubble/Scenes/game.unity");
        if (Directory.Exists("Assets/PlayServicesResolver"))
        {
            //if (!imported)
            //{

            //    AssetDatabase.ImportAsset("Assets/PlayServicesResolver");
            //#if GOOGLE_MOBILE_ADS
            //            GooglePlayServices.PlayServicesResolver.MenuResolve();
            //            EditorPrefs.SetBool("notfirsttime", true);
            //#endif

            //            Debug.Log("assets reimorted");
            //            //}
        }

        EditorApplication.update -= RunOnce;
    }
}