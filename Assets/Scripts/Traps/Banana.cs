using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour {
    private TrapBase trapBase;
    [SerializeField] private float stunDuration;
    [SerializeField] private float stunAnimSpeed = 1f;

    private Animator thisAnim;

    // the player (or whatever collided with this trap)
    private GameObject player = null;
    // Player's animator for animation
    private Animator anim = null;
    //trap's collider
    private BoxCollider box;

    // SFX
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip clip;

    private void Start()
    {
        trapBase = GetComponent<TrapBase>();
        audioSource = GetComponent<AudioSource>();
        thisAnim = GetComponentInChildren<Animator>();
        box = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject;
            anim = player.GetComponent<PlayerOneMovement>().GetAnim();
            thisAnim.SetTrigger("Collide");
            anim.SetFloat("StunAnimSpeed", stunAnimSpeed);
            anim.Play("FaceplantStart", 0);
            box.enabled = false;
            audioSource.PlayOneShot(clip);
            trapBase.Stun(player, stunDuration, this.gameObject);
        }
    }
}
