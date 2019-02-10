using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour {
	private TrapBase trapBase;
    private CameraTwoRotator cam;
	GameObject prefab;
	private float waitTime = 2.0f;
	private float timer = 0.0f;
  	GameObject projectile;
	//private bool ghost = true;
	// Use this for initialization
	void Start () {
		prefab = Resources.Load("projectile") as GameObject;
		trapBase = GetComponent<TrapBase>();
        cam = GameObject.Find("Player 2 Camera").GetComponent<CameraTwoRotator>();
	}

	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if(trapBase.enabled == true){
			if(timer > waitTime){
				projectile = Instantiate(prefab);
				projectile.transform.position = transform.position;
				Rigidbody rb = projectile.GetComponent<Rigidbody>();
                switch(cam.GetState())
                {
                    case 1:
                        rb.velocity = new Vector3(-20, 0, 0);
                        break;
                    case 2:
                        rb.velocity = new Vector3(0, 0, -20);
                        break;
                    case 3:
                        rb.velocity = new Vector3(20, 0, 0);
                        break;
                    case 4:
                        rb.velocity = new Vector3(0, 0, 20);
                        break;
                }
				timer = timer - waitTime;
			}
		}
	}
}
