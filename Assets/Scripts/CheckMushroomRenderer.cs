using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMushroomRenderer : MonoBehaviour {
    [SerializeField] private bool prePlaced;
    [SerializeField] private int faceNumber;

    private static int floor, face;
    private CameraOneRotator bottomCam;
    private CameraTwoRotator topCam;

    private MeshRenderer[] mrs;
    private SkinnedMeshRenderer[] smrs;

    private void Awake()
    {
        mrs = GetComponentsInChildren<MeshRenderer>();
        smrs = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        topCam = GameObject.Find("Player 2 Camera").GetComponent<CameraTwoRotator>();
        bottomCam = GameObject.Find("Player 1").GetComponent<CameraOneRotator>();
    }

    private void Update()
    {
        bool topCamLooking, bottomCamLooking;

        if (prePlaced)
        {
            topCamLooking = false;
            bottomCamLooking = faceNumber == bottomCam.GetState() && 1 == bottomCam.GetFloor();
        }
        else
        {
            topCamLooking = face == topCam.GetState() && floor == topCam.GetFloor();
            bottomCamLooking = face == bottomCam.GetState() && floor == bottomCam.GetFloor();
        }

        Debug.Log(face + " " + floor);

        if (topCamLooking || bottomCamLooking)
        {
            foreach (MeshRenderer mr in mrs)
            {
                if (mr != null) mr.enabled = true;
            }

            foreach (SkinnedMeshRenderer smr in smrs)
            {
                if (smr != null) smr.enabled = true;
            }
        }
        else
        {
            foreach (MeshRenderer mr in mrs)
            {
                if (mr != null) mr.enabled = false;
            }

            foreach (SkinnedMeshRenderer smr in smrs)
            {
                if (smr != null) smr.enabled = false;
            }
        }
    }

    public void SetFace(int i)
    {
        if (i % 4 == 0)
        {
            face = 4;
        }
        else
        {
            face = i % 4;
        }
    }

    public void SetFloor(int i)
    {
        floor = i;

    }
}
