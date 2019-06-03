﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags]
public enum Location
{
    FreeFloat = 1, //Doesn't need to be attached to a surface
    Ceiling = 2,
    Floor = 4,
    LeftWall = 8,
    RightWall = 16,

    AnyWall = LeftWall | RightWall,
    AnySurface = LeftWall | RightWall | Floor | Ceiling,
    FloorAndCeiling = Floor | Ceiling
}

// a base class for traps to build on
public class TrapBase : MonoBehaviour {
    [EnumFlag] [SerializeField]
    public Location ValidLocations;


    public GameObject InstantiateTrap(Vector3 position)
    {
        return Instantiate(this.gameObject, position, this.transform.rotation);
    }

    public GameObject InstantiateTrap(Vector3 position, Quaternion rotation)
    {
        return Instantiate(this.gameObject, position, rotation);
    }

    // keeps track of which direction the player was moving at the moment of this save
    // be sure to call UpdatePlayerVelocities() before using these variables
    private int playerx = 0;
    private int playery = 0;
    private int playerz = 0;

    // call before using the playerx playery playerz variables
    public void UpdatePlayerVelocities( GameObject obj )
    {
        // find out the velocity of the player
        if (obj.gameObject.GetComponent<Rigidbody>().velocity.x > 0)
        {
            playerx = 1;
        }
        else if (obj.gameObject.GetComponent<Rigidbody>().velocity.x < 0)
        {
            playerx = -1;
        }
        if (obj.gameObject.GetComponent<Rigidbody>().velocity.y > 0)
        {
            playery = 1;
        }
        else if (obj.gameObject.GetComponent<Rigidbody>().velocity.y <= 0)
        {
            playery = -1;
        }
        if (obj.gameObject.GetComponent<Rigidbody>().velocity.z > 0)
        {
            playerz = 1;
        }
        else if (obj.gameObject.GetComponent<Rigidbody>().velocity.z < 0)
        {
            playerz = -1;
        }
    }


    //for stun function and its enum
    //private bool waitActive = true;
    private bool once = false;

    // apply knockback to inputted
    // must be used in a FixedUpdate method, will apply velocity per frame. Use a timing
    // method to decide how many frames force is applied.
    public void KnockBack(GameObject obj, int knockBackDistance, int knockUpDistance)
    {
        // find out which way the player is facing (on faces of the tower) so the knock can do accordingly
        int playerDirection = obj.gameObject.GetComponent<PlayerOneMovement>().GetState();
        Rigidbody rb = obj.gameObject.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * -1 * knockBackDistance);

        switch (playerDirection)
        {
            case 1:
                if(playerx == 0)
                {
                    playerx = 1;
                }
                rb.velocity = new Vector3(knockBackDistance * -playerx, knockUpDistance * -playery, 0);
                break;
            case 2:
                if (playerz == 0)
                {
                    playerz = 1;
                }
                rb.velocity = new Vector3(0, knockUpDistance * -playery, knockBackDistance * -playerz);
                break;
            case 3:
                if (playerx == 0)
                {
                    playerx = -1;
                }
                rb.velocity = new Vector3(knockBackDistance * -playerx, knockUpDistance * -playery, 0);
                break;
            case 4:
                if (playerz == 0)
                {
                    playerz = -1;
                }
                rb.velocity = new Vector3(0, knockUpDistance * -playery, knockBackDistance * -playerz);
                break;
        }
    }

    // apply stun to inputted
    // goes to enumerator for its waitforseconds
    // pass in the trap GameObject itself if you want it to be destroyed after stun runs
    // do not pass in the trap GameObject if you want the stun to not destroy your trap.
    // once boolean is so couroutine only runs once, otherwise player might get stuck in trap and it loops infinitely
    public void Stun(GameObject obj, float stunDuration, GameObject trap = null)
    {

        if (once == false)
        {
            once = true;
            StartCoroutine(Wait(obj, stunDuration, trap));
        }
    }

    private IEnumerator Wait(GameObject obj, float stunDuration, GameObject trap = null)
    {
        obj.gameObject.GetComponent<PlayerOneMovement>().SetMove(false);
        obj.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, obj.gameObject.GetComponent<Rigidbody>().velocity.y, 0);

        float stunTimePassed = 0;
        while (stunTimePassed <= stunDuration)
        {
            obj.gameObject.GetComponent<PlayerOneMovement>().SetMove(false);

            stunTimePassed += Time.deltaTime;

            yield return null;
        }

        obj.gameObject.GetComponent<PlayerOneMovement>().SetMove(true);

        once = false;

        if (trap != null)
        {
            Destroy(trap);
        }
    }

    // apply knockback to inputted
    // input directions: both "percents" should be between 0 and 1
    public void Slow(GameObject obj, float slowPercent, float jumpReductionPercent)
    {
        float GetJumpPenalty = obj.gameObject.GetComponent<PlayerOneMovement>().GetSlowJumpPenalty();
        if (jumpReductionPercent <= GetJumpPenalty)
        {
            obj.gameObject.GetComponent<PlayerOneMovement>().SetSlowJumpPenalty(jumpReductionPercent);
        }
        float GetPenalty = obj.gameObject.GetComponent<PlayerOneMovement>().GetSlowPenalty();
        if (slowPercent <= GetPenalty)
        {
            obj.gameObject.GetComponent<PlayerOneMovement>().SetSlowPenalty(slowPercent);
        }
    }

    // apply knockback to inputted
    public void RestartFace(GameObject obj)
    {
        Debug.Log("RestartFace");
    }
}
