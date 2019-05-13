using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour {


    private void Awake()
    {
        if(SceneManager.GetActiveScene().name == "Tower1")
        {
            SceneManager.LoadScene("Tower1_Platforms", LoadSceneMode.Additive);
            SceneManager.LoadScene("Tower1_Traps", LoadSceneMode.Additive);
        }
        if(SceneManager.GetActiveScene().name == "Tutorial")
        {
            SceneManager.LoadScene("Tutorial_Platforms", LoadSceneMode.Additive);
        }
    }
}
