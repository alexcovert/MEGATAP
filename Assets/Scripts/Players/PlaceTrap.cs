using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public enum Direction
{
    Left,
    Right,
    Up,
    Down
}


public class PlaceTrap : MonoBehaviour {
    [Header("Design Values -------------")]
    [SerializeField] private int gridSize;
    [SerializeField] private int cursorDistFromCenter;

    [Header("Programmers - GameObjects/Scripts -----")]
    [SerializeField] private GameObject tower;

    [SerializeField] private GameObject[] trapButtons;
    [SerializeField] private TrapBase[] trapPrefabs;
    [SerializeField] private GameObject trapQueue;

    [SerializeField] private Image controllerCursor;

    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject gameManager;
    [SerializeField] private Camera cam;

    private CastSpell cs;
    private PauseMenu pause;
    private CheckControllers checkControllers;

    [Header("Audio-----------------------------")]
    [SerializeField] private AudioClip trapPlacementGood;
    [SerializeField] private AudioClip trapPlacementBad;
    private AudioSource audioSource;

    [Header("Queue Size -----")]
    [SerializeField] private int queueSize = 7;
    private MoveControllerCursor cursorMove;
    
    //Andy's Queue Stuff
    public List<GameObject> queue { get; private set; }
    private int queueIndex;
    [HideInInspector] public bool active { get; private set; }

    //Alex's Trap Stuff
    private TrapBase trap;
    public Direction CurrentDirection { get; private set; }
    private GameObject ghostTrap;
    private float gridXOffset, gridZOffset, gridYOffset = 0.35f; //changed when trap is rotated so that it still properly aligns with grid.
    private SpriteRenderer[] placementSquares;
    
    
    //Controller Stuff
    private bool p2Controller;
    private bool placeEnabled;




    private int numTimesRotated = 0;
    private bool resetEnabled = true;

	void Start () {
        //Get references
        pause = gameManager.GetComponent<PauseMenu>();
        cs = GetComponent<CastSpell>();
        audioSource = GetComponent<AudioSource>();

        //Queue Initialization
        queue = new List<GameObject>();
        cursorMove = GetComponent<MoveControllerCursor>();
        active = true;
        CreateTrapQueue();
        trapQueue.transform.SetAsLastSibling();

        //Handle cursor or set buttons if controller connected
        checkControllers = gameManager.GetComponent<CheckControllers>();
        p2Controller = checkControllers.GetControllerTwoState();
        placeEnabled = true;

        if (p2Controller)
        {
            eventSystem.SetSelectedGameObject(queue[0].gameObject);
        }
    }
	

	void Update () {
        //Move ghost with cursor
        MoveGhost();

        //Get controller select
        p2Controller = checkControllers.GetControllerTwoState();
        if (p2Controller && !pause.GameIsPaused)
        {
            if (Input.GetButtonDown("Place_Joy_2") && placeEnabled)
            {
                MoveGhost();
                SetTrap();
            }
        }
        //Reset queue's when tower rotates
        if (Input.GetButtonDown("Submit_Joy_2") && resetEnabled && !pause.GameIsPaused && numTimesRotated < 4 * (tower.GetComponentInChildren<NumberOfFloors>().NumFloors - 1) - 1)
        {
            //if(cam.GetComponent<CameraTwoRotator>().GetFloor() == tower.GetComponent<NumberOfFloors>().NumFloors && cam.GetComponent<CameraTwoRotator>().GetState() == 4)
            //{
            //    lastFace = true;
            //}
            numTimesRotated++;
            resetEnabled = false;
            StartCoroutine(EnableInput());

            DestroyGhost();
            ClearTrapQueue();
            CreateTrapQueue();
            eventSystem.SetSelectedGameObject(queue[0]);
            cursorMove.MovingTraps = true;
            controllerCursor.transform.localPosition = new Vector3(0, 130);
        }
    }

