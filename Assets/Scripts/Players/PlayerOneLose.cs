using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerOneLose : MonoBehaviour {
    public bool Lose { get; private set; }
    private CameraOneRotator cam;
    [SerializeField] MoveVines vines;
	public GameObject speccyLosesUI;
	public GameObject LoseGameCamera;
	public GameObject CanvasUI;
	public GameObject CanvasUI2;
	public GameObject CanvasUI3;
	public GameObject CanvasUI4;
	public GameObject CanvasUI6;
    [SerializeField] private GameOverMenu menu;

    private bool tutorial;

    private AudioSource audioSource;
    [SerializeField] private AudioClip gameOverClip;

    private void Start () {
        Lose = false;
        cam = GetComponent<CameraOneRotator>();
        if (SceneManager.GetActiveScene().name == "Tutorial") tutorial = true;
        audioSource = GameObject.Find("VineParent").GetComponent<AudioSource>();
	}


    void OnTriggerEnter(Collider other)
    {
        //check collision with Rising walls that are tagged with "rise"
        if (!tutorial && other.tag == "Vine" && cam.GetFloor() == vines.GetVineFloor() && cam.GetState() == vines.GetVineFace() && vines.Started && !Lose)
        {
            menu.Open(false);
            audioSource.volume -= 0.2f;
            if(gameOverClip != null) audioSource.PlayOneShot(gameOverClip);
            Lose = true;
            // Initiate.Fade("GameOver", Color.black, 1);
            speccyLosesUI.SetActive(true);
            speccyLosesUI.transform.SetAsLastSibling();

            
            LoseGameCamera.SetActive(true);
            CanvasUI.SetActive(false);
            CanvasUI2.SetActive(false);
            CanvasUI3.SetActive(false);
            CanvasUI4.SetActive(false);
            CanvasUI6.SetActive(false);
            StartCoroutine(fadeIn(speccyLosesUI));

            
        }
    }


    private IEnumerator fadeIn(GameObject ui)
    {
        Image[] images = ui.GetComponentsInChildren<Image>();
        TextMeshProUGUI[] text = ui.GetComponentsInChildren<TextMeshProUGUI>();

        for (float i = 0; i < 2; i += Time.deltaTime)
        {
            foreach (Image im in images)
            {
                if(im.name != "GameOver")
                    im.color = new Color(im.color.r, im.color.g, im.color.b, Mathf.Lerp(0, 1, i / 2));
            }
            foreach (TextMeshProUGUI t in text)
            {
                t.color = new Color(t.color.r, t.color.g, t.color.b, Mathf.Lerp(0, 1, i / 2));
            }


            yield return null;
        }


        menu.SetSelected();
    }


}
