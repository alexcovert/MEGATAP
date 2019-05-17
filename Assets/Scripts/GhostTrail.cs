using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour {

	private float timeBetween;
	public float startTime;

	public GameObject ghost;
//	private Player player;

	void Start () {
		//player = GetComponent<Player>();
	}

	void Update () {
		if(timeBetween<0)
		{
			GameObject instance = (GameObject)Instantiate(ghost, transform.position, Quaternion.identity);
			Destroy(instance, 3f);
			timeBetween = startTime;
		}else
		{
			timeBetween -= Time.deltaTime;
		}
	}
}
