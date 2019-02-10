using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour {

	GameObject prefab;
	private float waitTime = 2.0f;
	private float timer = 0.0f;
  GameObject projectile;

	// Use this for initialization
	void Start () {
		prefab = Resources.Load("projectile") as GameObject;
	}

	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;

		if(timer > waitTime){
			timer = timer - waitTime;
			projectile = Instantiate(prefab);
			projectile.transform.position = transform.position;
			Rigidbody rb = projectile.GetComponent<Rigidbody>();
			rb.velocity = new Vector3(20, 0, 0);
		}
	}
}
