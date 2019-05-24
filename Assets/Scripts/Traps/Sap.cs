using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sap : MonoBehaviour {

    private TrapBase trapBase;

    // let the FixedUpdate method know that there was a collision
    private bool hit = false;
    // be sure to only slow the player once, not every frame
    private bool slowTriggered = false;
    // the player (or whatever collided with this trap)
    private GameObject player = null;
    // keep track of how many frames of knockback have passed
    private int slowTimer = 0;
    // Player's animator for animation
    private Animator anim = null;
    //private float tempSpeed = 15; // here for score reasons, will be changed during runtime in slow loop

    [Tooltip("Defines how much slower the player will go.")][SerializeField] private float slowSeverity = 0.25f;
    [Tooltip("Defines how much lower the jump will go." )][SerializeField] private float jumpReduceSeverity = 0.5f;
    [Tooltip("Defines how long the slow will last after being activated. (In number of frames)")] [SerializeField] private int slowDuration = 60;

    private void Start()
    {
        trapBase = GetComponent<TrapBase>();
        slowTimer = slowDuration;
    }
    // Update is called once per frame
    // knockback has a knockback velocity, knockup velocity, and a knockTimer to 
    // force the knockback into an arc shape.
    void FixedUpdate()
    {
        if(player != null && hit == true)
        {
            // if colliding, give an amount of slow
            if (slowTimer > 0 && slowTriggered == false)
            {
                trapBase.Slow(player, slowSeverity, jumpReduceSeverity);
                slowTriggered = true;
            }
            else if (slowTriggered)
            {
                player.GetComponent<PlayerOneMovement>().SetSlowJumpPenalty(1);
                slowTriggered = false;
            }
            if(slowTimer <= 0)
            {
                if (hit == true)
                {
                    player.GetComponent<PlayerOneMovement>().SetSlowed(false);
                    if (player.GetComponent<PlayerOneMovement>().GetUnSlow() == true)
                    {
                        player.GetComponent<PlayerOneMovement>().SetSlowPenalty(1);
                        player.GetComponent<PlayerOneMovement>().SetSlowJumpPenalty(1);
                    }
                    else
                    {
                        player.GetComponent<PlayerOneMovement>().SetSlowPenalty(0.99f);
                        player.GetComponent<PlayerOneMovement>().SetSlowJumpPenalty(0.99f);
                    }
                    anim.SetBool("Slowed", false);
                }
                hit = false;

            }
            // tick timer down if there is any
            if (slowTimer > 0)
            {
                slowTimer--;
            }
        }

    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            hit = true;
            player = other.gameObject;
            //Player animation goes to idle properly in sap.
            anim = player.GetComponent<Animator>();
            if (player.GetComponent<PlayerOneMovement>().GetInputAxis() != 0)
            {
                if (player.GetComponent<PlayerOneMovement>().IsCrouched() == false && player.GetComponent<PlayerOneMovement>().IsStunned() == false && player.GetComponentInChildren<PlayerGrounded>().IsGrounded() == true)
                {
                    anim.Play("Trudging", 0);
                }
                anim.SetBool("Slowed", hit);
            }
            player.GetComponent<PlayerOneMovement>().SetSlowed(true);
            slowTimer = slowDuration;
        }
    }
}
