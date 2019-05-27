using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BeginGo : MonoBehaviour {
    public bool CountdownFinished { get; private set; }
    [SerializeField] private GameObject canvas;
    [SerializeField] private Camera camBot;
    [SerializeField] private Camera camTop;
    [SerializeField] private Camera ZoomCam;
    [SerializeField] GameObject playerTwo;
    [SerializeField] GameObject playerOne;
    [SerializeField] private GameObject countdownCanvas;
    [SerializeField] EventSystem es;
    [SerializeField] float countdownTime = 4;
    private bool once;

    private PlaceTrap pt;
    private CastSpell cs;
    private PlayerOneMovement playerMov;
    private GameObject activeCountdown;
    [SerializeField] private PauseMenu pause;

    private Vector3 TargetPosition;

    [SerializeField] private float moveInSpeed = 0.1f;

    // Use this for initialization
    void Start () {
        pt = playerTwo.GetComponent<PlaceTrap>();
        cs = playerTwo.GetComponent<CastSpell>();
        playerMov = playerOne.GetComponent<PlayerOneMovement>(); 

        canvas.SetActive(false);
        TargetPosition = new Vector3(camTop.transform.position.x, 21, -75);

        camBot.enabled = false;
        camTop.enabled = false;

        es.GetComponent<StandaloneInputModule>().submitButton = "Submit_Menu";
        pt.InputEnabled = false;
        cs.InputEnabled = false;
        playerMov.InputEnabled = false;
        pause.GameIsPaused = true;

        CountdownFinished = false;
    }

    private void Update()
    {
        ZoomCam.transform.position = Vector3.Lerp(ZoomCam.transform.position, TargetPosition , moveInSpeed);
        if (ZoomCam.transform.position.x <= camTop.transform.position.x && ZoomCam.transform.position.y <= 21 + 10 && ZoomCam.transform.position.z <= -70 && once == false)
        {

            StartCoroutine(StartDelay());
        }
    }

    private IEnumerator StartDelay()
    {
        once = true;

        float timePerNum = countdownTime / 3;
        GameObject count = Instantiate(countdownCanvas);
        TextMeshProUGUI text = count.GetComponentInChildren<TextMeshProUGUI>();



        count.SetActive(true);

        text.text = "3";
        yield return new WaitForSeconds(timePerNum);
        text.text = "2";
        yield return new WaitForSeconds(timePerNum);
        text.text = "1";
        yield return new WaitForSeconds(timePerNum);
        text.text = "Go!";
        yield return new WaitForSeconds(1);

        count.SetActive(false);

        es.GetComponent<StandaloneInputModule>().submitButton = "Nothing";


        canvas.SetActive(true);

        camBot.enabled = true;
        camTop.enabled = true;
        ZoomCam.enabled = false;

        pt.InputEnabled = true;
        cs.InputEnabled = true;
        playerMov.InputEnabled = true;
        pause.GameIsPaused = false;
        CountdownFinished = true;


        //Show
        ZoomCam.cullingMask |= 1 << LayerMask.NameToLayer("Trees1");
        GameObject.Find("Player 2").GetComponent<ChangeNav>().ResetNav();
    }
}
