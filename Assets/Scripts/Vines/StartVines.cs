using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartVines : MonoBehaviour {
    [SerializeField] private MoveVines vines;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            vines.Started = true;
            Destroy(this.gameObject);
        }
    }
}
