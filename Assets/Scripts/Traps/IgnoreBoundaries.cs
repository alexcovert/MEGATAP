using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreBoundaries : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Physics.IgnoreLayerCollision(2, 19);
	}
	
	// Update is called once per frame
	void Update () {
		
    }
}
