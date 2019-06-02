using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapOnlyChecking : MonoBehaviour {

    [HideInInspector]
    public bool nearbyTrap;


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("TrapOverlap") && !GameObject.Equals(this.transform.parent, other.transform.parent))
        {
            nearbyTrap = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        nearbyTrap = false;
    }
}
