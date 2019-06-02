﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CreditsMenu : MonoBehaviour {
    [SerializeField] GameObject menuButton;
    [SerializeField] EventSystem es;
    
    private CheckControllers checkControllers;
    private SceneTransition loader;

    private void Awake()
    {
        GameObject inputObj = GameObject.Find("InputManager");
        checkControllers = inputObj.GetComponent<CheckControllers>();

        loader = GetComponent<SceneTransition>();
    }

    void Start ()
    {
        if (checkControllers.GetControllerOneState() || checkControllers.GetControllerTwoState())
        {
            es.SetSelectedGameObject(menuButton.gameObject);
        }
    }

	void Update ()
    {
        if ((checkControllers.GetControllerOneState() || checkControllers.GetControllerTwoState()) && es.currentSelectedGameObject == null)
        {
            es.SetSelectedGameObject(menuButton.gameObject);
        }

        if(Input.GetButtonDown("Cancel") || Input.GetKeyDown("Escape"))
        {
            StartCoroutine(loader.LoadScene("Menu"));
        }
    }

    public void OnClickMenu()
    {
        StartCoroutine(loader.LoadScene("Menu"));
    }
}
