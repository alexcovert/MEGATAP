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

    private void Start()
    {
        cc = GameObject.Find("InputManager").GetComponent<CheckControllers>();
        charSelectText = charSelectButton.GetComponentInChildren<TextMeshProUGUI>();
        GameObject p1 = GameObject.Find("Player 1");
        player1 = p1.GetComponent<PlayerOneMovement>();

        GameObject p2 = GameObject.Find("Player 2");
        pt = p2.GetComponent<PlaceTrap>();
        cs = p2.GetComponent<CastSpell>();
    }

    public void Open(bool speccyWin)
    {
        Instantiate(fadePrefab);
        cs.InputEnabled = false;
        pt.InputEnabled = false;
        player1.InputEnabled = false;
        player1.SetMove(false);
        vines.Started = false;
        GameOver = true;

        if (speccyWin)
        {
            text.text = "Bottom Player Wins!";
        }
        else
        {
            text.text = "Top Player Wins!";
        }

        StartCoroutine(FadeMusic(speccyWin));

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
    }

    public void SetSelected()
    {
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

    private IEnumerator FadeMusic(bool speccyWin)
    {
        float fadeTime = 2;

        for(float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            musicPlayer.volume = Mathf.Lerp(1, 0, t / fadeTime);
            yield return null;
        }

        if(speccyWin)
        {
            musicPlayer.clip = speccyWinMusic;
        }
        else
        {
            musicPlayer.clip = speccyLoseMusic;
        }
        musicPlayer.Play();

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            musicPlayer.volume = Mathf.Lerp(0, 1, t / fadeTime);
            yield return null;
        }
        Debug.Log(musicPlayer.volume);
    }
}

