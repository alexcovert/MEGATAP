using UnityEngine;

//<alex>
[RequireComponent(typeof(BoxCollider))]
public class CheckValidLocations : MonoBehaviour {
    private TrapBase tb;

    public bool Valid { get; private set; }
    public bool Placed;

    private void Start()
    { 
        Valid = false;
    }

    private void OnTriggerStay(Collider other)
    {
        SetValidBool(true, other.tag);
    }

    private void OnTriggerExit(Collider other)
    {
        SetValidBool(false, other.tag);
    }

    //Set whether the trap is in a valid location based on whether it's colliding with a platform
    //and what direction it is facing.
    private void SetValidBool(bool valid, string tag)
    {
        if (tag == "Platform" && !Placed)
        {
            Valid = valid;
        }
    }
}
