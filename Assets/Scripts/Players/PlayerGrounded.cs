using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrounded : MonoBehaviour {

    private bool grounded = false;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "TrapHitbox" || collision.gameObject.tag == "Projectile")
        {
            grounded = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "TrapHitbox" || collision.gameObject.tag == "Projectile")
        {
            grounded = false;
        }
    }

    public bool IsGrounded()
    {
        return grounded;
    }
}
