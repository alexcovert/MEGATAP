using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class GameOverMenu : MonoBehaviour {
    public bool GameOver = false;

    [SerializeField] private EventSystem es;
    [SerializeField] private GameObject charSelectButton;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private GameObject fadePrefab;
    [SerializeField] private MoveVines vines;

    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioClip speccyWinMusic;
    [SerializeField] private AudioClip speccyLoseMusic;

    private PlayerOneMovement player1;
    private PlaceTrap pt;
    private CastSpell cs;
    private CheckControllers cc;
    private TextMeshProUGUI charSelectText;
    private SceneTransition loader;
    private StandaloneInputModule inputModule;

    private void Start()
    {
        //inputs
        cc = GameObject.Find("InputManager").GetComponent<CheckControllers>();
        inputModule = es.GetComponent<StandaloneInputModule>();

        //char select stuff - we disable the button if they're only playing with mouse and keyboard
        charSelectText = charSelectButton.GetComponentInChildren<TextMeshProUGUI>();

        //players
        GameObject p1 = GameObject.Find("Player 1");
        player1 = p1.GetComponent<PlayerOneMovement>();

        GameObject p2 = GameObject.Find("Player 2");
        pt = p2.GetComponent<PlaceTrap>();
        cs = p2.GetComponent<CastSpell>();


        loader = GetComponent<SceneTransition>();
    }

    public void Open(bool speccyWin)
    {
        //Start fade
        Instantiate(fadePrefab);
        StartCoroutine(FadeMusic(speccyWin));

        //Disable inputs for players
        cs.InputEnabled = false;
        pt.InputEnabled = false;
        player1.InputEnabled = false;
        player1.SetMove(false);
        vines.Started = false;
        GameOver = true;

        //Change game over text
        if (speccyWin)
        {
            text.text = "Bottom Player Wins!";
        }
        else
        {
            text.text = "Top Player Wins!";
        }
        
        //set input module
        inputModule.submitButton = "Submit_Menu";
        inputModule.repeatDelay = 0.5f;

        //set character select button interactable or uninteractable based on whether they're using controllers
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
    }

    public void SetSelected()
    {
        if (cc.GetBottomPlayerControllerState() || cc.topPlayersController)
        {
            es.SetSelectedGameObject(restartButton);
        }    
    }

    //Buttons
    public void onClickRetry()
    {
        StartCoroutine(loader.LoadScene("Tower1"));
    }
    
    public void onClickTutorial()
    {
        StartCoroutine(loader.LoadScene("Tutorial"));
    }
    
    public void onClickCharacterSelect()
    {
        StartCoroutine(loader.LoadScene("CharacterSelect"));
    }

    public void onClickMenu()
    {
        GameObject musicPlayer = GameObject.Find("MusicPlayer");
        if (musicPlayer != null) Destroy(musicPlayer);
        StartCoroutine(loader.LoadScene("Menu"));
    }
    
    public void QuitGame()
    {
        Debug.Log("Quiting Game");
        Application.Quit();
    }



    //Music fade
    private IEnumerator FadeMusic(bool speccyWin)
    {
        float fadeTime = 2;

        //fade out
        for(float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            musicPlayer.volume = Mathf.Lerp(1, 0, t / fadeTime);
            yield return null;
        }

        //set new clip
        if(speccyWin)
        {
            musicPlayer.clip = speccyWinMusic;
        }
        else
        {
            musicPlayer.clip = speccyLoseMusic;
        }
        musicPlayer.Play();

        //fade back in
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            musicPlayer.volume = Mathf.Lerp(0, 1, t / fadeTime);
            yield return null;
        }
    }
}

