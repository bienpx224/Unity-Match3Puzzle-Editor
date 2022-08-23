using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapLevelNumber : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        //renderer.sortingLayerID = 0;
        //renderer.sortingOrder = 2;
        int num = int.Parse( transform.parent.parent.name.Replace( "Level", "" ) );
        //GetComponent<TextMesh>().text = "" + num;
        GetComponent<Text>().text = "" + num;
      //  if( num >= 10 ) transform.position += Vector3.left * 0.05f;
   //     if( num == 1 || num == 11 ) transform.position -= Vector3.right * 0.05f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
