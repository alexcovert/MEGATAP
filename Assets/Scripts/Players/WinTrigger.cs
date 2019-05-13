using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour {
    public bool Win { get; private set; }

	void Start () {
        Win = false;
	}


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Initiate.Fade("VictoryScreen", Color.black, 1);
            Win = true;
        }
    }
}
