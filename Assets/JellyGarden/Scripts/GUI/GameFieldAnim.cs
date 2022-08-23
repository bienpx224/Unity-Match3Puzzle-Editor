using UnityEngine;
using System.Collections;

public class GameFieldAnim : MonoBehaviour {

    void EndAnimGamField()
    {
       LevelManager.THIS.gameStatus = GameState.Playing;
    }
}