    //Returns cursor position on tower as a grid location rather than free-floating
    private Vector3? GetGridPosition()
    {
        if (RaycastFromCam() != null)
        {
            RaycastHit hit = RaycastFromCam().Value;
            float hitX = -1;
            float hitZ = -1;
            switch (cam.GetComponent<CameraTwoRotator>().GetState())
            {
                case 1:
                    hitX = Mathf.RoundToInt((hit.point.x - 1) / gridSize) * gridSize + 1 + gridXOffset;
                    hitZ = Mathf.RoundToInt(hit.point.z + -2);
                    break;
                case 2:
                    hitX = Mathf.RoundToInt(hit.point.x + 2);
                    hitZ = Mathf.RoundToInt((hit.point.z - 1) / gridSize) * gridSize + 1 + gridZOffset;
                    break;
                case 3:
                    hitX = Mathf.RoundToInt((hit.point.x - 1) / gridSize) * gridSize + 1 + gridXOffset;
                    hitZ = Mathf.RoundToInt(hit.point.z + 2);
                    break;
                case 4:
                    hitX = Mathf.RoundToInt(hit.point.x + -2);
                    hitZ = Mathf.RoundToInt((hit.point.z - 1) / gridSize) * gridSize + 1 + gridZOffset;
                    break;
            }
            float hitY = Mathf.RoundToInt((hit.point.y - 1)/ gridSize) * gridSize + gridYOffset;
            return new Vector3(hitX, hitY, hitZ);
        }
        else return null;
    }

