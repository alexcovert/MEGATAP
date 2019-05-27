using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class buttonSFX : MonoBehaviour {


	private AudioSource audioSource;
	[SerializeField] private AudioClip hoverSFX;
    [SerializeField] private AudioClip selectSFX;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Hover(){
		if(audioSource != null && hoverSFX != null) audioSource.PlayOneShot(hoverSFX);
	}

	public void Press(){
        if (audioSource != null && selectSFX != null) audioSource.PlayOneShot(selectSFX);
	}

}
