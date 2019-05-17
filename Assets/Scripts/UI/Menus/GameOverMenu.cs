using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameOverMenu : MonoBehaviour {
    [SerializeField] private EventSystem es;

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

