using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButtonController : MonoBehaviour {

    private CheckControllers cc;

    // Bottom Player Worldspace UI images
    [Header("Bottom Player UI Images")]
    [SerializeField] GameObject controllerA1;
    [SerializeField] GameObject keyboardSpace1;
    [SerializeField] GameObject controllerA2;
    [SerializeField] GameObject controllerA3;
    [SerializeField] GameObject controllerA4;
    [SerializeField] GameObject keyboardSpace2;
    [SerializeField] GameObject keyboardSpace3;
    [SerializeField] GameObject controllerX;
    [SerializeField] GameObject keyboardControl1;
    [SerializeField] GameObject controllerY;
    [SerializeField] GameObject keyboardShift1;

    // Top Player UI images
    [Header("Top Plyer UI Images")]
    [SerializeField] GameObject tControllerA1;
    [SerializeField] GameObject tControllerA2;
    [SerializeField] GameObject tControllerA3;
    [SerializeField] GameObject tControllerBumpers;
    [SerializeField] GameObject tControllerStick;
    [SerializeField] GameObject tControllerDPad;
    [SerializeField] GameObject tControllerY1;
    [SerializeField] GameObject tControllerY2;
    [SerializeField] GameObject tControllerA4;
    [SerializeField] GameObject tKeyboardMouse1;
    [SerializeField] GameObject tKeyboardMouse2;
    [SerializeField] GameObject tKeyboardMouse3;
    [SerializeField] GameObject tKeyboardMouse4;
    [SerializeField] GameObject tKeyboardMouse5;
    [SerializeField] GameObject tKeyboardEnter1;
    [SerializeField] GameObject tKeyboardEnter2;
    [SerializeField] GameObject tKeyboardMouse6;

    // Use this for initialization
    void Start () {
        cc = GameObject.Find("InputManager").GetComponent<CheckControllers>();
        // if bottom player is using a controller
        if (cc.GetBottomPlayerControllerState())
        {
            controllerA1.SetActive(true);
            keyboardSpace1.SetActive(false);
            controllerA2.SetActive(true);
            controllerA3.SetActive(true);
            controllerA4.SetActive(true);
            keyboardSpace2.SetActive(false);
            keyboardSpace3.SetActive(false);
            controllerX.SetActive(true);
            keyboardControl1.SetActive(false);
            controllerY.SetActive(true);
            keyboardShift1.SetActive(false);

            tControllerA1.SetActive(true);
            tKeyboardMouse1.SetActive(false);
        } else // if they are not
        {
            controllerA1.SetActive(false);
            keyboardSpace1.SetActive(true);
            controllerA2.SetActive(false);
            controllerA3.SetActive(false);
            controllerA4.SetActive(false);
            keyboardSpace2.SetActive(true);
            keyboardSpace3.SetActive(true);
            controllerX.SetActive(false);
            keyboardControl1.SetActive(true);
            controllerY.SetActive(false);
            keyboardShift1.SetActive(true);

            tControllerA1.SetActive(false);
            tKeyboardMouse1.SetActive(true);
        }

        // if top player is using a controller
        if (cc.topPlayersController)
        {

            tControllerA2.SetActive(true);
            tControllerA3.SetActive(true);
            tControllerBumpers.SetActive(true);
            tControllerStick.SetActive(true);
            tControllerDPad.SetActive(true);
            tControllerY1.SetActive(true);
            tControllerY2.SetActive(true);
            tControllerA4.SetActive(true);
            tKeyboardMouse2.SetActive(false);
            tKeyboardMouse3.SetActive(false);
            tKeyboardMouse4.SetActive(false);
            tKeyboardMouse5.SetActive(false);
            tKeyboardEnter1.SetActive(false);
            tKeyboardEnter2.SetActive(false);
            tKeyboardMouse6.SetActive(false);
        } else // if not using a controller
        {

            tControllerA2.SetActive(false);
            tControllerA3.SetActive(false);
            tControllerBumpers.SetActive(false);
            tControllerStick.SetActive(false);
            tControllerDPad.SetActive(false);
            tControllerY1.SetActive(false);
            tControllerY2.SetActive(false);
            tControllerA4.SetActive(false);
            tKeyboardMouse2.SetActive(true);
            tKeyboardMouse3.SetActive(true);
            tKeyboardMouse4.SetActive(true);
            tKeyboardMouse5.SetActive(true);
            tKeyboardEnter1.SetActive(true);
            tKeyboardEnter2.SetActive(true);
            tKeyboardMouse6.SetActive(true);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
