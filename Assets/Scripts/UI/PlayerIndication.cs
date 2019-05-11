using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerIndication : MonoBehaviour {
    [SerializeField] private Camera camTop;
    [SerializeField] GameObject playerOne;
    [SerializeField] GameObject playerTwo;
    [SerializeField] TextMeshProUGUI topText;
    [SerializeField] TextMeshProUGUI botText;
    [SerializeField] private float time = 0.5f;

    
    private InputManager inputManager;
    private int numTimesRotated = 0;
    [SerializeField] private GameObject tower;
    private PauseMenu pause;
    [SerializeField] private GameObject gameManager;

    private int floorTop = 2;
    private int floorBot = 1;

    //private int floorDifTop = 0;
    //private int floorDifBot = 0;

    private int playerOneCurrentFace = 1;
    private int playerOneLastFace = 1;

    private Color colorTop;
    private Color colorBot;

    private CameraOneRotator cam1;
    private CameraTwoRotator cam2;
    private PlaceTrap placeTrap;

    // Use this for initialization
    void Awake () {
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        pause = gameManager.GetComponent<PauseMenu>();
        colorTop = topText.color;
        colorBot = botText.color;

        cam1 = playerOne.GetComponent<CameraOneRotator>();
        cam2 = camTop.GetComponent<CameraTwoRotator>();
        placeTrap = playerTwo.GetComponent<PlaceTrap>();
    }
	
	// Update is called once per frame
	void Update () {

        //floorDifTop = floorTop - floorBot;
        //floorDifBot = floorBot - floorTop;

        numTimesRotated = placeTrap.GetNumRotated();
        
        if(playerOneCurrentFace != cam1.GetState())
        {
            playerOneLastFace = playerOneCurrentFace;
            playerOneCurrentFace = cam1.GetState();
        }
        if(playerOneLastFace > playerOneCurrentFace)
        {
            StartCoroutine(BotFading());
        }
        if (inputManager.GetButtonDown(InputCommand.TopPlayerRotate) && !pause.GameIsPaused && numTimesRotated%4 == 0 &&numTimesRotated < 4 * (tower.GetComponentInChildren<NumberOfFloors>().NumFloors - 1) - 1)
        {
            StartCoroutine(TopFading());
        }

        floorTop = cam2.GetFloor();
        floorBot = cam1.GetFloor();

        topText.text = "Floor " + floorTop;
        botText.text = "Floor " + floorBot;

        //if (floorDifTop > 0)
        //{
        //    if (floorDifTop == 1)
        //    {
        //        topText.text = "Speccy: " + (floorDifTop) + " floor below";
        //    }
        //    else
        //    {
        //        topText.text = "Speccy: " + (floorDifTop) + " floors below";
        //    }
        //}
        //else if (floorDifTop < 0)
        //{
        //    if (floorDifTop == -1)
        //    {
        //        topText.text = "Speccy: " + (-floorDifTop) + "floor above";
        //    }
        //    else
        //    {
        //        topText.text = "Speccy: " + (-floorDifTop) + "floors above";
        //    }
        //}
        //else
        //{
        //    topText.text = "Speccy: on same floor";
        //}

        //if (floorDifBot < 0)
        //{
        //    if (floorDifBot == -1)
        //    {
        //        botText.text = "Ollie: " + (-floorDifBot) + " floor above";
        //    }
        //    else
        //    {
        //        botText.text = "Ollie: " + (-floorDifBot) + " floors above";
        //    }
        //}
        //else if (floorDifBot > 0)
        //{
        //    if (floorDifBot == 1)
        //    {
        //        botText.text = "Ollie: " + floorDifBot + " floor below";
        //    }
        //    else
        //    {
        //        botText.text = "Ollie: " + floorDifBot + " floors below";
        //    }
        //}
        //else
        //{
        //    botText.text = "Ollie: on same floor";
        //}

    }
    private IEnumerator BotFading()
    {
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            botText.color = new Color(colorBot.r, colorBot.g, colorBot.b, Mathf.Lerp(0, 1, t / time));
            yield return null;
        }
        
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            botText.color = new Color(colorBot.r, colorBot.g, colorBot.b, Mathf.Lerp(1, 0, t / time));
            yield return null;
        }
    }

    private IEnumerator TopFading()
    {
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            topText.color = new Color(colorTop.r, colorTop.g, colorTop.b, Mathf.Lerp(0, 1, t / time));
            yield return null;
        }

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            topText.color = new Color(colorTop.r, colorTop.g, colorTop.b, Mathf.Lerp(1, 0, t / time));
            yield return null;
        }
    }
}
