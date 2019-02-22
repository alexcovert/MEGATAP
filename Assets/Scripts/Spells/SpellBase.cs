using System.Collections;
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
    private bool waitActive = true;
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
    public void Stun(GameObject player, float stunDuration)
    {
        if (once == false)
        {
            once = true;
            StartCoroutine(WaitStun(player, stunDuration));
        }

    }

    private IEnumerator WaitStun(GameObject player, float stunDuration)
    {
        waitActive = true;
        player.gameObject.GetComponent<PlayerOneMovement>().SetSpeed(0);
        player.gameObject.GetComponent<PlayerOneMovement>().SetMove(false);
        player.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, player.gameObject.GetComponent<Rigidbody>().velocity.y, 0);
        yield return new WaitForSeconds(stunDuration);
        waitActive = false;
        if (waitActive == false)
        {
            player.gameObject.GetComponent<PlayerOneMovement>().SetMove(true);
            player.GetComponent<PlayerOneMovement>().SetSpeed(player.GetComponent<PlayerOneMovement>().GetConstantSpeed());
            //once = false;
            waitActive = true;
        }
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
        player.gameObject.GetComponent<PlayerOneMovement>().SetJumpHeight(player.gameObject.GetComponent<PlayerOneMovement>().GetJumpHeight() * jumpReductionPercent);
        player.gameObject.GetComponent<PlayerOneMovement>().SetSpeed(player.gameObject.GetComponent<PlayerOneMovement>().GetSpeed() * slowPercent);

        yield return new WaitForSeconds(slowDuration);

        player.GetComponent<PlayerOneMovement>().SetJumpHeight(player.GetComponent<PlayerOneMovement>().GetConstantJumpHeight());
        player.GetComponent<PlayerOneMovement>().SetSpeed(player.GetComponent<PlayerOneMovement>().GetConstantSpeed());
    }

    // apply knockback to inputted
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
