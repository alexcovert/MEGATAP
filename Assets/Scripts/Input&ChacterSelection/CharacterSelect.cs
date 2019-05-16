using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelect : MonoBehaviour {
    private InputManager inputManager;
    private CheckControllers checkControllers;

    [Header("Designers - Sensitivity/Delay -----")]
    [SerializeField] private float stickSensitivity;
    [SerializeField] private float stickDelay;

    [Header("Programmers - GameObjects/Script Refs -----")]
    [SerializeField] private TextMeshProUGUI startText;

    [SerializeField] private Image playerOneSelector;
    [SerializeField] private Image playerTwoSelector;
    [SerializeField] private Image topBackgroundObj;
    [SerializeField] private Image botBackgroundObj;
    [SerializeField] private Image topCharObj;
    [SerializeField] private Image bottomCharObj;


    [SerializeField] private Sprite topBackgroundColored;
    [SerializeField] private Sprite topBackgroundGrey;
    [SerializeField] private Sprite bottomBackgroundColored;
    [SerializeField] private Sprite bottomBackgroundGrey;

    [SerializeField] private Sprite topCharColored;
    [SerializeField] private Sprite topCharGrey;
    [SerializeField] private Sprite bottomCharColored;
    [SerializeField] private Sprite bottomCharGrey;

    [SerializeField] private Sprite controllerBlue;
    [SerializeField] private Sprite controllerRed;
    [SerializeField] private Sprite controllerGrey;


    [SerializeField] private AudioClip lockInSFX;
    [SerializeField] private AudioClip unlockSFX;
    private AudioSource audioSource;

    private bool stickMove = true;

    //States can be -1 (left), 0 (middle), and 1 (right)
    private int selectorOneState = 0;
    private int selectorTwoState = 0;

    private bool p1LockIn = false;
    private bool p2LockIn = false;
    [SerializeField] private Light ollieLight;
    [SerializeField] private Light speccyLight;

    float quarterDist;

    private SceneTransition loader;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        checkControllers = GameObject.Find("InputManager").GetComponent<CheckControllers>();
        loader = GetComponent<SceneTransition>();
        if(!checkControllers.GetControllerOneState())
        {
            stickDelay *= 2;
            inputManager.GetComponent<CheckControllers>().topPlayersController = false;
        }
    }

    private void Update() {
        Vector2 playerOnePos = playerOneSelector.transform.position;
        Vector2 playerTwoPos = playerTwoSelector.transform.position;
        quarterDist = Screen.height / 4;
        //ChangeColors();


        //Movement
        //If only one controller is plugged in
        if (!checkControllers.GetControllerOneState())
        {
            //Keyboard
            if (Input.GetAxis("Vertical_Keyboard") < 0 && selectorOneState < 1 && stickMove && !p1LockIn)
            {
                playerOneSelector.transform.position = new Vector2(playerOnePos.x, playerOnePos.y + quarterDist);
                selectorOneState++;
                stickMove = false;
                StartCoroutine(StickDelay());
            }
            if (Input.GetAxis("Vertical_Keyboard") > 0 && selectorOneState > -1 && stickMove && !p1LockIn)
            {
                playerOneSelector.transform.position = new Vector2(playerOnePos.x, playerOnePos.y - quarterDist);
                selectorOneState--;
                stickMove = false;
                StartCoroutine(StickDelay());
            }

            //Mouse clicks
            if (Input.GetMouseButtonDown(0) && Input.mousePosition.y >= Screen.height / 2 && !p1LockIn)
            {
                playerOneSelector.transform.position = new Vector2(playerOnePos.x, Screen.height / 2 + quarterDist);
                selectorOneState = 1;
            }
            if (Input.GetMouseButtonDown(0) && Input.mousePosition.y <= Screen.height / 2 && !p1LockIn)
            {
                playerOneSelector.transform.position = new Vector2(playerOnePos.x, Screen.height / 2 - quarterDist);
                selectorOneState = -1;
            }

            //Controller 1 movement
            if (Input.GetAxis("Vertical_Joy_1_Stick") > stickSensitivity && selectorTwoState < 1 && stickMove && !p2LockIn)
            {
                playerTwoSelector.transform.position = new Vector2(playerTwoPos.x, playerTwoPos.y + quarterDist);
                selectorTwoState++;
                stickMove = false;
                StartCoroutine(StickDelay());
            }
            if (Input.GetAxis("Vertical_Joy_1_Stick") < -stickSensitivity && selectorTwoState > -1 && stickMove && !p2LockIn)
            {
                playerTwoSelector.transform.position = new Vector2(playerTwoPos.x, playerTwoPos.y - quarterDist);
                selectorTwoState--;
                stickMove = false;
                StartCoroutine(StickDelay());
            }
        }
        //If both controllers plugged in
        else
        {
            //Controller 1 movement
            if (Input.GetAxis("Vertical_Joy_1_Stick") > stickSensitivity && selectorOneState < 1 && stickMove && !p1LockIn)
            {
                playerOneSelector.transform.position = new Vector2(playerOnePos.x, playerOnePos.y + quarterDist);
                selectorOneState++;
                stickMove = false;
                StartCoroutine(StickDelay());
            }
            if (Input.GetAxis("Vertical_Joy_1_Stick") < -stickSensitivity && selectorOneState > -1 && stickMove && !p1LockIn)
            {
                playerOneSelector.transform.position = new Vector2(playerOnePos.x, playerOnePos.y - quarterDist);
                selectorOneState--;
                stickMove = false;
                StartCoroutine(StickDelay());
            }

            //Controller 2 movement
            if (Input.GetAxis("Vertical_Joy_2_Stick") > stickSensitivity && selectorTwoState < 1 && stickMove && !p2LockIn)
            {
                playerTwoSelector.transform.position = new Vector2(playerTwoPos.x, playerTwoPos.y + quarterDist);
                selectorTwoState++;
                stickMove = false;
                StartCoroutine(StickDelay());
            }
            if (Input.GetAxis("Vertical_Joy_2_Stick") < -stickSensitivity && selectorTwoState > -1 && stickMove && !p2LockIn)
            {
                playerTwoSelector.transform.position = new Vector2(playerTwoPos.x, playerTwoPos.y - quarterDist);
                selectorTwoState--;
                stickMove = false;
                StartCoroutine(StickDelay());
            }
        }

        Debug.Log(p1LockIn + ", " + p2LockIn);

        LockIn();
        CheckStart();
    }


    
    private void LockIn()
    {
        //If one controller / one keyboard
        if(!checkControllers.GetControllerOneState())
        {
            //Player one lock in
            if (selectorOneState != 0 && Input.GetButtonDown("Enter") && !p1LockIn)
            {
                //Don't allow them to lock in in same character
                if(!p2LockIn || selectorOneState == -selectorTwoState)
                {
                    p1LockIn = true;
                    audioSource.PlayOneShot(lockInSFX);

                    //Spotlight
                    if(selectorOneState == 1)
                    {
                        ollieLight.enabled = true;
                    }
                    else
                    {
                        speccyLight.enabled = true;
                    }
                }
            }
            //Player one cancel
            if (selectorOneState != 0 && p1LockIn && Input.GetButtonDown("Escape"))
            {
                p1LockIn = false;
                audioSource.PlayOneShot(unlockSFX);

                //Spotlight
                if (selectorOneState == 1)
                {
                    ollieLight.enabled = false;
                }
                else
                {
                    speccyLight.enabled = false;
                }
            }
        }
        else
        {
            //If both controllers plugged in

            //Player one lock in
            if (selectorOneState != 0 && Input.GetButtonDown("Submit_Menu_Joy_1") && !p1LockIn)
            {
                if (!p2LockIn || selectorOneState == -selectorTwoState)
                {
                    p1LockIn = true;
                    audioSource.PlayOneShot(lockInSFX);

                    //Spotlight
                    if (selectorOneState == 1)
                    {
                        ollieLight.enabled = true;
                    }
                    else
                    {
                        speccyLight.enabled = true;
                    }
                }
            }
            //Player one cancel
            if (selectorOneState != 0 && p1LockIn && Input.GetButtonDown("Cancel_Joy_1"))
            {
                p1LockIn = false;
                audioSource.PlayOneShot(unlockSFX);

                //Spotlight
                if (selectorOneState == 1)
                {
                    ollieLight.enabled = false;
                }
                else
                {
                    speccyLight.enabled = false;
                }
            }
        }

        //Player two lock in
        if (selectorTwoState != 0 && Input.GetButtonDown("Submit_Menu_Joy_2") && !p2LockIn)
        {
            if(!p1LockIn || selectorOneState == -selectorTwoState)
            {
                p2LockIn = true;
                audioSource.PlayOneShot(lockInSFX);

                //Spotlight
                if (selectorTwoState == 1)
                {
                    ollieLight.enabled = true;
                }
                else
                {
                    speccyLight.enabled = true;
                }
            }
        }
        //Player two cancel
        if (selectorTwoState != 0 && p2LockIn && Input.GetButtonDown("Cancel_Joy_2"))
        {
            p2LockIn = false;
            audioSource.PlayOneShot(unlockSFX);

            if (selectorTwoState == 1)
            {
                ollieLight.enabled = false;
            }
            else
            {
                speccyLight.enabled = false;
            }
        }

    }
    

    private void CheckStart()
    {
        //Check characters selected are opposite to allow scene start
        if (selectorOneState == -selectorTwoState && selectorOneState != 0 && p1LockIn && p2LockIn)
        {
            startText.text = "Press Start to Start!";
            if (inputManager.GetButtonDown(InputCommand.Start) && checkControllers.GetControllerOneState())
            {
                if (selectorOneState == -1)
                {
                    inputManager.P1IsTop = false;
                }
                else
                {
                    inputManager.P1IsTop = true;
                }

                if (inputManager.TutorialSelected)
                {
                    StartCoroutine(loader.LoadScene("Tutorial"));
                }
                else
                {
                    StartCoroutine(loader.LoadScene("Tower1"));
                }
            }
            else if (inputManager.GetButtonDown(InputCommand.Start) && !checkControllers.GetControllerOneState())
            {
                if (selectorOneState == -1)
                {
                    Debug.Log("P1 is top");
                    inputManager.P1IsTop = true;
                    checkControllers.topPlayersController = true;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    Debug.Log("P1 is bottom");
                    inputManager.P1IsTop = false;
                }

                if (inputManager.TutorialSelected)
                {
                    //Initiate.Fade("Tutorial", Color.black, 2);
                    StartCoroutine(loader.LoadScene("Tutorial"));
                }
                else
                {
                    StartCoroutine(loader.LoadScene("Tower1"));
                }
            }
        }
        else
        {
            startText.text = "";
        }
    }
    

    //private void ChangeColors()
    //{
    //    //Controller images
    //    switch(selectorOneState)
    //    {
    //        case -1:
    //            playerOneSelector.sprite = controllerRed;
    //            break;
    //        case 0:
    //            playerOneSelector.sprite = controllerGrey;
    //            break;
    //        case 1:
    //            playerOneSelector.sprite = controllerBlue;
    //            break;
    //    }

    //    switch (selectorTwoState)
    //    {
    //        case -1:
    //            playerTwoSelector.sprite = controllerRed;
    //            break;
    //        case 0:
    //            playerTwoSelector.sprite = controllerGrey;
    //            break;
    //        case 1:
    //            playerTwoSelector.sprite = controllerBlue;
    //            break;
    //    }

    //    //Background & characters
    //    if(selectorOneState == 1 || selectorTwoState == 1)
    //    {
    //        topBackgroundObj.sprite = topBackgroundColored;
    //        topCharObj.sprite = topCharColored;
    //    }
    //    else
    //    {
    //        topBackgroundObj.sprite = topBackgroundGrey;
    //        topCharObj.sprite = topCharGrey;
    //    }

    //    if(selectorOneState == -1 || selectorTwoState == -1)
    //    {
    //        botBackgroundObj.sprite = bottomBackgroundColored;
    //        bottomCharObj.sprite = bottomCharColored;
    //    }
    //    else
    //    {
    //        botBackgroundObj.sprite = bottomBackgroundGrey;
    //        bottomCharObj.sprite = bottomCharGrey;
    //    }


    //}

    private IEnumerator StickDelay()
    {
        yield return new WaitForSeconds(stickDelay);
        stickMove = true;
    }
}
