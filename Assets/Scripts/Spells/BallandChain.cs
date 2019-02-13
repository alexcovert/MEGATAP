﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallandChain : MonoBehaviour {

    private SpellBase spellBase;

    // let the FixedUpdate method know that there was a collision
    private bool hit = false;
    // the player (or whatever collided with this trap)
    private GameObject player = null;


    private void Start()
    {
        spellBase = GetComponent<SpellBase>();
    }
    // Update is called once per frame
    // knockback has a knockback velocity, knockup velocity, and a knockTimer to 
    // force the knockback into an arc shape.
    void FixedUpdate()
    {
        if (player != null)
        {
            // if colliding, give an amount of slow
            if (hit)
            {
                spellBase.Slow(player, 0.5f, 0.5f, 5f);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            hit = true;
            player = other.gameObject;
            this.GetComponent<Renderer>().enabled = false;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Boundary"))
        {
            Destroy(this);
        }
    }
}