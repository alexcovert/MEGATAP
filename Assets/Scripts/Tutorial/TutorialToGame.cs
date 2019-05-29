using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialToGame : MonoBehaviour {

    private InputManager inputManager;
    private SceneTransition loader;
    private ChangeNav changeNav;

    private void Awake()
    {
        GameObject inputMan = GameObject.Find("InputManager");
        inputManager = inputMan.GetComponent<InputManager>();
        loader = GetComponent<SceneTransition>();

        changeNav = GameObject.Find("Player 2").GetComponent<ChangeNav>();
    }
    private void Start()
    {
        changeNav.ResetNav();
    }
    private void Update()
    {
        if (inputManager.GetButtonDown(InputCommand.Start))
        {
            StartCoroutine(loader.LoadScene("Tower1"));
        }
    }
}
