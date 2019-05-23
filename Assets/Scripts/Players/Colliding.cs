using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colliding : MonoBehaviour {
    //This is to see if the player can stand up properly when crouching
    private bool collided = false;
	// Use this for initialization
	void Start () {
        collided = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Platform" || other.tag == "TrapHitBox")
        {
            collided = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Platform" || other.tag == "TrapHitBox")
        {
            collided = false;
        }
    }

    public bool GetCollision()
    {
        return collided;
    }
}
