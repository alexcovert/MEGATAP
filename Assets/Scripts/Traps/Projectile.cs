using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	private TrapBase trapBase;


	private bool hit = false;
	private GameObject player = null;

	// Use this for initialization
	void Start () {
		trapBase = GetComponent<TrapBase>();
	}

	void FixedUpdate(){
		Debug.Log(hit);
		Debug.Log(player);
		if (player != null)
		{
				if (hit)
				{
						trapBase.Stun2(player, 3, this.gameObject);
				}
				else
				{
						hit = false;
				}

		}
	}

	// Update is called once per frame
	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "Player"){
			player = col.gameObject;
			hit = true;
		}
		else{
			Destroy(gameObject);
		}
	}
}
