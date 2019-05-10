using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Destroy all scripts and colliders on a TRAP once a player has passed it's face.
public class Destroy : MonoBehaviour {
    private CameraOneRotator bottomPlayer;
    private CameraTwoRotator topPlayer;

    //The floor and face that this object is placed on
    private int floor;
    private int face;

    private void Start()
    {
        bottomPlayer = GameObject.Find("Player 1").GetComponent<CameraOneRotator>();
        topPlayer = GameObject.Find("Player 2 Camera").GetComponent<CameraTwoRotator>();
        floor = topPlayer.GetFloor();
        face = topPlayer.GetState();
    }

    private void Update()
    {
        int topPlayerState = (topPlayer.GetFloor() * 4) + topPlayer.GetState() - 4;
        int bottomPlayerState = (bottomPlayer.GetFloor() * 4) + bottomPlayer.GetState() - 4;
        int currentState = (floor * 4) + face - 4;
        //Debug.Log(topPlayerState + ", " + bottomPlayerState + ", " + currentState);

        if (topPlayerState >= currentState + 1 && bottomPlayerState >= currentState + 1)
        {
            Component[] components = GetComponents<Component>();
            Component[] childComponents = GetComponentsInChildren<Component>();

            foreach(Component c in components)
            {
                //Desetroy ALL components besides meshrenderers and transforms so it stays the same visually
                if (c.GetType() != typeof(Transform) && c.GetType() != typeof(SkinnedMeshRenderer) && c.GetType() != typeof(MeshRenderer) && c != this)
                {
                    Destroy(c);
                }
            }

            foreach (Component c in childComponents)
            {
                if (c.name == "Base" || c.name == "PlaceImage")
                {
                    Destroy(c.gameObject);
                }

                if(c.GetType() == typeof(TrapOverlap))
                {
                    Destroy(c);
                }
            }
            
            Destroy(this);
        }
    }

}
