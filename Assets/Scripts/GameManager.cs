using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour {


    private void Awake()
    {
        if(SceneManager.GetActiveScene().name == "Tower1")
        {
            bool platformsLoaded = false;
            bool trapsLoaded = false;

            for(int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == "Tower1_Platforms")
                    platformsLoaded = true;
                if (SceneManager.GetSceneAt(i).name == "Tower1_Traps")
                    trapsLoaded = true;
            }

            if(!platformsLoaded) SceneManager.LoadScene("Tower1_Platforms", LoadSceneMode.Additive);
            if(!trapsLoaded) SceneManager.LoadScene("Tower1_Traps", LoadSceneMode.Additive);

        }
        if(SceneManager.GetActiveScene().name == "Tutorial")
        {
            SceneManager.LoadScene("Tutorial_Platforms", LoadSceneMode.Additive);
        }
    }
}
