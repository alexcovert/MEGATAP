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
            if (hit)
            {
                child = player.GetComponentsInChildren<Renderer>();
                spellBase.Stun(player, stunDuration, turnStone, anim);
                StartCoroutine(Wait(this.gameObject));
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

            //Turn off renderer & particles
            this.GetComponent<Renderer>().enabled = false;
            ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem p in particles)
            {
                Destroy(p);
            }
        }
        if (hit == false && other.tag == "Boundary" && once == false)
        {
            StartCoroutine(WaitToDie(2f));
        }
        if(hit == false && other.tag == "Boundary" && once == true)
        {
            Destroy(this.gameObject);
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
    }

    private IEnumerator Wait(GameObject obj)
    {
        yield return new WaitForSeconds(stunDuration - 0.1f);
        Revert();
        yield return new WaitForSeconds(0.1f);
        Destroy(obj);
    }

    private IEnumerator WaitToDie(float time)
    {
        yield return new WaitForSeconds(time);
        once = true;
    }
}
