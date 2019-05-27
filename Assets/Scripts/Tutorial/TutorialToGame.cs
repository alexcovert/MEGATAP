using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialToGame : MonoBehaviour {

    private InputManager inputManager;
//    private CheckControllers checkControllers;
    private SceneTransition loader;

    private void Awake()
    {
        GameObject inputMan = GameObject.Find("InputManager");
        inputManager = inputMan.GetComponent<InputManager>();
  //      checkControllers = inputMan.GetComponent<CheckControllers>();
        loader = GetComponent<SceneTransition>();
    }

    private void Update()
    {
        if (inputManager.GetButtonDown(InputCommand.Start) || Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(loader.LoadScene("Tower1"));
        }
    }
}
