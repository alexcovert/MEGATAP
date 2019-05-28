using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour {
    public int FaceNumberIfPrePlaced;
    [HideInInspector] public int FaceNumber;
    [HideInInspector] public int FloorNumber;


    [HideInInspector] public CameraTwoRotator cam2;
    private CameraOneRotator cam1;

    GameObject prefab;
    GameObject projectile;
    private Rigidbody rb;
    private BoxCollider col;

    private Vector3 velocity;
    private Quaternion projectileRotation;
    private GameOverMenu gameOver;

    private ParticleSystem[] chargeParticles;

    //private bool ghost = true;
    // Use this for initialization
    void Awake() {
        prefab = Resources.Load("projectile") as GameObject;
        cam2 = GameObject.Find("Player 2 Camera").GetComponent<CameraTwoRotator>();
        cam1 = GameObject.Find("Player 1").GetComponent<CameraOneRotator>();
        gameOver = GameObject.Find("GameManager").GetComponent<GameOverMenu>();
        if (!transform.parent.GetComponentInChildren<CheckMultipleBases>().Placed)
        {
            switch (cam2.GetState())
            {
                case 1:
                    transform.parent.eulerAngles = new Vector3(0, 0, 0);
                    projectileRotation = Quaternion.identity;
                    velocity = new Vector3(-20, 0, 0);
                    FaceNumber = 1;
                    break;
                case 2:
                    transform.parent.eulerAngles = new Vector3(0, 270, 0);
                    projectileRotation = Quaternion.Euler(0, -90, 0);
                    velocity = new Vector3(0, 0, -20);
                    FaceNumber = 2;
                    break;
                case 3:
                    transform.parent.eulerAngles = new Vector3(0, 180, 0);
                    projectileRotation = Quaternion.Euler(0, -180, 0);
                    velocity = new Vector3(20, 0, 0);
                    FaceNumber = 3;
                    break;
                case 4:
                    transform.parent.eulerAngles = new Vector3(0, 90, 0);
                    projectileRotation = Quaternion.Euler(0, -270, 0);
                    velocity = new Vector3(0, 0, 20);
                    FaceNumber = 4;
                    break;
            }

            FloorNumber = cam2.GetFloor();
        }
        else
        {
            switch (FaceNumberIfPrePlaced)
            {
                case 1:
                    transform.parent.eulerAngles = new Vector3(0, 0, 0);
                    projectileRotation = Quaternion.identity;
                    velocity = new Vector3(-20, 0, 0);
                    FloorNumber = 1;
                    FaceNumber = 1;
                    break;
                case 2:
                    transform.parent.eulerAngles = new Vector3(0, 270, 0);
                    projectileRotation = Quaternion.Euler(0, -90, 0);
                    velocity = new Vector3(0, 0, -20);
                    FloorNumber = 1;
                    FaceNumber = 2;
                    break;
                case 3:
                    transform.parent.eulerAngles = new Vector3(0, 180, 0);
                    projectileRotation = Quaternion.Euler(0, -180, 0);
                    velocity = new Vector3(20, 0, 0);
                    FloorNumber = 1;
                    FaceNumber = 3;
                    break;
                case 4:
                    transform.parent.eulerAngles = new Vector3(0, 90, 0);
                    projectileRotation = Quaternion.Euler(0, -270, 0);
                    velocity = new Vector3(0, 0, 20);
                    FloorNumber = 1;
                    FaceNumber = 4;
                    break;
            }
        }

    }

    private void Start()
    {
        chargeParticles = transform.parent.GetComponentsInChildren<ParticleSystem>();
    }

    private void Charge()
    {
        if(chargeParticles[0] != null) chargeParticles[0].Play();
    }

    private void LoadArrow()
    {
        if (!gameOver.GameOver && ((FaceNumber == cam1.GetState() && FloorNumber == cam1.GetFloor()) || (FaceNumber == cam2.GetState() && FloorNumber == cam2.GetFloor())))
        {
            projectile = Instantiate(prefab);

            projectile.transform.position = transform.parent.position + new Vector3(0, 0.75f, 0) + transform.forward * 0.5f;
            projectile.transform.rotation = projectileRotation;

            rb = projectile.GetComponent<Rigidbody>();
        }
        
    }

    private void Projectile()
    {
        if (!gameOver.GameOver && ((FaceNumber == cam1.GetState() && FloorNumber == cam1.GetFloor()) || (FaceNumber == cam2.GetState() && FloorNumber == cam2.GetFloor())))
        {
            if (projectile != null) col = projectile.GetComponent<BoxCollider>();
            if (col != null) col.enabled = true;
            if (rb != null) rb.velocity = velocity;
            if (chargeParticles[1] != null) chargeParticles[1].Play();
        }
    }
}
