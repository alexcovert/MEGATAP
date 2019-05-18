using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class GameOverMenu : MonoBehaviour {
    [SerializeField] private EventSystem es;
    [SerializeField] private GameObject charSelectButton;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private TextMeshProUGUI text;
    private CheckControllers cc;
    private TextMeshProUGUI charSelectText;

    private void Start()
    {
        cc = GameObject.Find("InputManager").GetComponent<CheckControllers>();
        charSelectText = charSelectButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Open(bool speccyWin)
    {
        if(speccyWin)
        {
            text.text = "Bottom Player Wins!";
        }
        else
        {
            text.text = "Top Player Wins!";
        }

        es.GetComponent<StandaloneInputModule>().submitButton = "Submit_Menu";
        if (cc != null && charSelectText != null)
        {
            if (!(cc.GetControllerOneState() || cc.GetControllerTwoState()))
            {
                charSelectButton.GetComponent<Button>().interactable = false;
                charSelectText.color = new Color(charSelectText.color.r, charSelectText.color.g, charSelectText.color.b, 0.5f);
            }
            else
            {
                charSelectButton.GetComponent<Button>().interactable = true;
                charSelectText.color = new Color(charSelectText.color.r, charSelectText.color.g, charSelectText.color.b, 1);
            }
        }

        if (cc.GetBottomPlayerControllerState() || cc.topPlayersController)
        {

            es.SetSelectedGameObject(restartButton);
        }
    }

    public void onClickRetry()
    {
        SceneManager.LoadScene("Tower1");
    }
    
    public void onClickTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
    
    public void onClickCharacterSelect()
    {
    	SceneManager.LoadScene("CharacterSelect");
    }

    public void onClickMenu()
    {
        GameObject musicPlayer = GameObject.Find("MusicPlayer");
        if (musicPlayer != null) Destroy(musicPlayer);
        SceneManager.LoadScene("Menu");
    }
    
    public void QuitGame()
    {
        Debug.Log("Quiting Game");
        Application.Quit();
    }
}

