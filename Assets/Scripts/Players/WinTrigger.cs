using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour {
	public GameObject speccyWinUI;
	public GameObject WinGameCamera;
	public GameObject CanvasUI;
	public GameObject CanvasUI2;
	public GameObject CanvasUI3;
	public GameObject CanvasUI4;
	public GameObject CanvasUI5;
	public GameObject CanvasUI6;
    public bool Win { get; private set; }

	void Start () {
        Win = false;
	}


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //Initiate.Fade("VictoryScreen", Color.black, 1);
            Win = true;
            speccyWinUI.SetActive(true);
            speccyWinUI.transform.SetAsLastSibling();
            WinGameCamera.SetActive(true);
            CanvasUI.SetActive(false);
            CanvasUI2.SetActive(false);
            CanvasUI3.SetActive(false);
            CanvasUI4.SetActive(false);
            CanvasUI5.SetActive(false);
            CanvasUI6.SetActive(false);
        }
    }
}
