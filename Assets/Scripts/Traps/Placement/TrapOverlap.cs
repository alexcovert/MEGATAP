using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapOverlap : MonoBehaviour {
    [HideInInspector]
    public bool nearbyTrap;


    private void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.layer == LayerMask.NameToLayer("TrapOverlap") || other.tag == "PickUp" || other.tag == "Platform" || other.tag == "Trigger4") && !GameObject.Equals(this.transform.parent, other.transform.parent))
        {
            nearbyTrap = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        nearbyTrap = false;
    }
}
