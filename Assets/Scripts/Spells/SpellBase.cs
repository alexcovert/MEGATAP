﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SpellDirection
{
    Ceiling = 1,
    Floor = 2,
    Left = 4,
    Right = 8,
    Instant = 16
}


public class SpellBase : MonoBehaviour {
    public int LocationCast;

    [EnumFlag] [SerializeField] public SpellDirection CastDirection;
    [SerializeField] public float CooldownTime;

    [HideInInspector]
    public bool SpellCast; //Keep track of if the spell was cast - for checking instant spells (alex - post processing spells)
    //Spell speed towards player
    [SerializeField] private float speed;


    public GameObject InstantiateSpell(Vector3 position)
    {
        return Instantiate(this.gameObject, position, this.transform.rotation);
    }

    public GameObject InstantiateSpell(float posx, float posy, float posz)
    {
        return Instantiate(this.gameObject, new Vector3 (posx, posy, posz), this.transform.rotation);
    }

    // keeps track of which direction the player was moving at the moment of this save
    // be sure to call UpdatePlayerVelocities() before using these variables
    private int playerx = 0;
    private int playery = 0;
    private int playerz = 0;

    // call before using the playerx playery playerz variables
    public void UpdatePlayerVelocities(GameObject player)
    {
        // find out the velocity of the player
        if (player.gameObject.GetComponent<Rigidbody>().velocity.x > 0)
        {
            playerx = 1;
        }
        else if (player.gameObject.GetComponent<Rigidbody>().velocity.x < 0)
        {
            playerx = -1;
        }
        if (player.gameObject.GetComponent<Rigidbody>().velocity.y > 0)
        {
            playery = 1;
        }
        else if (player.gameObject.GetComponent<Rigidbody>().velocity.y < 0)
        {
            playery = -1;
        }
        if (player.gameObject.GetComponent<Rigidbody>().velocity.z > 0)
        {
            playerz = 1;
        }
        else if (player.gameObject.GetComponent<Rigidbody>().velocity.z < 0)
        {
            playerz = -1;
        }
    }

    //for stun function and its enum
    private bool once = false;

    // apply knockback to inputted
    // must be used in a FixedUpdate method, will apply velocity per frame. Use a timing
    // method to decide how many frames force is applied.
    public void KnockBack(GameObject player, int knockBackDistance, int knockUpDistance)
    {
        // find out which way the player is facing (on faces of the tower) so the knock can do accordingly
        int playerDirection = player.gameObject.GetComponent<PlayerOneMovement>().GetState();
        Rigidbody rb = player.gameObject.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * -1 * knockBackDistance);

        switch (playerDirection)
        {
            case 1:
                rb.velocity = new Vector3(-knockBackDistance * playerx, knockUpDistance * -playery, 0);
                break;
            case 2:
                rb.velocity = new Vector3(0, knockUpDistance * -playery, -knockBackDistance * playerz);
                break;
            case 3:
                rb.velocity = new Vector3(knockBackDistance * -playerx, knockUpDistance * -playery, 0);
                break;
            case 4:
                rb.velocity = new Vector3(0, knockUpDistance * -playery, knockBackDistance * playerz);
                break;
        }
    }

    // apply stun to inputted
    // goes to enumerator for its waitforseconds
    public void Stun(GameObject player, float stunDuration, Material mat = null)
    {
        Renderer[] child = player.GetComponentsInChildren<Renderer>();
        if (once == false)
        {
            once = true;
            StartCoroutine(WaitStun(player, stunDuration, mat, child));
        }

    }

    private IEnumerator WaitStun(GameObject player, float stunDuration, Material mat, Renderer[] child = null)
    {
        player.gameObject.GetComponent<PlayerOneMovement>().SetMove(false);
        player.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, player.gameObject.GetComponent<Rigidbody>().velocity.y, 0);

        float stunTime = 0;

        while (stunTime <= stunDuration)
        {
            player.gameObject.GetComponent<PlayerOneMovement>().SetMove(false);
            stunTime += Time.deltaTime;
            if (mat != null)
            {
                foreach (Renderer r in child)
                {
                    if (r.name == "Body" || r.name == "Hat" || r.name == "HatEyes" || r.name == "Poncho") r.material = mat;
                }
            }
            yield return null;
        }
        //  yield return new WaitForSeconds(stunDuration);
        player.gameObject.GetComponent<PlayerOneMovement>().SetMove(true);
    }

    // apply slow to inputted
    // input directions: both "percents" should be between 0 and 1
    public void Slow(GameObject player, float slowPercent, float jumpReductionPercent, float slowDuration)
    {
        if (once == false)
        {
            once = true;
            StartCoroutine(WaitSlow(player, slowPercent, jumpReductionPercent, slowDuration));
        }
    }

    private IEnumerator WaitSlow(GameObject player, float slowPercent, float jumpReductionPercent, float slowDuration)
    {
        float GetJumpPenalty = player.gameObject.GetComponent<PlayerOneMovement>().GetSlowJumpPenalty();

        if (jumpReductionPercent <= GetJumpPenalty)
        {
            player.gameObject.GetComponent<PlayerOneMovement>().SetSlowJumpPenalty(jumpReductionPercent);
        }

        float GetPenalty = player.gameObject.GetComponent<PlayerOneMovement>().GetSlowPenalty();
        if (slowPercent <= GetPenalty)
        {
            player.gameObject.GetComponent<PlayerOneMovement>().SetSlowPenalty(slowPercent);
        }

        player.gameObject.GetComponent<PlayerOneMovement>().SetSlowSpellTime(slowDuration);
        player.gameObject.GetComponent<PlayerOneMovement>().SetSlowTimeInitial(0);
        player.gameObject.GetComponent<PlayerOneMovement>().SetUnSlow(false);

        float slowTimePassed = 0;
        bool noSlow = false;

        while (slowTimePassed <= slowDuration)
        {
            slowTimePassed += Time.deltaTime;

            GetPenalty = player.gameObject.GetComponent<PlayerOneMovement>().GetSlowPenalty();
            if (slowPercent <= GetPenalty)
            {
                player.gameObject.GetComponent<PlayerOneMovement>().SetSlowPenalty(slowPercent);
            }

            GetJumpPenalty = player.gameObject.GetComponent<PlayerOneMovement>().GetSlowJumpPenalty();
            if (jumpReductionPercent <= GetJumpPenalty)
            {
                player.gameObject.GetComponent<PlayerOneMovement>().SetSlowJumpPenalty(jumpReductionPercent);
            }

            noSlow = player.gameObject.GetComponent<PlayerOneMovement>().GetUnSlow();

            yield return null;
        }
        if (noSlow == true && player.gameObject.GetComponent<PlayerOneMovement>().GetSlowed() == false)
        {
            player.GetComponent<PlayerOneMovement>().SetSlowJumpPenalty(1);
            player.GetComponent<PlayerOneMovement>().SetSlowPenalty(1);
        }
        else if(noSlow == true && player.gameObject.GetComponent<PlayerOneMovement>().GetSlowed() == true)
        {
            player.GetComponent<PlayerOneMovement>().SetSlowJumpPenalty(0.99f);
            player.GetComponent<PlayerOneMovement>().SetSlowPenalty(0.99f);
        }
    }

    public void RestartFace(GameObject obj)
    {
        Debug.Log("RestartFace");
    }


    //Getters for CastSpell

    public int GetLocation()
    {
        return LocationCast;
    }

    public SpellDirection GetDirection()
    {
        return CastDirection;
    }

    public float GetSpeed()
    {
        return speed;
    }
}
