using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMultipleBases : MonoBehaviour {

    private CheckValidLocations[] checkLocations;
	public bool Valid { get; private set; }
    public bool Placed;

	void Awake () {
        checkLocations = GetComponentsInChildren<CheckValidLocations>();	
	}

    private void Update()
    {
        if(!Placed)
        {
            bool valid = true;
            foreach (CheckValidLocations check in checkLocations)
            {
                if (!check.Valid)
                {
                    valid = false;
                }
            }

            Valid = valid;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        Valid = false;
    }

}