    //Raycast from the camera to tower
    private RaycastHit? RaycastFromCam()
    {
        RaycastHit hit;
        Ray ray;
        //Ray to controller cursor
        if (p2Controller && controllerCursor.transform.position.y > Screen.height / 2)
        {
            ray = cam.ScreenPointToRay(controllerCursor.transform.position);
            if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("Tower")))
            {
                return hit;
            }
            else return null;
        }
        //Ray to mouse cursor
        else if(Input.mousePosition.y > Screen.height / 2)
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("Tower")))
            {
                return hit;
            }
            else return null;
        }
        else
        {
            return null;
        }
    }

    //Called from custom event trigger script on floor prefabs column when it is clicked on w/ computer mouse
    //ONLY computer mouse - controller cursor is handled in Update
    public void OnClickTower()
    {
        if(!Input.GetMouseButtonUp(1) && !pause.GameIsPaused)
        {
            SetTrap();
        }
    }

    private void SetTrap()
    {
        if(ghostTrap != null)
        {
            //Check if trap is on correct surface
            bool validLocation;
            CheckMultipleBases bases = ghostTrap.GetComponentInChildren<CheckMultipleBases>();
            CheckValidLocations check = ghostTrap.GetComponentInChildren<CheckValidLocations>();

            if (bases != null)
            {
                validLocation = bases.Valid;
            }
            else if (check != null)
            {
                validLocation = check.Valid;
            }
            else
            {
                validLocation = true;
                Debug.Log("Warning: Trap not set up correctly; valid location is always true.");
            }

            //CheckNearby() also checks the collider provided for the "safe zone" around the trap
            if (GetGridPosition() != null && CheckNearby() && validLocation)
            {
                Vector3 position = GetGridPosition().Value;
                if (ghostTrap != null && CheckFloor(position.y))
                {
                    audioSource.PlayOneShot(trapPlacementGood);
                    trap.InstantiateTrap(position, ghostTrap.transform.rotation);
                    if (check != null) check.Placed = true;
                    //if (bases != null) bases.Placed = true;
                    ClearButton();
                    trap = null;
                    foreach (SpriteRenderer sr in placementSquares)
                    {
                        sr.enabled = false;
                    }
                    placementSquares = null;
                    DestroyGhost();

                    SetSelectedButton();
                }
                else
                {
                    audioSource.PlayOneShot(trapPlacementBad);
                }
            }
            else
            {
                //audioSource.PlayOneShot(trapPlacementBad);
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
            placementSquares = ghostTrap.GetComponentInChildren<Canvas>().gameObject.GetComponentsInChildren<SpriteRenderer>();
        }
        
        
        Destroy(ghostTrap.GetComponent<Collider>());
        
        //Delete spikes script so animation doesn't play
        if(ghostTrap.GetComponentInChildren<Spikes>() != null)
        {
            Destroy(ghostTrap.GetComponent<Spikes>());
        }
        //Make half transparent------------------------------------------------
        //Check for both mesh renderer and skinned mesh renderers
        MeshRenderer[] mrs = ghostTrap.GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] smrs = ghostTrap.GetComponentsInChildren<SkinnedMeshRenderer>();
        //each mr can also have multiple materials
        List<Material> mats = new List<Material>();

        foreach (MeshRenderer mr in mrs)
        {
            mr.GetMaterials(mats);
            foreach(Material mat in mats)
            {
                Color color = mat.color;
                color.a = 0.5f;
                mat.color = color;
            }
        }

        foreach (SkinnedMeshRenderer smr in smrs)
        {
            smr.GetMaterials(mats);
            foreach (Material mat in mats)
            {
                Color color = mat.color;
                color.a = 0.5f;
                mat.color = color;
            }
        }
        ghostTrap.GetComponent<TrapBase>().enabled = false;

    }

    private void MoveGhost()
    {
        if (ghostTrap != null)
        {
            UpdateRotationInput();
            FinalizeRotationInput();
            bool validLocation;
            CheckMultipleBases bases = ghostTrap.GetComponentInChildren<CheckMultipleBases>();
            CheckValidLocations check = ghostTrap.GetComponentInChildren<CheckValidLocations>();

            if (bases != null)
            {
                validLocation = bases.Valid;
            }
            else if (check != null)
            {
                validLocation = check.Valid;
            }
            else
            {
                validLocation = true;
                Debug.Log("Warning: Trap not set up correctly; valid location is always true.");
            }

            if(validLocation && CheckNearby() && GetGridPosition() != null && placementSquares.Length == 2)
            {
                placementSquares[0].enabled = false;
                placementSquares[1].enabled = true;
            }
            else if (placementSquares.Length == 2)
            { 
                placementSquares[0].enabled = true;
                placementSquares[1].enabled = false;
            }
            if (GetGridPosition() != null)
            {
                //Rotate trap based on side of tower
                switch (cam.GetComponent<CameraTwoRotator>().GetState())
                {
                    case 1:
                        ghostTrap.transform.eulerAngles = new Vector3(ghostTrap.transform.eulerAngles.x, 0, ghostTrap.transform.eulerAngles.z);
                        break;
                    case 2:
                        ghostTrap.transform.eulerAngles = new Vector3(ghostTrap.transform.eulerAngles.x, 270, ghostTrap.transform.eulerAngles.z);
                        break;
                    case 3:
                        ghostTrap.transform.eulerAngles = new Vector3(ghostTrap.transform.eulerAngles.x, 180, ghostTrap.transform.eulerAngles.z);
                        break;
                    case 4:
                        ghostTrap.transform.eulerAngles = new Vector3(ghostTrap.transform.eulerAngles.x, 90, ghostTrap.transform.eulerAngles.z);
                        break;
                }
                Vector3 position = GetGridPosition().Value;
                ghostTrap.transform.position = position;

                //Cancel the trap
                if ((Input.GetMouseButton(1) || Input.GetButton("Cancel_Joy_2")) && !pause.GameIsPaused)
                {
                    DestroyGhost();
                    placementSquares = null;
                    SetSelectedButton();
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

            //Commented out while we are not doing trap rotation - KEEP for later
            //if (Input.GetButtonDown("RotateLeft_Joy_2") && !pause.GameIsPaused)
            //{
            //    if (hit.normal.x == -1 || hit.normal.x == 1)
            //    {
            //        trapRot--;
            //    }
            //    else
            //    {
            //        trapRot++;
            //    }
            //}
            //else if (Input.GetButtonDown("RotateRight_Joy_2") && !pause.GameIsPaused)
            //{
            //    if (hit.normal.x == -1 || hit.normal.x == 1)
            //    {
            //        trapRot++;
            //    }
            //    else
            //    {
            //        trapRot--;
            //    }
            //}

            //Add Offsets so they still stick to grid
            if(trapRot % 4 == 0)
            {//Facing Up
                CurrentDirection = Direction.Up;
                gridYOffset = 0.35f;
                gridXOffset = 0;
                gridZOffset = 0;
            }
            else if((trapRot - 1) % 4 == 0)
            {//Facing Left
                CurrentDirection = Direction.Left;
                gridYOffset = 1;
                switch(cam.GetComponent<CameraTwoRotator>().GetState())
                {
                    case 1:
                    case 3:
                        gridXOffset = 0.6f;
                        gridZOffset = 0;
                        break;
                    case 2:
                    case 4:
                        gridXOffset = 0;
                        gridZOffset = -0.6f;
                        break;
                }
            }
            else if((trapRot - 2) % 4 == 0)
            {//Facing Down
                CurrentDirection = Direction.Down;
                gridYOffset = 1.7f;
                gridXOffset = 0;
                gridZOffset = 0;
            }
            else if((trapRot - 3) % 4 == 0)
            {//Facing Right
                CurrentDirection = Direction.Right;
                gridYOffset = 1;
                switch (cam.GetComponent<CameraTwoRotator>().GetState())
                {
                    case 1:
                    case 3:
                        gridXOffset = -0.6f;
                        gridZOffset = 0;
                        break;
                    case 2:
                    case 4:
                        gridXOffset = 0;
                        gridZOffset = 0.6f;
                        break;
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

    public void DestroyGhost()
    {
        if(ghostTrap != null)
        {
            Destroy(ghostTrap);
            ghostTrap = null;
        }
    }

    //Called from trap button / CallClick script
    public void OnClickTrap(int trapNum)
    {
        trap = trapPrefabs[trapNum];
        trapRot = 0;
//        eventSystem.SetSelectedGameObject(null);
        StartCoroutine(EnableInput());
        DestroyGhost();
        GetComponent<CastSpell>().DestroyTarget();
        SetGhost();
    }


    //Mostly for controller - wait between inputs to prevent spamming and some button selection bugs
    private IEnumerator EnableInput()
    {
        yield return new WaitForSeconds(0.5f);
        resetEnabled = true;
    }



    /// --------------------------------------------------------
    /// QUEUE/ BUTTON STUFF
    /// --------------------------------------------------------
    private void GetIndex(GameObject trap)
    {
        queueIndex = trap.GetComponent<ButtonIndex>().GetIndex();
    }

    private void CreateTrapQueue()
    {
        trapRot = 0;
        for(int i = 0; i < queueSize; i++)
        {
            int random = Random.Range(0, trapButtons.Length);
            GameObject newTrap = Instantiate(trapButtons[random], new Vector3 (-80f + 40f*i, -11f, 0), Quaternion.identity) as GameObject;
            newTrap.transform.SetParent(trapQueue.transform, false);

            //Add click listeners for all trap buttons
            newTrap.GetComponent<Button>().onClick.AddListener(() => OnClickTrap(random));
            newTrap.GetComponent<ButtonIndex>().ButtonIndexing(i);
            newTrap.GetComponent<Button>().onClick.AddListener(() => GetIndex(newTrap));

            queue.Add(newTrap);

            if (active == false)
            {
                queue[i].GetComponent<Button>().interactable = false;
            }
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

    //Set new selected button if the controller is being used.
    private void SetSelectedButton()
    {
        if (p2Controller)
        {
            if (active)
            {
                bool buttonSet = false;
                for (int i = 0; i < queue.Count; i++)
                {
                    if (queue[i].activeInHierarchy && !buttonSet)
                    {
                        eventSystem.SetSelectedGameObject(queue[i]);
                        buttonSet = true;
                    }
                }
                if (!buttonSet)
                {
                    for (int i = 0; i < cs.queue.Length; i++)
                    {
                        if (cs.queue[i] != null && cs.queue[i].activeInHierarchy && !buttonSet)
                        {
                            controllerCursor.transform.localPosition = new Vector3(0, -100);
                            eventSystem.SetSelectedGameObject(cs.queue[i]);
                            buttonSet = true;
                        }
                    }
                }

            }
            //placeEnabled = false;
        }
    }

    //private void SwitchQueue()
    //{
    //    if (active == true)
    //    {
    //        trapQueue.transform.SetAsFirstSibling();
    //        trapQueue.transform.position += new Vector3(15f, 15f, 0);
    //        for (int i = 0; i < queue.Count; i++)
    //        {
    //            queue[i].GetComponent<Button>().interactable = false;
    //        }
    //    }

    //    if (active == false)
    //    {
    //        controllerCursor.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 + cursorDistFromCenter, 0);
    //        GetComponent<MoveControllerCursor>().MovingTraps = true;
    //        bool buttonSet = false;
    //        trapQueue.transform.SetAsLastSibling();
    //        trapQueue.transform.position -= new Vector3(15f, 15f, 0);
    //        for (int i = 0; i < queue.Count; i++)
    //        {
    //            queue[i].GetComponent<Button>().interactable = true;
    //            if (queue[i].activeInHierarchy && !buttonSet)
    //            {
    //                eventSystem.SetSelectedGameObject(queue[i]);
    //                buttonSet = true;
    //            }
    //        }
    //    }
    //    active = !active;
    //}
}
