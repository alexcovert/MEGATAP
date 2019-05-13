using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallandChain : MonoBehaviour {

    private SpellBase spellBase;

    [SerializeField] private float slowRun = 0.5f;
    [SerializeField] private float reduceJump = 0.5f;
    [SerializeField] private float spellDuration = 5f;

    // let the FixedUpdate method know that there was a collision
    private bool hit = false;
    // the player (or whatever collided with this trap)
    private GameObject player = null;

    //Hit two boundaries to die
    private bool once = false;

    private MeshRenderer[] slowEffect = new MeshRenderer[2];

    // SFX
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip clip;

    private void Start()
    {
        spellBase = GetComponent<SpellBase>();
        audioSource = GetComponent<AudioSource>();
        switch(GameObject.Find("Player 1").GetComponent<CameraOneRotator>().GetState())
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
    // Update is called once per frame
    // knockback has a knockback velocity, knockup velocity, and a knockTimer to
    // force the knockback into an arc shape.
    void FixedUpdate()
    {
        if (player != null)
        {
            // if colliding, give an amount of stun
            if (hit)
            {
                spellBase.Slow(player, slowRun, reduceJump, spellDuration);
                StartCoroutine(Wait(this.gameObject));
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            hit = true;
            player = other.gameObject;
            MeshRenderer[] mrs = player.GetComponentsInChildren<MeshRenderer>();
            int slowEffectCount = 0;
            foreach(MeshRenderer mr in mrs)
            { 
                if (mr.name == "SlowEffect")
                {
                    slowEffect[slowEffectCount] = mr;
                    slowEffectCount++;
                }
            }
            
            foreach(MeshRenderer e in slowEffect)
            {
                if(e != null)
                {
                    e.enabled = true;
                }
            }
            StartCoroutine(DisableSlowEffect());
            this.GetComponent<Renderer>().enabled = false;
            audioSource.PlayOneShot(clip);
        }
      
        if (hit == false && other.tag == "Boundary" && once == false)
        {
            StartCoroutine(WaitToDie(2f));
        }
        if (hit == false && other.tag == "Boundary" && once == true)
        {
            StartCoroutine(DestroyObj());
        }
    }

    private IEnumerator Wait(GameObject obj)
    {
        yield return new WaitForSeconds(spellDuration);
        yield return new WaitForSeconds(0.1f);
        Destroy(obj);
    }

    private IEnumerator WaitToDie(float time)
    {
        yield return new WaitForSeconds(time);
        once = true;
    }

    private IEnumerator DestroyObj()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }

    private IEnumerator DisableSlowEffect()
    {
        yield return new WaitForSeconds(spellDuration);
        foreach (MeshRenderer e in slowEffect)
        {
            if (e != null)
            {
                e.enabled = false;
            }
        }
    }
}
