using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckRenderers : MonoBehaviour {
    [SerializeField] private bool prePlaced;
    [SerializeField] private int faceNumber;

    private int floor, face;
    private CameraOneRotator bottomCam;
    private CameraTwoRotator topCam;

    private MeshRenderer[] mrs;
    private SkinnedMeshRenderer[] smrs;

    private bool on, prevOn;
    private void Awake()
    {
        topCam = GameObject.Find("Player 2 Camera").GetComponent<CameraTwoRotator>();
        bottomCam = GameObject.Find("Player 1").GetComponent<CameraOneRotator>();

        mrs = GetComponentsInChildren<MeshRenderer>();
        smrs = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        face = topCam.GetState();
        floor = topCam.GetFloor();
    }

    private void Update()
    {
        bool topCamLooking, bottomCamLooking;

        if(prePlaced)
        {
            topCamLooking = false;
            bottomCamLooking = faceNumber == bottomCam.GetState() && 1 == bottomCam.GetFloor();
        }
        else
        {
            topCamLooking = face == topCam.GetState() && floor == topCam.GetFloor();
            bottomCamLooking = face == bottomCam.GetState() && floor == bottomCam.GetFloor();
        }

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
}
