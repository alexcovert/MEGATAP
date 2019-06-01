using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class WinTrigger : MonoBehaviour {
	public GameObject speccyWinUI;
	public GameObject WinGameCamera;
	public GameObject CanvasUI;
	public GameObject CanvasUI2;
	public GameObject CanvasUI3;
	public GameObject CanvasUI4;
    public GameObject CanvasUI6;
    public Animator anim;
    public bool Win { get; private set; }
    [SerializeField] private GameOverMenu menu;
	void Start () {
        Win = false;
	}


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !Win)
        {
            menu.Open(true);
            speccyWinUI.SetActive(true);
            speccyWinUI.transform.SetAsLastSibling();
            
            Win = true;

            anim.Play("Dance");

            WinGameCamera.SetActive(true);
            CanvasUI.SetActive(false);
            CanvasUI2.SetActive(false);
            CanvasUI3.SetActive(false);
            CanvasUI4.SetActive(false);
            CanvasUI6.SetActive(false);
            
            StartCoroutine(fadeIn(speccyWinUI));
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
                if (im.name != "GameOver")
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
