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


    [SerializeField] private Image backImg;

    [SerializeField] private AudioClip lockInSFX;
    [SerializeField] private AudioClip unlockSFX;
    [SerializeField] private AudioClip moveSFX;
    private AudioSource audioSource;

    private bool stickMove = true;

    //States can be -1 (left), 0 (middle), and 1 (right)
    private int selectorOneState = 0;
    private int selectorTwoState = 0;

    private bool p1LockIn = false;
    private bool p2LockIn = false;
    [SerializeField] private Light ollieLight;
    [SerializeField] private Light speccyLight;
    [SerializeField] private GameObject objToFollow;

    float quarterDist;

    private SceneTransition loader;

    private float returnTimer = 0;
    private float startReturnTimer = 0;

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
        SpotlightFollow();

        //Movement---------------------------------------------------------------------------------------------------
        //If only one controller is plugged in
        if (!checkControllers.GetControllerOneState())
        {
            //Keyboard
            if (Input.GetAxis("Vertical_Keyboard") < 0 && selectorOneState < 1 && stickMove && !p1LockIn)
            {
                playerOneSelector.transform.position = new Vector2(playerOnePos.x, playerOnePos.y + quarterDist);
                selectorOneState++;
                if(moveSFX != null) audioSource.PlayOneShot(moveSFX);
                stickMove = false;
                StartCoroutine(StickDelay());
            }
            if (Input.GetAxis("Vertical_Keyboard") > 0 && selectorOneState > -1 && stickMove && !p1LockIn)
            {
                playerOneSelector.transform.position = new Vector2(playerOnePos.x, playerOnePos.y - quarterDist);
                selectorOneState--;
                if (moveSFX != null) audioSource.PlayOneShot(moveSFX);
                stickMove = false;
                StartCoroutine(StickDelay());
            }

            //Mouse clicks
            if (Input.GetMouseButtonDown(0) && Input.mousePosition.y >= Screen.height / 2 && !p1LockIn)
            {
                playerOneSelector.transform.position = new Vector2(playerOnePos.x, Screen.height / 2 + quarterDist);
                if (moveSFX != null) audioSource.PlayOneShot(moveSFX);
                selectorOneState = 1;
            }
            if (Input.GetMouseButtonDown(0) && Input.mousePosition.y <= Screen.height / 2 && !p1LockIn)
            {
                playerOneSelector.transform.position = new Vector2(playerOnePos.x, Screen.height / 2 - quarterDist);
                if (moveSFX != null) audioSource.PlayOneShot(moveSFX);
                selectorOneState = -1;
            }

            //Controller 1 movement
            if (Input.GetAxis("Vertical_Joy_1_Stick") > stickSensitivity && selectorTwoState < 1 && stickMove && !p2LockIn)
            {
                playerTwoSelector.transform.position = new Vector2(playerTwoPos.x, playerTwoPos.y + quarterDist);
                selectorTwoState++;
                stickMove = false;
                if (moveSFX != null) audioSource.PlayOneShot(moveSFX);
                StartCoroutine(StickDelay());
            }
            if (Input.GetAxis("Vertical_Joy_1_Stick") < -stickSensitivity && selectorTwoState > -1 && stickMove && !p2LockIn)
            {
                playerTwoSelector.transform.position = new Vector2(playerTwoPos.x, playerTwoPos.y - quarterDist);
                selectorTwoState--;
                if (moveSFX != null) audioSource.PlayOneShot(moveSFX);
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
                if (moveSFX != null) audioSource.PlayOneShot(moveSFX);
                stickMove = false;
                StartCoroutine(StickDelay());
            }
            if (Input.GetAxis("Vertical_Joy_1_Stick") < -stickSensitivity && selectorOneState > -1 && stickMove && !p1LockIn)
            {
                playerOneSelector.transform.position = new Vector2(playerOnePos.x, playerOnePos.y - quarterDist);
                selectorOneState--;
                if (moveSFX != null) audioSource.PlayOneShot(moveSFX);
                stickMove = false;
                StartCoroutine(StickDelay());
            }

            //Controller 2 movement
            if (Input.GetAxis("Vertical_Joy_2_Stick") > stickSensitivity && selectorTwoState < 1 && stickMove && !p2LockIn)
            {
                playerTwoSelector.transform.position = new Vector2(playerTwoPos.x, playerTwoPos.y + quarterDist);
                selectorTwoState++;
                if (moveSFX != null) audioSource.PlayOneShot(moveSFX);
                stickMove = false;
                StartCoroutine(StickDelay());
            }
            if (Input.GetAxis("Vertical_Joy_2_Stick") < -stickSensitivity && selectorTwoState > -1 && stickMove && !p2LockIn)
            {
                playerTwoSelector.transform.position = new Vector2(playerTwoPos.x, playerTwoPos.y - quarterDist);
                selectorTwoState--;
                if (moveSFX != null) audioSource.PlayOneShot(moveSFX);
                stickMove = false;
                StartCoroutine(StickDelay());
            }
        }
        //End movement ---------------------------------------------------------------------------------------------------------



        LockIn();
        CheckStart();
        CheckReturn();
    }

    private void CheckReturn()
    {
        backImg.fillAmount = 1 / (1.5f / returnTimer);
        if(Input.GetButton("Cancel_Joy_1") || Input.GetButton("Cancel_Joy_2") || Input.GetButton("Escape"))
        {
            if (startReturnTimer >= 0.5f)
                returnTimer += Time.deltaTime;
            else
                startReturnTimer += Time.deltaTime;
        }
        else if(Input.GetButtonUp("Cancel_Joy_1") || Input.GetButtonUp("Cancel_Joy_2") || Input.GetButtonUp("Escape"))
        {
            returnTimer = 0;
            startReturnTimer = 0;
        }

        if(returnTimer > 1.5f)
        {
            ReturnToMenu();
            returnTimer = 0;
            startReturnTimer = 0;
        }
    }

    public void ReturnToMenu()
    {
        StartCoroutine(loader.LoadScene("Menu"));
    }
    
    private void LockIn()
    {
        //If one controller / one keyboard----------------------------------------------------------------------------------------
        if (!checkControllers.GetControllerOneState())
        {
            //Player one lock in
            if (selectorOneState != 0 && Input.GetButtonDown("Enter") && !p1LockIn)
            {
                //Don't allow them to lock in in same character
                if (!p2LockIn || selectorOneState == -selectorTwoState)
                {
                    p1LockIn = true;
                    if(lockInSFX != null) audioSource.PlayOneShot(lockInSFX);

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
            if (selectorOneState != 0 && p1LockIn && Input.GetButtonDown("Escape"))
            {
                p1LockIn = false;
                if (unlockSFX != null) audioSource.PlayOneShot(unlockSFX);

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

            //Player two lock in
            if (selectorTwoState != 0 && Input.GetButtonDown("Submit_Menu_Joy_1") && !p2LockIn)
            {
                if (!p1LockIn || selectorOneState == -selectorTwoState)
                {
                    p2LockIn = true;
                    if (lockInSFX != null) audioSource.PlayOneShot(lockInSFX);

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
            if (selectorTwoState != 0 && p2LockIn && Input.GetButtonDown("Cancel_Joy_1"))
            {
                p2LockIn = false;
                if (unlockSFX != null) audioSource.PlayOneShot(unlockSFX);

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
        else
        {
            //If both controllers plugged in----------------------------------------------------------------------------------------

            //Player one lock in
            if (selectorOneState != 0 && Input.GetButtonDown("Submit_Menu_Joy_1") && !p1LockIn)
            {
                if (!p2LockIn || selectorOneState == -selectorTwoState)
                {
                    p1LockIn = true;
                    if (lockInSFX != null) audioSource.PlayOneShot(lockInSFX);

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
                if (unlockSFX != null) audioSource.PlayOneShot(unlockSFX);

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

            //Player two lock in
            if (selectorTwoState != 0 && Input.GetButtonDown("Submit_Menu_Joy_2") && !p2LockIn)
            {
                if (!p1LockIn || selectorOneState == -selectorTwoState)
                {
                    p2LockIn = true;
                    if (lockInSFX != null) audioSource.PlayOneShot(lockInSFX);

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
                if (unlockSFX != null) audioSource.PlayOneShot(unlockSFX);

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

    }
    

    private void CheckStart()
    {
        //Check characters selected are opposite to allow scene start
        if (selectorOneState == -selectorTwoState && selectorOneState != 0 && p1LockIn && p2LockIn)
        {
            startText.text = "Press Start!";
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
                    inputManager.P1IsTop = true;
                    checkControllers.topPlayersController = true;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    inputManager.P1IsTop = false;
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
        }
        else
        {
            startText.text = "Choose Your Side!";
        }
    }
    
    private void SpotlightFollow()
    {
        speccyLight.transform.position = new Vector3(objToFollow.transform.position.x, speccyLight.transform.position.y, objToFollow.transform.position.z);
    }

    private IEnumerator StickDelay()
    {
        yield return new WaitForSeconds(stickDelay);
        stickMove = true;
    }
}
