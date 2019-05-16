using UnityEngine;

//<alex>
[RequireComponent(typeof(BoxCollider))]
public class CheckValidLocations : MonoBehaviour {
    private TrapBase tb;
    private Location toCheck;
    public bool Valid { get; private set; }
    public bool Placed;
    private void Start()
    {
        tb = GetComponentInParent<TrapBase>();
        toCheck = tb.ValidLocations;

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
            Direction dir = GameObject.Find("Player 2").GetComponent<PlaceTrap>().CurrentDirection;
            switch (toCheck)
            {
                case Location.AnySurface:
                    Valid = valid;
                    break;
                case Location.Floor:
                    if (dir == Direction.Up)
                    {
                        Valid = valid;
                    }
                    break;
                case Location.Ceiling:
                    if (dir == Direction.Up)
                    {
                        Valid = valid;
                    }
                    break;
                //<ac> TODO: All other directions; need traps to test with first.
                case Location.AnyWall:
                case Location.FloorAndCeiling:
                case Location.FreeFloat:
                case Location.LeftWall:
                case Location.RightWall:
                    Debug.Log("Snapping not implemented for this trap");
                    Valid = true;
                    break;
            }
        }
    }
}
