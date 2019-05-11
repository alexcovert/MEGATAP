using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSFX : MonoBehaviour
{

    [SerializeField] private AudioClip stringDrawSFX;
    [SerializeField] private AudioClip stringReleaseSFX;
    [SerializeField] private AudioClip arrowWhoosh;
    
    private AudioSource audioSource;
    private ProjectileShooter shooter;

    private CameraOneRotator cam1;

    private int floor, face;

    private void Start()
    {
        cam1 = GameObject.Find("Player 1").GetComponent<CameraOneRotator>();

        audioSource = GetComponent<AudioSource>();
        shooter = GetComponent<ProjectileShooter>();

        face = shooter.FaceNumber;
        floor = shooter.FloorNumber;

        //Debug.Log(floor + ", " + cam1.GetFloor() + ";   " + face + ", " + cam1.GetState(), this);
    }



    //Sound Effects
    //Only play if Speccy is on same face
    private void StringDraw()
    {
        if(floor == cam1.GetFloor() && face == cam1.GetState())
           audioSource.PlayOneShot(stringDrawSFX);
    }

    private void StringRelease()
    {
        if (floor == cam1.GetFloor() && face == cam1.GetState())
        {
            audioSource.PlayOneShot(stringReleaseSFX);
            audioSource.PlayOneShot(arrowWhoosh);
        }
    }
    

}
