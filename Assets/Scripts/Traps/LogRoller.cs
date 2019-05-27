﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogRoller : MonoBehaviour
{
    [SerializeField] private float timeToShoot = 4;

    private TrapBase trapBase;
    private CameraTwoRotator cam;
    private CameraOneRotator cam1;
    private GameObject logPrefab;
    private float timer = 0.0f;
    private GameObject logProjectile;
    private Rigidbody rb;
    private bool once = true;
    private int face;

    private int floor;

    [SerializeField] private float speed = 40;
    //private Vector3 velocity;
    private Quaternion projectileRotation;
    // Use this for initialization

    private GameOverMenu gameOver;


    void Start()
    {
        trapBase = GetComponent<TrapBase>();
        gameOver = GameObject.Find("GameManager").GetComponent<GameOverMenu>();
        cam = GameObject.Find("Player 2 Camera").GetComponent<CameraTwoRotator>();
        cam1 = GameObject.Find("Player 1").GetComponent<CameraOneRotator>();
        logPrefab = Resources.Load("LogProjectile") as GameObject;
        face = cam.GetState();
        floor = cam.GetFloor();

        switch (cam.GetState())
        {
            case 1:
                transform.eulerAngles = new Vector3(0, 0, 0);
                projectileRotation = Quaternion.Euler(-90, 0, 0);
              //  velocity = new Vector3(-speed, 0, 0);
                break;
            case 2:
                transform.eulerAngles = new Vector3(0, 270, 0);
                projectileRotation = Quaternion.Euler(-90, -90, 0);
               // velocity = new Vector3(0, 0, -speed);
                break;
            case 3:
                transform.eulerAngles = new Vector3(0, 180, 0);
                projectileRotation = Quaternion.Euler(-90, -180, 0);
               // velocity = new Vector3(speed, 0, 0);
                break;
            case 4:
                transform.eulerAngles = new Vector3(0, 90, 0);
                projectileRotation = Quaternion.Euler(-90, -270, 0);
                //velocity = new Vector3(0, 0, speed);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (trapBase.enabled == true && !gameOver.GameOver)
        {
            CapsuleCollider col;
            if (timer > timeToShoot - 1)
            {
                if((floor == cam.GetFloor() && face == cam.GetState()) || (floor == cam1.GetFloor() && face == cam1.GetState()))
                {
                    if (once == true)
                    {
                        logProjectile = Instantiate(logPrefab);
                        col = logProjectile.GetComponentInChildren<CapsuleCollider>();

                        Physics.IgnoreCollision(col, this.GetComponent<Collider>());

                        switch (face)
                        {
                            case 1:
                                logProjectile.transform.position = transform.position + new Vector3(-0.5f, 0.4f, 0);
                                break;
                            case 2:
                                logProjectile.transform.position = transform.position + new Vector3(0, 0.4f, -0.5f);
                                break;
                            case 3:
                                logProjectile.transform.position = transform.position + new Vector3(0.5f, 0.4f, 0);
                                break;
                            case 4:
                                logProjectile.transform.position = transform.position + new Vector3(0, 0.4f, 0.5f);
                                break;
                        }
                        logProjectile.transform.rotation = projectileRotation;
                        rb = logProjectile.GetComponentInChildren<Rigidbody>();
                        rb.useGravity = false;
                        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
                        once = false;
                    }
                }
                if (timer > timeToShoot)
                {
                    if (rb != null)
                    {
                        switch (face)
                        {
                            case 1:
                                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionZ;
                                break;
                            case 2:
                                rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionX;
                                break;
                            case 3:
                                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionZ;
                                break;
                            case 4:
                                rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionX;
                                break;
                        }
                        rb.useGravity = true;
                        rb.AddForce(-transform.right * speed);
                    }
                    timer = timer - timeToShoot;
                    once = true;
                }
            }
        }
    }
}
