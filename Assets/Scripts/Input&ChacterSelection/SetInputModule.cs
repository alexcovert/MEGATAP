using UnityEngine;
using UnityEngine.EventSystems;

//Sets the horizontal navigation component of the event system based on which controller is the top player
public class SetInputModule : MonoBehaviour {
    private InputManager inputManager;
    private StandaloneInputModule inputModule;

    private void Awake()
    {
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        inputModule = GetComponent<StandaloneInputModule>();
    }

    private void Start ()
    {
        if(inputManager.P1IsTop)
        {
            inputModule.horizontalAxis = "Bumpers_Joy_1";
        }
        else
        {
            inputModule.horizontalAxis = "Bumpers_Joy_2";
        }
	}

}
