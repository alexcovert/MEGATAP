using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogProjectile : MonoBehaviour {
    private TrapBase trapBase;

    // custom to this trap
    [SerializeField] private int knockBackValue = 75;
    [SerializeField] private int knockUpValue = 25;
    [SerializeField] private float stunDuration = 0.75f;
    //Time before log disappears
    [SerializeField] private float lifeTime = 8f;

    [SerializeField] private float animationSpeed;

    [SerializeField] private float speedForKnockback = 0.05f;

    // let the FixedUpdate method know that there was a collision with player
    private bool hit = false;
    // the player (or whatever collided with this trap)
    private GameObject player = null;
    // keep track of how many frames of knockback have passed
    private int knockTimer = 0;
    //Player's animator for knockback animation
    private Animator anim = null;
    //Transform of log model
    private GameObject child;
    //see if log is still moving
    private Rigidbody rb;

    //Log collider
    private BoxCollider box;

    //Figure out which face this is on
    private CameraOneRotator playerOne;

    private bool canHit = true;


    private AudioSource audioSource;
    [SerializeField] private AudioClip logHitSFX;
    bool sfxPlayed = false;

    void Awake()
    {
        playerOne = GameObject.Find("Player 1").GetComponent<CameraOneRotator>();
    }

    // Use this for initialization
    void Start () {
        trapBase = GetComponent<TrapBase>();
        child = transform.parent.gameObject.transform.GetChild(1).gameObject;
        rb = child.GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(Death());
    }

    // Update is called once per frame
    private void Update()
    {
        this.transform.position = child.transform.position;

        //only needs to have 0 velocity once.
        if ((rb.velocity.x <= 0.00001 && rb.velocity.x >= -0.00001) && (rb.velocity.z <= 0.00001 && rb.velocity.z >= -0.00001))
        {
            box.enabled = false;
        }
        if (canHit == true)
        {
            switch (playerOne.GetState())
            {
                case 1:
                    if (rb.velocity.x > speedForKnockback)
                    {
                        box.center = new Vector3(0.0065f, 0, -0.0003f);
                        box.enabled = true;
                    }
                    //left
                    else if (rb.velocity.x < -speedForKnockback)
                    {
                        box.center = new Vector3(-0.0065f, 0, -0.0003f);
                        box.enabled = true;
                    }
                    else if (rb.velocity.x > -speedForKnockback && rb.velocity.x < speedForKnockback)
                    {
                        box.enabled = false;
                    }
                    break;
                case 2:
                    if (rb.velocity.z > speedForKnockback)
                    {
                        box.center = new Vector3(0.0065f, 0, -0.0003f);
                        box.enabled = true;
                    }
                    //left
                    else if (rb.velocity.z < -speedForKnockback)
                    {
                        box.center = new Vector3(-0.0065f, 0, -0.0003f);
                        box.enabled = true;
                    }
                    else if (rb.velocity.z > -speedForKnockback && rb.velocity.z < speedForKnockback)
                    {
                        box.enabled = false;
                    }
                    break;
                case 3:
                    //left
                    if (rb.velocity.x > speedForKnockback)
                    {
                        box.center = new Vector3(-0.0065f, 0, -0.0003f);
                        box.enabled = true;
                    }
                    else if (rb.velocity.x < -speedForKnockback)
                    {
                        box.center = new Vector3(0.0065f, 0, -0.0003f);
                        box.enabled = true;
                    }
                    else if (rb.velocity.x > -speedForKnockback && rb.velocity.x < speedForKnockback)
                    {
                        box.enabled = false;
                    }
                    break;
                case 4:
                    //left
                    if (rb.velocity.z > speedForKnockback)
                    {
                        box.center = new Vector3(-0.0065f, 0, -0.0003f);
                        box.enabled = true;
                    }
                    else if (rb.velocity.z < 0)
                    {
                        box.center = new Vector3(0.0065f, 0, -0.0003f);
                        box.enabled = true;
                    }
                    else if (rb.velocity.z > -speedForKnockback && rb.velocity.z < speedForKnockback)
                    {
                        box.enabled = false;
                    }
                    break;
            }
        }
    }
    void FixedUpdate () {
        
        if (player != null)
        {
            if (hit)
            {
                if (knockTimer < 7 && knockTimer >= 5)
                {
                    trapBase.KnockBack(player, knockBackValue, 0);
                    knockTimer++;
                }
                else if (knockTimer < 7)
                {
                    trapBase.KnockBack(player, 0, knockUpValue);
                    trapBase.Stun(player.gameObject, stunDuration);
                    knockTimer++;
                }
                else
                {
                    hit = false;
                    anim.SetBool("Knockback", hit);
                    knockTimer = 0;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            player = col.gameObject;
            hit = true;
            trapBase.UpdatePlayerVelocities(col.gameObject);
            anim = player.GetComponent<PlayerOneMovement>().GetAnim();
            if (logHitSFX != null && !sfxPlayed)
            {
                audioSource.PlayOneShot(logHitSFX);
                sfxPlayed = true;
            }
            if (player.GetComponent<PlayerOneMovement>().IsCrouched() == false)
            {
                anim.Play("Knockback", 0);
            }
        }
    }

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(lifeTime);
        canHit = false;
        box.enabled = false;
        yield return new WaitForSeconds(2f);
        Destroy(this.transform.parent.gameObject);
    }
    
}
