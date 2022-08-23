using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

public class EventsListener : MonoBehaviour {

    void OnEnable() {
        LevelManager.OnMapState += OnMapState;
        LevelManager.OnEnterGame += OnEnterGame;
        LevelManager.OnLevelLoaded += OnLevelLoaded;
        LevelManager.OnMenuPlay += OnMenuPlay;
        LevelManager.OnMenuComplete += OnMenuComplete;
        LevelManager.OnStartPlay += OnStartPlay;
        LevelManager.OnWin += OnWin;
        LevelManager.OnLose += OnLose;

    }

    void OnDisable() {
        LevelManager.OnMapState -= OnMapState;
        LevelManager.OnEnterGame -= OnEnterGame;
        LevelManager.OnLevelLoaded -= OnLevelLoaded;
        LevelManager.OnMenuPlay -= OnMenuPlay;
        LevelManager.OnMenuComplete -= OnMenuComplete;
        LevelManager.OnStartPlay -= OnStartPlay;
        LevelManager.OnWin -= OnWin;
        LevelManager.OnLose -= OnLose;

    }

    #region GAME_EVENTS
    void OnMapState() {
    }
    void OnEnterGame() {
        AnalyticsEvent("OnEnterGame", LevelManager.THIS.currentLevel);
    }
    void OnLevelLoaded() {
    }
    void OnMenuPlay() {
    }
    void OnMenuComplete() {
    }
    void OnStartPlay() {
    }
    void OnWin() {
        AnalyticsEvent("OnWin", LevelManager.THIS.currentLevel);
    }
    void OnLose() {
        AnalyticsEvent("OnLose", LevelManager.THIS.currentLevel);
    }

    #endregion

    void AnalyticsEvent(string _event, int level) {
#if UNITY_ANALYTICS
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add(_event, level);
        Analytics.CustomEvent(_event, dic);

#endif
    }


}
