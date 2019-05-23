using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{

    private SpellBase spellBase;
    private GameObject player = null;
    private bool hit = false;
    private AudioSource audioSource;
    [SerializeField] private AudioClip startSFX;
    [SerializeField] private AudioClip impactSFX;

    [SerializeField] private float stunDuration = 3;

    private Animator anim;


    // Use this for initialization
    void Start()
    {
        spellBase = this.GetComponent<SpellBase>();
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(startSFX);
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
        StartCoroutine(WaitToDie(stunDuration * 1.5f));
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            // if colliding, give an amount of slow
            if (hit)
            {
                spellBase.Stun(player, stunDuration);
                anim.SetBool("Stunned", hit);
                StartCoroutine(Wait(this.gameObject));
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !hit)
        {
            hit = true;

            audioSource.PlayOneShot(impactSFX);
            player = other.gameObject;
            anim = player.GetComponent<PlayerOneMovement>().GetAnim();
            if (player.GetComponent<PlayerOneMovement>().IsCrouched() == false)
            {
                anim.Play("Stunned", 0);
            }
            StartCoroutine(WaitToDie(stunDuration * 1.5f));
        }
        if (hit == false && other.tag == "Boundary")
        {
            StartCoroutine(WaitToDie(stunDuration * 1.5f));
        }
    }

    private IEnumerator Wait(GameObject obj)
    {
        yield return new WaitForSeconds(stunDuration);
        anim.SetBool("Stunned", false);
        yield return new WaitForSeconds(stunDuration);
        Destroy(obj);
    }

    private IEnumerator WaitToDie(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }

}
