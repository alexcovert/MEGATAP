using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CheckControllers : MonoBehaviour {
    private string[] joysticks;

    //Whether the FIRST and SECOND controllers are PLUGGED IN
    private bool controllerOne;
    private bool controllerTwo;
    //whether the TOP and BOTTOM players controllers are PLUGGED IN
    //thise will not necessarily correspond to controllerOne/controllerTwo if the character selection decides differently
    public bool topPlayersController;
    private bool bottomPlayersController;

    private Canvas canvas;
    private SetEventTriggerVars[] eventVars;
    private SetPointerClickEvents[] clickEvents;

    private Button[] trapButtons;
    private Button[] spellButtons;

    string scene;
    private InputManager inputManager;
    private PauseMenu pause;


    private void Awake()
    {
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        joysticks = Input.GetJoystickNames();
        scene = SceneManager.GetActiveScene().name;
        if (scene == "Tower1")
        {
            GameObject canv = GameObject.Find("Canvas");
            canvas = canv.GetComponent<Canvas>();

            GameObject tower = GameObject.Find("Tower");
            eventVars = tower.GetComponentsInChildren<SetEventTriggerVars>();
            clickEvents = tower.GetComponentsInChildren<SetPointerClickEvents>();
        }

        CheckConnected();
        Debug.Log("Player 1 controller: " + controllerOne);
        Debug.Log("Player 2 controller: " + controllerTwo);


    }

    private void Update()
    {
        if(Time.timeScale != 0)
        {
            CheckConnected();
            if (topPlayersController)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                if (canvas != null && eventVars != null)
                {
                    canvas.GetComponent<GraphicRaycaster>().enabled = true;
                    foreach (SetEventTriggerVars v in eventVars)
                    {
                        v.enabled = true;
                    }
                    foreach (SetPointerClickEvents p in clickEvents)
                    {
                        p.enabled = true;
                    }
                }
            }
            else if ((!inputManager.P1IsTop && !controllerOne) || !topPlayersController)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                if (canvas != null && eventVars != null && topPlayersController)
                {
                    canvas.GetComponent<GraphicRaycaster>().enabled = false;
                    foreach (SetEventTriggerVars v in eventVars)
                    {
                        v.enabled = false;
                    }
                    foreach (SetPointerClickEvents p in clickEvents)
                    {
                        p.enabled = false;
                    }
                }
            }
        }
    }

    private void CheckConnected()
    {
        joysticks = Input.GetJoystickNames();

        if (joysticks.Length == 2)
        {
            for (int i = 0; i < joysticks.Length; i++)
            {
                if (!string.IsNullOrEmpty(joysticks[i]))
                {
                    //Debug.Log("Controller " + i + " is connected using: " + joysticks[i]);
                    if (i == 0)
                    {
                        controllerOne = true;
                        if(inputManager.P1IsTop)
                        {
                            topPlayersController = true;
                        }
                        else //if P1 is bottom
                        {
                            bottomPlayersController = true;
                        }
                    }
                    if (i == 1)
                    {
                        controllerTwo = true;
                        if(inputManager.P1IsTop)
                        {
                            bottomPlayersController = true;
                        }
                        else // if P1 is bottom / p2 is top
                        {
                            topPlayersController = true;
                        }
                    }
                }
                else
                {
                    //Debug.Log("Controller " + i + " is disconnected.");
                    if (i == 0)
                    {
                        controllerOne = false;
                        if (inputManager.P1IsTop)
                        {
                            topPlayersController = false;
                        }
                        else //if P1 is bottom
                        {
                            bottomPlayersController = false;
                        }
                    }
                    if (i == 1)
                    {
                        controllerTwo = false;
                        if (inputManager.P1IsTop)
                        {
                            bottomPlayersController = false;
                        }
                        else // if P1 is bottom / p2 is top
                        {
                            topPlayersController = false;
                        }

                    }
                }
            }
        }
        else if(joysticks.Length == 1)
        {
            controllerOne = false;
            controllerTwo = true;
            if (inputManager.P1IsTop)
            {
                bottomPlayersController = false;
            }
            else // if P1 is bottom / p2 is top
            {
                bottomPlayersController = true;
                //topPlayersController = true;
            }
        }
        else
        {
            controllerOne = false;
            controllerTwo = false;
            topPlayersController = false;
            bottomPlayersController = false;
        }
    }

    public bool GetControllerTwoState()
    {
        return controllerTwo;
    }

    public bool GetControllerOneState()
    {
        return controllerOne;
    }
    
    public bool GetTopPlayerControllerState()
    {
        return topPlayersController;
    }

    public bool GetBottomPlayerControllerState()
    {
        return bottomPlayersController;
    }
}
