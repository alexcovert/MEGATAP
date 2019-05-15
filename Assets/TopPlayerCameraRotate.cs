using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopPlayerCameraRotate : MonoBehaviour {
	public float rotateSpeed;
	public GameObject player;
	//public GameObject movingCamera;
	// Update is called once per frame
	void Start(){
		Vector3 playerY = player.transform.position;
		//movingCamera.transform.position = new Vector3 (95.0f, playerY.y+20, 0.0f);
		transform.position = new Vector3 (0.0f, playerY.y, 0.0f);
		
	}
	void Update () {
		transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
	}
	
}
