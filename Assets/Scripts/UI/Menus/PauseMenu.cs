using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour {
	[HideInInspector] public bool GameIsPaused = false;
	
	[SerializeField] GameObject pauseMenuUI;
    [SerializeField] Button[] pauseButtons;
    [SerializeField] GameObject controlsCanvas;
    [SerializeField] GameObject topController;
    [SerializeField] GameObject bottomController;
    [SerializeField] GameObject topKeyboard;
    [SerializeField] GameObject bottomKeyboard;
    [SerializeField] Button resumeButton;
    [SerializeField] GameObject playerTwo;
    [SerializeField] GameObject playerOne;
    [SerializeField] EventSystem es;


    private PlaceTrap pt;
    private CastSpell cs;
    private PlayerOneMovement playerMov;
    private PlayerOneLose speccyLose;
    private WinTrigger speccyWin;
    //For changing controls images
    private CheckControllers cc;
    private BeginGo countdown;

    //When we pause/resume we need to set the trap/spell buttons uninteractable, then interactable again
    //these bool arrays keep track of which ones are used/on cooldown so we don't set them interactable on resume
    private bool[] onCooldown;
    private bool[] usedTraps;

    private bool controlsUp;
    private GameObject selectedButton;

    CursorLockMode currentLockMode;
    bool cursorVisible;

    private int controllerThatPaused = -1; //0 = keyboard, 1 = controller 1, 2 = controller 2; -1 = not paused

    private void Start()
    {
        pt = playerTwo.GetComponent<PlaceTrap>();
        cs = playerTwo.GetComponent<CastSpell>();
        playerMov = playerOne.GetComponent<PlayerOneMovement>();

        //Get input refs
        GameObject inputManager = GameObject.Find("InputManager");
        //input = inputManager.GetComponent<InputManager>();
        cc = inputManager.GetComponent<CheckControllers>();

        GameObject gameManager = GameObject.Find("GameManager");
        countdown = gameManager.GetComponent<BeginGo>();

        speccyLose = playerOne.GetComponent<PlayerOneLose>();
        speccyWin = GameObject.Find("WinTrigger").GetComponent<WinTrigger>();
    }

    // Update is called once per frame
    void Update () {
        if(!GameIsPaused)
        {
            if (Input.GetButtonDown("Escape") && countdown.CountdownFinished && !speccyLose.Lose && !speccyWin.Win && (!cc.topPlayersController || !cc.GetBottomPlayerControllerState()))
            {
                controllerThatPaused = 0;
                Pause();
                es.GetComponent<StandaloneInputModule>().submitButton = "Submit_Menu_Click";
                es.GetComponent<StandaloneInputModule>().verticalAxis = "Nothing";

                currentLockMode = Cursor.lockState;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;              
            }
            if (Input.GetButtonDown("Start_Joy_1") && countdown.CountdownFinished && !speccyLose.Lose && !speccyWin.Win)
            {
                controllerThatPaused = 1;


                Cursor.lockState = CursorLockMode.Locked;

                Pause();
                es.GetComponent<StandaloneInputModule>().submitButton = "Submit_Menu_Joy_1";
                es.GetComponent<StandaloneInputModule>().verticalAxis = "Vertical_Menu_Stick_Joy_1";
            }
            if (Input.GetButtonDown("Start_Joy_2") && countdown.CountdownFinished && !speccyLose.Lose && !speccyWin.Win)
            {
                controllerThatPaused = 2;


                Cursor.lockState = CursorLockMode.Locked;

                Pause();
                es.GetComponent<StandaloneInputModule>().submitButton = "Submit_Menu_Joy_2";
                es.GetComponent<StandaloneInputModule>().verticalAxis = "Vertical_Menu_Stick_Joy_2";
            }
        }
        else
        {
            if(controllerThatPaused == 0 && Input.GetButtonDown("Escape") && countdown.CountdownFinished && !speccyLose.Lose && !speccyWin.Win)
            {
                if(controlsUp)
                {
                    CloseControlsPanel();
                }
                else
                {
                    controllerThatPaused = -1;
                    Debug.Log("Cursor: " + currentLockMode);
                    Cursor.lockState = currentLockMode;

                    Resume();
                }
            }
            if (controllerThatPaused == 1 && (Input.GetButtonDown("Start_Joy_1") || Input.GetButtonDown("Cancel_Joy_1")) && countdown.CountdownFinished && !speccyLose.Lose && !speccyWin.Win)
            {
                if (controlsUp)
                {
                    CloseControlsPanel();
                }
                else
                {
                    controllerThatPaused = -1;
                    Resume();
                }
            }
            if (controllerThatPaused == 2 && (Input.GetButtonDown("Start_Joy_2") || Input.GetButtonDown("Cancel_Joy_2")) && countdown.CountdownFinished && !speccyLose.Lose && !speccyWin.Win)
            {
                if (controlsUp)
                {
                    CloseControlsPanel();
                }
                else
                {
                    controllerThatPaused = -1;
                    Resume();
                }
            }
        }
	}

    private void CloseControlsPanel()
    {
        for (int i = 0; i < pauseButtons.Length; i++)
        {
            pauseButtons[i].interactable = true;
        }
        es.SetSelectedGameObject(pauseButtons[1].gameObject);
        controlsCanvas.SetActive(false);
        controlsUp = false;
    }
	
	public void Resume(){
		pauseMenuUI.SetActive(false);

        //Set correct spell buttons interactable again
        for (int i = 0; i < cs.queue.Length; i++)
        {
            if (cs.queue[i] != null && !onCooldown[i])
            {
                cs.queue[i].GetComponent<Button>().interactable = true;
            }
        }
        //Set correct trap buttons interactable again
        for (int i = 0; i < pt.queue.Count; i++)
        {
            if (pt.queue[i] != null && !usedTraps[i])
            {
                pt.queue[i].GetComponent<Button>().interactable = true;
            }
        }
        es.SetSelectedGameObject(selectedButton);

        es.GetComponent<StandaloneInputModule>().submitButton = "Nothing";
        Time.timeScale = 1f;

        Cursor.lockState = currentLockMode;
        //Resume Inputs
        StartCoroutine(pt.ResumeInput());
        StartCoroutine(cs.ResumeInput());
        StartCoroutine(playerMov.ResumeInput());
        GameIsPaused = false;

    }
	
	public void Pause(){
        //Set buttons not interactable
        selectedButton = es.currentSelectedGameObject;

        //Keep track of which spells are on cooldown / uninteractable so we don't set them interactable when we resume.
        //& set the button uninteractable
        onCooldown = new bool[cs.queue.Length];
        for (int i = 0; i < cs.queue.Length; i++)
        {
            if (cs.queue[i] != null)
            {
                if (cs.queue[i].GetComponent<Button>().interactable)
                {
                    onCooldown[i] = false;

                    cs.queue[i].GetComponent<Button>().interactable = false;
                }
                else
                {
                    onCooldown[i] = true;
                }
            }
        }
        //Keep track of which traps have been used && set them uninteractable
        usedTraps = new bool[pt.queue.Count];
        for (int i = 0; i < pt.queue.Count; i++)
        {
            if(pt.queue[i].GetComponent<Button>().interactable)
            {
                usedTraps[i] = false;

                pt.queue[i].GetComponent<Button>().interactable = false;
            }
            else
            {
                usedTraps[i] = true;
            }
            
        }

        //Bring up Pause menu
        pauseMenuUI.SetActive(true);
        pauseMenuUI.transform.SetAsLastSibling();
        
        if(controllerThatPaused == 1 || controllerThatPaused == 2)
            es.SetSelectedGameObject(resumeButton.gameObject);
        Time.timeScale = 0f;

        //Disable Inputs
        pt.InputEnabled = false;
        cs.InputEnabled = false;
        playerMov.InputEnabled = false;
        GameIsPaused = true;
	}





	public void LoadMenu(){
        GameObject musicPlayer = GameObject.Find("MusicPlayer");
        if (musicPlayer != null) Destroy(musicPlayer);
        Initiate.Fade("Menu", Color.black, 2);
		Time.timeScale = 1f;
	}





	public void ControlScreen(){
        controlsUp = true;
        controlsCanvas.SetActive(true);
        controlsCanvas.transform.SetAsLastSibling();

        //Set top player's image
        if (cc.topPlayersController)
        {
            topController.SetActive(true);
            topController.transform.SetAsLastSibling();
            topKeyboard.SetActive(false);
        }
        else
        {
            topKeyboard.SetActive(true);
            topKeyboard.transform.SetAsLastSibling();
            topController.SetActive(false);
        }

        //Set bottom player's image
        if(cc.GetBottomPlayerControllerState())
        {
            bottomController.SetActive(true);
            bottomController.transform.SetAsLastSibling();
            bottomKeyboard.SetActive(false);
        }
        else
        {
            bottomKeyboard.SetActive(true);
            bottomKeyboard.transform.SetAsLastSibling();
            bottomController.SetActive(false);
        }


        for(int i = 0; i < pauseButtons.Length; i++)
        {
            pauseButtons[i].interactable = false;
        }
	}





    public void QuitGame()
    {
        Debug.Log("Quiting Game");
        Application.Quit();
    }
}
