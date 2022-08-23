using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JellyGarden.Scripts.Editor
{
    [InitializeOnLoad]
    public static class EditorMenu
    {

        [MenuItem("Jelly Garden/Scenes/Main scene")]
        public static void MainScene()
        {
            EditorSceneManager.OpenScene("Assets/JellyGarden/Scenes/main.unity");
        }

        [MenuItem("Jelly Garden/Scenes/Game scene")]
        public static void GameScene()
        {
            EditorSceneManager.OpenScene("Assets/JellyGarden/Scenes/game.unity");
        }

        [MenuItem("Jelly Garden/Documentation/Main")]
        public static void MainDoc()
        {
            Application.OpenURL("https://www.candy-smith.com/jelly-garden-match-3-documentation");
        }

        [MenuItem("Jelly Garden/Documentation/ADS/Unity ads")]
        public static void UnityadsDoc()
        {
            Application.OpenURL("https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0/edit");
        }

        [MenuItem("Jelly Garden/Documentation/ADS/Google mobile ads(admob)")]
        public static void AdmobDoc()
        {
            Application.OpenURL("https://docs.google.com/document/d/1I69mo9yLzkg35wtbHpsQd3Ke1knC5pf7G1Wag8MdO-M/edit");
        }

        [MenuItem("Jelly Garden/Documentation/ADS/Chartboost")]
        public static void ChartboostDoc()
        {
            Application.OpenURL("https://docs.google.com/document/d/1ibnQbuxFgI4izzyUtT45WH5m1ab3R5d1E3ke3Wrb10Y/edit");
        }

        [MenuItem("Jelly Garden/Documentation/Unity IAP (in-apps)")]
        public static void Inapp()
        {
            Application.OpenURL("https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0/edit#heading=h.60xg5ccbex9m");
        }

        [MenuItem("Jelly Garden/Documentation/Leadboard/Facebook (step 1)")]
        public static void FBDoc()
        {
            Application.OpenURL("https://docs.google.com/document/d/1bTNdM3VSg8qu9nWwO7o7WeywMPhVLVl8E_O0gMIVIw0/edit?usp=sharing");
        }

        [MenuItem("Jelly Garden/Documentation/Leadboard/Gamesparks (step 2)")]
        public static void GSDoc()
        {
            Application.OpenURL("https://docs.google.com/document/d/1JcQfiiD2ALz6v_i9UIcG93INWZKC7z6FHXH_u6w9A8E/edit");
        }

    }
}