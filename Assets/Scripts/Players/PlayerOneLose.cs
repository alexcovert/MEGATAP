using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerOneLose : MonoBehaviour {
    public bool Lose { get; private set; }
    private CameraOneRotator cam;
    [SerializeField] MoveVines vines;
	public GameObject speccyLosesUI;
	public GameObject LoseGameCamera;
	public GameObject CanvasUI;

    private void Start () {
        Lose = false;
        cam = GetComponent<CameraOneRotator>();
	}


    void OnTriggerEnter(Collider other)
    {
        //check collision with Rising walls that are tagged with "rise"
        if (other.tag == "Vine" && cam.GetFloor() == vines.GetVineFloor() && cam.GetState() == vines.GetVineFace() && vines.Started)
        {
            Lose = true;
            // Initiate.Fade("GameOver", Color.black, 1);
            speccyLosesUI.SetActive(true);
            speccyLosesUI.transform.SetAsLastSibling();
            LoseGameCamera.SetActive(true);
            CanvasUI.SetActive(false);
        }
    }
    
    
}
