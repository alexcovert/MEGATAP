using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameOverMenu : MonoBehaviour {
    [SerializeField] private EventSystem es;
    [SerializeField] private GameObject[] menuButtons;
    private CheckControllers cc;

    private void Start()
    {
        GameObject inputManager = GameObject.Find("InputManager");
        cc = inputManager.GetComponent<CheckControllers>();
        if(cc.GetControllerOneState() || cc.GetControllerTwoState())
        {
            es.SetSelectedGameObject(menuButtons[0]);
        }
    }

    private void Update()
    {
        if ((cc.GetControllerOneState() || cc.GetControllerTwoState()) && es.currentSelectedGameObject == null)
        {
            es.SetSelectedGameObject(menuButtons[0]);
        }
    }

    public void onClickRetry()
    {
        SceneManager.LoadScene("Tower1");
    }

    public void onClickMenu()
    {
        GameObject musicPlayer = GameObject.Find("MusicPlayer");
        if (musicPlayer != null) Destroy(musicPlayer);
        SceneManager.LoadScene("Menu");
    }
}
