using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Petrify : MonoBehaviour {

    private SpellBase spellBase;
    [SerializeField] private float stunDuration;
    //Change boy's material when hit and turn back
    [SerializeField] private Material normalBody;
    [SerializeField] private Material normalHat;
    [SerializeField] private Material normalHatEyes;
    [SerializeField] private Material normalPoncho;
    [SerializeField] private Material turnStone;

    //Hit two boundaries to die
    private bool once = false;

    private Renderer[] child;

    // let the FixedUpdate method know that there was a collision
    private bool hit = false;
    // the player (or whatever collided with this trap)
    private GameObject player = null;

    private Animator anim;

	 private AudioSource audioSource;
    [SerializeField] private AudioClip cast;
    [SerializeField] private AudioClip clip;


    private void Start()
    {
        spellBase = GetComponent<SpellBase>();
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(cast);
        switch (GameObject.Find("Player 1").GetComponent<CameraOneRotator>().GetState())
        {
            case 1:
                break;
            case 2:
                transform.eulerAngles = new Vector3(0, -90, 0);
                break;
            case 3:
                transform.eulerAngles = new Vector3(0, 180, 0);
                break;
            case 4:
                transform.eulerAngles = new Vector3(0, 90, 0);
                break;

        }
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            if (player.gameObject.GetComponent<PlayerOneMovement>().IsStunned() == false)
            {
                Revert();
                anim.enabled = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
        	audioSource.PlayOneShot(clip);
            hit = true;
            player = other.gameObject;
            anim = player.gameObject.GetComponent<PlayerOneMovement>().GetAnim();
            anim.enabled = false;

            //Turn off renderer & particles
            this.GetComponent<Renderer>().enabled = false;
            ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem p in particles)
            {
                Destroy(p);
            }
            Destroy(this.gameObject, 8);
            child = player.GetComponentsInChildren<Renderer>();
            StartCoroutine(CheckPetrifyStatus());
            spellBase.Stun(player, stunDuration, turnStone);
            StartCoroutine(Wait(this.gameObject));
        }
        if (hit == false && other.tag == "Boundary" && once == false)
        {
            StartCoroutine(WaitToDie(2f));
        }
        if(hit == false && other.tag == "Boundary" && once == true)
        {
            StartCoroutine(Die(3f));
        }
         
    }

    private void Revert()
    {
        foreach (Renderer r in child)
        {
            if (r.name == "Body")
            {
                r.material = normalBody;
            }
            if (r.name == "Hat")
            {
                r.material = normalHat;
            }
            if (r.name == "HatEyes")
            {
                r.material = normalHatEyes;
            }
            if (r.name == "Poncho")
            {
                r.material = normalPoncho;
            }
        }
        anim.enabled = true;
    }

    private IEnumerator Wait(GameObject obj)
    {
        float time = 0;
        while (time <= stunDuration + 0.2f)
        {
            if (player.gameObject.GetComponent<PlayerOneMovement>().GetUnPetrify() == true)
            {
                Revert();
                anim.enabled = true;
            }
            yield return null;
        }
        yield return new WaitForSeconds(8f);
        Destroy(this.gameObject);
    }

    private IEnumerator WaitToDie(float time)
    {
        yield return new WaitForSeconds(time);
        once = true;
    }

    private IEnumerator Die(float time)
    {
        yield return new WaitForSeconds(time * 4);
        Destroy(this.gameObject);
    }

    private IEnumerator CheckPetrifyStatus()
    {
        //For petrify's materials to stop flickering
        player.gameObject.GetComponent<PlayerOneMovement>().SetPetrifyTime(stunDuration);
        player.gameObject.GetComponent<PlayerOneMovement>().SetStunTimeInitial(0);
        player.gameObject.GetComponent<PlayerOneMovement>().SetUnPetrify(false);

        yield return null;
    }
}
