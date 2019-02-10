using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlaceTrap : MonoBehaviour {
    [SerializeField] private GameObject[] trapButtons;
    [SerializeField] private TrapBase[] trapPrefabs;
    [SerializeField] private Image controllerCursor;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameManager gm;
    [SerializeField] private Camera cam;


    [SerializeField] private int cursorSpeed;
    [SerializeField] private int gridSize;

    [SerializeField] private int queueSize = 7;
    private List<GameObject> queue = new List<GameObject>();
    [SerializeField] private GameObject trapQueue;
    private int queueIndex;

    private TrapBase trap;
    private GameObject ghostTrap;
    private GameObject previouslySelected;

    private bool p2Controller;
    private bool placeEnabled;

    private bool active = true;

	void Start () {
        //Handle cursor or set buttons if controller connected
        p2Controller = gm.GetControllerTwoState();
        if(p2Controller)
        {
            controllerCursor.enabled = true;
        }
        else
        {
            controllerCursor.enabled = false;
        }

        CreateTrapQueue();

        placeEnabled = false;
        trapQueue.transform.SetAsLastSibling();
    }
	

	void Update () {
        //Move controller cursor & get input
        p2Controller = gm.GetControllerTwoState();
        if (p2Controller)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal_Joy_2")) > 0.6f || Mathf.Abs(Input.GetAxisRaw("Vertical_Joy_2")) > 0.6f)
            {
                controllerCursor.transform.Translate(Input.GetAxisRaw("Horizontal_Joy_2") * cursorSpeed, Input.GetAxisRaw("Vertical_Joy_2") * cursorSpeed, 0);
            }
            
            if (Input.GetButton("Place_Joy_2") && placeEnabled)
            {
                SetTrap();
            }
        }
        MoveGhost();
        if (trap != null && ghostTrap != null) CheckValidLocation();

        if (Input.GetButtonDown("Submit_Joy_2"))
        {
            DestroyGhost();
            ClearTrapQueue();
            CreateTrapQueue();
        }

        if (Input.GetButtonDown("Swap_Queue"))
        {
            DestroyGhost();
            SwitchQueue();
        }

       // Debug.Log(Input.GetAxis("Horizontal_Menu"));
    }

    private Vector3? GetGridPosition()
    {
        if (RaycastFromCam() != null)
        {
            RaycastHit hit = RaycastFromCam().Value;
            int hitX = -1;
            int hitZ = -1;
            switch (cam.GetComponent<CameraTwoRotator>().GetState())
            {
                case 1:
                    hitX = Mathf.RoundToInt((hit.point.x - 1) / gridSize) * gridSize + 1;
                    hitZ = Mathf.RoundToInt(hit.point.z + -2);
                    break;
                case 2:
                    hitX = Mathf.RoundToInt(hit.point.x + 2);
                    hitZ = Mathf.RoundToInt((hit.point.z - 1) / gridSize) * gridSize + 1;
                    break;
                case 3:
                    hitX = Mathf.RoundToInt((hit.point.x - 1) / gridSize) * gridSize + 1;
                    hitZ = Mathf.RoundToInt(hit.point.z + 2);
                    break;
                case 4:
                    hitX = Mathf.RoundToInt(hit.point.x + -2);
                    hitZ = Mathf.RoundToInt((hit.point.z - 1) / gridSize) * gridSize + 1;
                    break;
            }
            int hitY = Mathf.RoundToInt((hit.point.y - 1)/ gridSize) * gridSize + 1;
            return new Vector3(hitX, hitY, hitZ);
        }
        else return null;
    }

    private RaycastHit? RaycastFromCam()
    {
        RaycastHit hit;
        Ray ray;
        if (p2Controller)
        {
            ray = cam.ScreenPointToRay(controllerCursor.transform.position);
        }
        else
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
        }
        if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("Tower")))
        {
            return hit;
        }
        else
        {
            return null;
        }
    }

    //Called from event trigger on center column of tower when player clicks on it
    public void OnClickTower()
    {
        if(!Input.GetMouseButtonUp(1))
        {
            SetTrap();
        }
    }

    private void SetTrap()
    {
        if (GetGridPosition() != null && CheckNearby())
        {

            Vector3 position = GetGridPosition().Value;
            if (ghostTrap != null && CheckFloor(position.y))
            {
                trap.InstantiateTrap(position, ghostTrap.transform.rotation);
                ClearButton();
                trap = null;
                DestroyGhost();

                if (p2Controller)
                {
                    //eventSystem.SetSelectedGameObject(previouslySelected);
                    for(int i = queue.Count - 1; i >= 0; i--)
                    {
                        if(queue[i].activeInHierarchy)
                        {
                            eventSystem.SetSelectedGameObject(queue[i]);
                        }
                    }
                    placeEnabled = false;
                }
            }
        }
    }

    //Check to see that mage is clicking on correct floor
    private bool CheckFloor(float hitY)
    {
        int floor = cam.GetComponent<CameraTwoRotator>().GetFloor();
        float upperLimit = floor * 20;
        float lowerLimit = upperLimit - 20;

        return (hitY >= lowerLimit && hitY <= upperLimit);
    }

    //Check  if it's being placed on correct object
    private bool CheckValidLocation()
    {
        //Debug.Log(trap.ValidLocations);
        return true;

    }

    private bool CheckNearby()
    {
        if (ghostTrap != null)
        {
            if (ghostTrap.GetComponentInChildren<TrapOverlap>() != null && ghostTrap.GetComponentInChildren<TrapOverlap>().nearbyTrap)
            {
                return false;
            }

            return true;
        }

        return false;
    }

    private void SetGhost()
    {
        if(trap != null)
        {
            ghostTrap = trap.InstantiateTrap(Vector3.zero);
        }
        
        
        Destroy(ghostTrap.GetComponent<Collider>());

        //Make half transparent
        if (ghostTrap.GetComponentInChildren<MeshRenderer>() != null)
        {
            Color color = ghostTrap.GetComponentInChildren<MeshRenderer>().material.color;
            color.a = 0.5f;
            ghostTrap.GetComponentInChildren<MeshRenderer>().material.color = color;
        }

    }

    private void MoveGhost()
    {
        if (ghostTrap != null)
        {
            UpdateRotationInput();
            FinalizeRotationInput();

            if (GetGridPosition() != null)
            {
                Vector3 position = GetGridPosition().Value;
                ghostTrap.transform.position = position;

                if (Input.GetMouseButton(1) || Input.GetButton("Cancel_Joy_2"))
                {
                    DestroyGhost();

                    if (p2Controller)
                    {
                        //eventSystem.SetSelectedGameObject(previouslySelected);
                        placeEnabled = false;
                    }
                }
            }
        }
    }

    //Change x/z rotation based on player input
    private int trapRot = 0;
    private void UpdateRotationInput()
    {
        if (RaycastFromCam() != null)
        {
            RaycastHit hit = RaycastFromCam().Value;

            if (Input.GetButtonDown("RotateLeft_Joy_2"))
            {
                if (hit.normal.x == -1 || hit.normal.x == 1)
                {
                    trapRot--;
                }
                else
                {
                    trapRot++;
                }
            }
            else if (Input.GetButtonDown("RotateRight_Joy_2"))
            {
                if (hit.normal.x == -1 || hit.normal.x == 1)
                {
                    trapRot++;
                }
                else
                {
                    trapRot--;
                }
            }
        }
    }

    //Change y rotation of hit based on current side of tower
    private void FinalizeRotationInput()
    {
        if (RaycastFromCam() != null)
        {
            RaycastHit hit = RaycastFromCam().Value;

            if (hit.normal.x == -1 || hit.normal.x == 1)
            {
                ghostTrap.transform.rotation = Quaternion.Euler(ghostTrap.transform.rotation.x, 90, 90 * trapRot);
            }
            else
            {
                ghostTrap.transform.rotation = Quaternion.Euler(ghostTrap.transform.rotation.x, 0, 90 * trapRot);
            }
        }
    }

    private void DestroyGhost()
    {
        if(ghostTrap != null)
        {
            Destroy(ghostTrap);
            ghostTrap = null;
        }
    }

    private void OnClickTrap(int trapNum)
    {
        trap = trapPrefabs[trapNum];

        eventSystem.SetSelectedGameObject(null);
        StartCoroutine(EnableInput());
        DestroyGhost();
        SetGhost();
    }

    private void GetIndex(GameObject trap)
    {
        queueIndex = trap.GetComponent<ButtonIndex>().GetIndex();
    }

    private void SetSelectedButton(int trapNum)
    {
        eventSystem.SetSelectedGameObject(trapButtons[trapNum].gameObject);
    }

    private void CreateTrapQueue()
    {
        for(int i = 0; i < queueSize; i++)
        {
            int random = Random.Range(0, trapButtons.Length);
            GameObject newTrap = Instantiate(trapButtons[random], new Vector3 (-150f + 50f*i, 0f, 0), Quaternion.identity) as GameObject;
            newTrap.transform.SetParent(trapQueue.transform, false);

            if(i == 0)
            {
                eventSystem.firstSelectedGameObject = trapButtons[0].gameObject;
                eventSystem.SetSelectedGameObject(newTrap.gameObject);
            }

            //Add click listeners for all trap buttons
            newTrap.GetComponent<Button>().onClick.AddListener(() => OnClickTrap(random));
            newTrap.GetComponent<ButtonIndex>().ButtonIndexing(i);
            newTrap.GetComponent<Button>().onClick.AddListener(() => GetIndex(newTrap));

            queue.Add(newTrap);
        }

    }

    private void ClearTrapQueue()
    {
        for(int i = 0; i < queue.Count; i++)
        {
            Destroy(queue[i]);
        }
        queue.Clear();
    }

    private void ClearButton()
    { 
        queue[queueIndex].SetActive(false);
    }

    //Make player wait .5 seconds after pressing button to be able to place trap.
    //Gets rid of controller bug where pressing A to select a trap also immediately places it
    IEnumerator EnableInput()
    {
        yield return new WaitForSeconds(0.5f);
        placeEnabled = true;
    }

    private void SwitchQueue()
    {
        if (active == true)
        {
            trapQueue.transform.SetAsFirstSibling();
            trapQueue.transform.position += new Vector3(15f, 15f, 0);
            for (int i = 0; i < queue.Count; i++)
            {
                queue[i].GetComponent<Button>().interactable = false;
            }
        }

        if (active == false)
        {
            trapQueue.transform.SetAsLastSibling();
            trapQueue.transform.position -= new Vector3(15f, 15f, 0);
            for (int i = 0; i < queue.Count; i++)
            {
                queue[i].GetComponent<Button>().interactable = true;
            }
        }
        active = !active;
    }
}
