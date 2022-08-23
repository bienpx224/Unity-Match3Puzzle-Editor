using UnityEngine;
using System.Collections;

public class TargetHolder : MonoBehaviour {
    public static Target target;
    public static int level;
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
