using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour {

    public bool On;

	private float timeBetween;
	[SerializeField] private float startTime;

	[SerializeField] private GameObject ghost;
    [SerializeField] private GameObject crouchGhost;
    private CameraOneRotator cam;
    private PlayerOneMovement movement;
    private Quaternion projectileRotation;

	void Start () {
        On = false;
        cam = GetComponent<CameraOneRotator>();
        movement = GetComponent<PlayerOneMovement>();
	}

	void Update () {
        switch (cam.GetState())
        {
            case 1:
                projectileRotation = Quaternion.identity;
                break;
            case 2:
                projectileRotation = Quaternion.Euler(0, -90, 0);
                break;
            case 3:
                projectileRotation = Quaternion.Euler(0, -180, 0);
                break;
            case 4:
                projectileRotation = Quaternion.Euler(0, -270, 0);
                break;
        }


        if(On)
        {
            if (timeBetween < 0)
            {
                GameObject instance;
                if(!movement.crouching)
                {
                    instance = (GameObject)Instantiate(ghost, transform.position + new Vector3(0, 2.5f, 0), projectileRotation);

                }
                else
                {
                    instance = (GameObject)Instantiate(ghost, transform.position + new Vector3(0, 1f, 0), projectileRotation);
                }
                Destroy(instance, 1f);
                timeBetween = startTime;
            }
            else
            {
                timeBetween -= Time.deltaTime;
            }
        }
	}
}
