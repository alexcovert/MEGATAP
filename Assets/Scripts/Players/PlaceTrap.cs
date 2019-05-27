using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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

    [Header("Percentage is X/100 currently.")]
    [SerializeField] private int CommonRarityChance = 50;
    [SerializeField] private int UncommonRarityChance = 35;
    [SerializeField] private int RareRarityChance = 15;

    [Header("Programmers - GameObjects/Scripts -----")]
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject trapQueue;
    [SerializeField] private Image controllerCursor;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject gameManager;
    [SerializeField] private Camera cam;

    private CastSpell cs;
    private PauseMenu pause;
    private CheckControllers checkControllers;

    [Header("Trap Buttons & Prefabs")]
    [SerializeField] private GameObject[] CommonTrapButtons;
    [SerializeField] private GameObject[] UncommonTrapButtons;
    [SerializeField] private GameObject[] RareTrapButtons;

    [SerializeField] private TrapBase[] CommonTrapPrefabs;
    [SerializeField] private TrapBase[] UncommonTrapPrefabs;
    [SerializeField] private TrapBase[] RareTrapPrefabs;

    [Header("Tutorial Traps")]
    [SerializeField] private GameObject[] tutorialTrapButtons;
    [SerializeField] private TrapBase[] tutorialTrapPrefabs;
    private bool tutorial;

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
    private float gridXOffset, gridZOffset, gridYOffset = 1f; //changed when trap is rotated so that it still properly aligns with grid.
    private SpriteRenderer[] placementSquares;


    //Controller Stuff
    private bool p2Controller;
    public bool InputEnabled = true;
    private bool resetEnabled = true;
    private int previouslySelectedIndex;
    private InputManager inputManager;

    private int numTimesRotated = 0;
    private bool atTop = false;

    // tutorial tips
    GameObject tutorialTopGoal;
    GameObject tutorialTopPlaceTrap;
    GameObject tutorialTopSelectTrap;
    GameObject tutorialTopMoveTrap;
    GameObject tutorialTopRotate;
    GameObject tutorialTopY;
    GameObject tutorialTopSpells;
    bool tmpIndicator = false;

    private void Awake()
    {
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
    }

    void Start() {
        //Get references
        pause = gameManager.GetComponent<PauseMenu>();
        cs = GetComponent<CastSpell>();
        audioSource = GetComponent<AudioSource>();

        //Queue Initialization
        queue = new List<GameObject>();
        cursorMove = GetComponent<MoveControllerCursor>();
        active = true;

        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            tutorial = true;
            CreateTutorialQueue(true);
        }
        else
        {
            CreateTrapQueue();
        }
        trapQueue.transform.SetAsLastSibling();

        //Handle cursor or set buttons if controller connected
        checkControllers = inputManager.GetComponent<CheckControllers>();
        p2Controller = checkControllers.GetTopPlayerControllerState();

        if (p2Controller)
        {
            eventSystem.SetSelectedGameObject(queue[0].gameObject);
            queue[0].gameObject.GetComponent<Button>().Select();
        }

        // define tutorial tips
        tutorialTopGoal = GameObject.Find("ToolTipTopGoal");
        tutorialTopPlaceTrap = GameObject.Find("ToolTipTopPlaceTrap");
        tutorialTopMoveTrap = GameObject.Find("ToolTipTopMoveTrap");
        tutorialTopSelectTrap = GameObject.Find("ToolTipTopSelectTrap");
        tutorialTopRotate = GameObject.Find("ToolTipTopRotate");
        tutorialTopY = GameObject.Find("ToolTipTopY");
        tutorialTopSpells = GameObject.Find("ToolTipTopSpells");

        if (tutorialTopGoal != null) { tutorialTopGoal.SetActive(true); }
        if (tutorialTopPlaceTrap != null) { tutorialTopPlaceTrap.SetActive(false); }
        if (tutorialTopMoveTrap != null) { tutorialTopMoveTrap.SetActive(false); }
        if (tutorialTopSelectTrap != null) { tutorialTopSelectTrap.SetActive(false); }
        if (tutorialTopRotate != null) { tutorialTopRotate.SetActive(false); }
        if (tutorialTopY != null) { tutorialTopY.SetActive(false); }
        if (tutorialTopSpells != null) { tutorialTopSpells.SetActive(false); }
    }


    void Update() {

        // tutorial tips
        if (tutorialTopGoal != null && tutorialTopGoal.activeSelf == true && (inputManager.GetButtonDown(InputCommand.TopPlayerSelect) || Input.GetMouseButtonDown(0)))
        {
            tutorialTopGoal.SetActive(false);
            tutorialTopSelectTrap.SetActive(true);
        } else if (tutorialTopSelectTrap != null && tutorialTopSelectTrap.activeSelf == true && (inputManager.GetButtonDown(InputCommand.TopPlayerMenu) || Input.GetMouseButtonDown(0)))
        {
            tutorialTopSelectTrap.SetActive(false);
            tutorialTopMoveTrap.SetActive(true);
            tutorialTopPlaceTrap.SetActive(true);
        } else if (trapQueueIsEmpty() && tutorialTopPlaceTrap.activeSelf == true)
        {
            tutorialTopPlaceTrap.SetActive(false);
            tutorialTopMoveTrap.SetActive(false);
            tutorialTopRotate.SetActive(true);
        } else if (tutorialTopRotate.activeSelf == true && inputManager.GetButtonDown(InputCommand.TopPlayerRotate))
        {
            tutorialTopRotate.SetActive(false);
            tutorialTopY.SetActive(true);
        } else if (tutorialTopY.activeSelf == true && (inputManager.GetButtonDown(InputCommand.TopPlayerSelect) || Input.GetMouseButton(0)))
        {
            tutorialTopY.SetActive(false);
            tmpIndicator = true;
        } else if (tmpIndicator == true && inputManager.GetButtonDown(InputCommand.TopPlayerRotate))
        {
            tutorialTopSpells.SetActive(true);
            tmpIndicator = false;
        } else if (tutorialTopSpells.activeSelf == true && inputManager.GetButtonDown(InputCommand.TopPlayerRotate))
        {
            tutorialTopSpells.SetActive(false);
        }


        //Move ghost with cursor
        MoveGhost();
        //Get controller select
        p2Controller = checkControllers.GetTopPlayerControllerState();
        if (p2Controller && !pause.GameIsPaused)
        {
            if (inputManager.GetButtonDown(InputCommand.TopPlayerSelect) && InputEnabled)
            {
                MoveGhost();
                SetTrap();
            }
        }
        //Reset queue's when tower rotates
        if (inputManager.GetButtonDown(InputCommand.TopPlayerRotate) && resetEnabled && !pause.GameIsPaused && numTimesRotated < 4 * (tower.GetComponentInChildren<NumberOfFloors>().NumFloors - 1) - 1)
        {

            numTimesRotated++;
            resetEnabled = false;
            StartCoroutine(EnableInput());

            DestroyGhost();
            ClearTrapQueue();
            if (tutorial)
            {
                CreateTutorialQueue(false);
            }
            else
            {
                CreateTrapQueue();
            }
            if (p2Controller) eventSystem.SetSelectedGameObject(queue[0]);
            cursorMove.MovingTraps = true;
            controllerCursor.transform.localPosition = new Vector3(-1, -1, 0);
        }

        //Cancel trap
        if (Input.GetMouseButtonDown(1) && ghostTrap != null && !checkControllers.topPlayersController)
        {
            DestroyGhost();
        }

    }

    //Returns cursor position on tower as a grid location rather than free-floating
    private Vector3? GetGridPosition()
    {

        if (RaycastFromCam() != null && !checkControllers.topPlayersController)
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
            float hitY = Mathf.RoundToInt((hit.point.y - 1) / gridSize) * gridSize + gridYOffset;
            return new Vector3(hitX, hitY, hitZ);
        }
        else
        {
            Vector3 gridPos;

            switch (cam.GetComponent<CameraTwoRotator>().GetState())
            {
                case 1:
                    gridPos = controllerCursor.transform.position + new Vector3(0, 0, -2f);
                    break;
                case 2:
                    gridPos = controllerCursor.transform.position + new Vector3(2f, 0, 0);
                    break;
                case 3:
                    gridPos = controllerCursor.transform.position + new Vector3(0, 0, 2f);
                    break;
                case 4:
                    gridPos = controllerCursor.transform.position + new Vector3(-2f, 0, 0);
                    break;

                default:
                    gridPos = Vector3.zero;
                    break;

            }

            return gridPos;
        }
    }

    private int trapRot = 0;
    private void UpdateRotationInput()
    {
        if (RaycastFromCam() != null)
        {
            RaycastHit hit = RaycastFromCam().Value;
            //Add Offsets so they still stick to grid
            if (trapRot % 4 == 0)
            {//Facing Up
                CurrentDirection = Direction.Up;
                gridYOffset = 0.35f;
                gridXOffset = 0;
                gridZOffset = 0;
            }
            else if ((trapRot - 1) % 4 == 0)
            {//Facing Left
                CurrentDirection = Direction.Left;
                gridYOffset = 1;
                switch (cam.GetComponent<CameraTwoRotator>().GetState())
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
            else if ((trapRot - 2) % 4 == 0)
            {//Facing Down
                CurrentDirection = Direction.Down;
                gridYOffset = 1.7f;
                gridXOffset = 0;
                gridZOffset = 0;
            }
            else if ((trapRot - 3) % 4 == 0)
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
        else if (Input.mousePosition.y > Screen.height / 2)
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
        if (!Input.GetMouseButtonUp(1) && !pause.GameIsPaused && !p2Controller)
        {
            SetTrap();
        }
    }

    private void SetTrap()
    {
        if (ghostTrap != null)
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
            if (CheckNearby() && validLocation)
            {
                Vector3 position = GetGridPosition().Value;
                if (ghostTrap != null && CheckFloor(position.y))
                {
                    audioSource.PlayOneShot(trapPlacementGood);
                    GameObject finalTrap = trap.InstantiateTrap(position, ghostTrap.transform.rotation);

                    //Destroy scripts that use OnTriggerStay to reduce lagz
                    TrapOverlap trapOverlap = finalTrap.GetComponentInChildren<TrapOverlap>();
                    CheckMultipleBases multipleBases = finalTrap.GetComponentInChildren<CheckMultipleBases>();
                    if (trapOverlap != null)
                        Destroy(trapOverlap);
                    if (multipleBases != null)
                        Destroy(multipleBases);


                    if (check != null) check.Placed = true;
                    previouslySelectedIndex = queueIndex;

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
                audioSource.PlayOneShot(trapPlacementBad);
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

            if (ghostTrap.GetComponentInChildren<TrapOnlyChecking>() != null && ghostTrap.GetComponentInChildren<TrapOnlyChecking>().nearbyTrap)
            {
                return false;
            }

            return true;
        }

        return false;
    }

    private bool CheckClickOnPlatform()
    {
        RaycastHit hit;
        Ray ray;

        //Ray to mouse cursor
        if (Input.mousePosition.y > Screen.height / 2)
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, float.MaxValue, ~LayerMask.GetMask("Ignore Raycast")))
            {
                if (hit.transform.tag == "Platform")
                {
                    return false;
                }
            }
            else return true;
        }

        return true;
    }
    private void SetGhost()
    {
        if (active)
        {
            if (trap != null)
            {
                ghostTrap = trap.InstantiateTrap(Vector3.zero);
                placementSquares = ghostTrap.GetComponentInChildren<Canvas>().gameObject.GetComponentsInChildren<SpriteRenderer>();
            }


            Destroy(ghostTrap.GetComponent<Collider>());

            //Delete spikes script so animation doesn't play
            if (ghostTrap.GetComponentInChildren<Spikes>() != null)
            {
                Destroy(ghostTrap.GetComponent<Spikes>());
            }
            //Delete projectile shooter so it doesn't shoot
            if (ghostTrap.GetComponentInChildren<ProjectileShooter>() != null)
            {
                Destroy(ghostTrap.GetComponentInChildren<ProjectileShooter>());
                Destroy(ghostTrap.GetComponentInChildren<Animator>());
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
                foreach (Material mat in mats)
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
    }

    private void MoveGhost()
    {
        if (ghostTrap != null)
        {
            //UpdateRotationInput();
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

            if (validLocation && CheckNearby() && placementSquares.Length == 2)
            {
                if (placementSquares[0] != null) placementSquares[0].enabled = false;
                if (placementSquares[1] != null) placementSquares[1].enabled = true;
            }
            else if (placementSquares.Length == 2)
            {
                if (placementSquares[0] != null) placementSquares[0].enabled = true;
                if (placementSquares[1] != null) placementSquares[1].enabled = false;
            }
            if (GetGridPosition() != Vector3.zero)
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
                ghostTrap.transform.rotation = Quaternion.Euler(ghostTrap.transform.rotation.x, 90, 0);
            }
            else
            {
                ghostTrap.transform.rotation = Quaternion.Euler(ghostTrap.transform.rotation.x, 0, 0);
            }
        }
    }

    public void DestroyGhost()
    {
        if (ghostTrap != null)
        {
            Destroy(ghostTrap);
            ghostTrap = null;
        }
    }

    //Called from trap button / CallClick script
    public void OnClickTrapCommon(int trapNum)
    {
        trap = CommonTrapPrefabs[trapNum];

        StartCoroutine(EnableInput());
        DestroyGhost();
        GetComponent<CastSpell>().DestroyTarget();
        SetGhost();
    }

    public void OnClickTrapUncommon(int trapNum)
    {
        trap = UncommonTrapPrefabs[trapNum];

        StartCoroutine(EnableInput());
        DestroyGhost();
        GetComponent<CastSpell>().DestroyTarget();
        SetGhost();
    }

    public void OnClickTrapRare(int trapNum)
    {
        trap = RareTrapPrefabs[trapNum];

        StartCoroutine(EnableInput());
        DestroyGhost();
        GetComponent<CastSpell>().DestroyTarget();
        SetGhost();
    }

    public void OnClickTrapTutorial(int trapNum)
    {
        trap = tutorialTrapPrefabs[trapNum];

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

    //Called from pause script to re-enable input after pressing "Resume"
    public IEnumerator ResumeInput()
    {
        yield return new WaitForSeconds(0.5f);
        InputEnabled = true;
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
        active = true;
        for (int i = 0; i < queueSize; i++)
        {
            int TrapChance = Random.Range(1, 100);

            int randomIndex;
            GameObject newTrap;

            if (TrapChance <= CommonRarityChance)
            {
                randomIndex = Random.Range(0, CommonTrapButtons.Length);
                newTrap = Instantiate(CommonTrapButtons[randomIndex], new Vector3(-230f + 40f * i, -30, 0), Quaternion.identity) as GameObject;

                newTrap.transform.SetParent(trapQueue.transform, false);

                //Add click listeners for all trap buttons
                newTrap.GetComponent<Button>().onClick.AddListener(() => OnClickTrapCommon(randomIndex));
                newTrap.GetComponent<ButtonIndex>().ButtonIndexing(i);
                newTrap.GetComponent<Button>().onClick.AddListener(() => GetIndex(newTrap));

                queue.Add(newTrap);
            }
            else if (TrapChance > CommonRarityChance && TrapChance < (100 - RareRarityChance))
            {
                randomIndex = Random.Range(0, UncommonTrapButtons.Length);
                newTrap = Instantiate(UncommonTrapButtons[randomIndex], new Vector3(-230f + 40f * i, -30, 0), Quaternion.identity) as GameObject;

                newTrap.transform.SetParent(trapQueue.transform, false);

                //Add click listeners for all trap buttons
                newTrap.GetComponent<Button>().onClick.AddListener(() => OnClickTrapUncommon(randomIndex));
                newTrap.GetComponent<ButtonIndex>().ButtonIndexing(i);
                newTrap.GetComponent<Button>().onClick.AddListener(() => GetIndex(newTrap));

                queue.Add(newTrap);
            }
            else if (TrapChance >= (CommonRarityChance + UncommonRarityChance))
            {
                randomIndex = Random.Range(0, RareTrapButtons.Length);
                newTrap = Instantiate(RareTrapButtons[randomIndex], new Vector3(-230f + 40f * i, -30, 0), Quaternion.identity) as GameObject;

                newTrap.transform.SetParent(trapQueue.transform, false);

                //Add click listeners for all trap buttons
                newTrap.GetComponent<Button>().onClick.AddListener(() => OnClickTrapRare(randomIndex));
                newTrap.GetComponent<ButtonIndex>().ButtonIndexing(i);
                newTrap.GetComponent<Button>().onClick.AddListener(() => GetIndex(newTrap));

                queue.Add(newTrap);
            }
            else
            {
                Debug.Log("Error. You weren't supposed to reach here.");
            }

            if (active == false)
            {
                queue[i].GetComponent<Button>().interactable = false;
            }
        }

    }

    //Called in start, from tutorial
    //Creates # of traps specified
    private void CreateTutorialQueue(bool first)
    {
        if (first)
        {
            for (int i = 0; i < tutorialTrapButtons.Length; i++)
            {
                GameObject newTrap = Instantiate(tutorialTrapButtons[i], new Vector3(-230f + 40f * i, -30, 0), Quaternion.identity) as GameObject;
                newTrap.transform.SetParent(trapQueue.transform, false);
                int counter = i;
                //Add click listeners for all trap buttons
                newTrap.GetComponent<Button>().onClick.AddListener(() => OnClickTrapTutorial(counter));
                newTrap.GetComponent<ButtonIndex>().ButtonIndexing(i);
                newTrap.GetComponent<Button>().onClick.AddListener(() => GetIndex(newTrap));
                
                queue.Add(newTrap);
            }
        }
        else
        {
            active = true;
            int trapCounter = 0; 

            GenerateCommon(ref trapCounter);
            GenerateUncommon(ref trapCounter);
            GenerateRare(ref trapCounter);
           

            while(trapCounter < queueSize)
            {
                int TrapChance = Random.Range(1, 100);
                int randomIndex;
                GameObject newTrap;

                if (TrapChance <= CommonRarityChance)
                {
                    randomIndex = Random.Range(0, CommonTrapButtons.Length);
                    newTrap = Instantiate(CommonTrapButtons[randomIndex], new Vector3(-230f + 40f * trapCounter, -30, 0), Quaternion.identity) as GameObject;

                    newTrap.transform.SetParent(trapQueue.transform, false);

                    //Add click listeners for all trap buttons
                    newTrap.GetComponent<Button>().onClick.AddListener(() => OnClickTrapCommon(randomIndex));
                    newTrap.GetComponent<ButtonIndex>().ButtonIndexing(trapCounter);
                    newTrap.GetComponent<Button>().onClick.AddListener(() => GetIndex(newTrap));

                    queue.Add(newTrap);
                }
                else if (TrapChance > CommonRarityChance && TrapChance < (100 - RareRarityChance))
                {
                    randomIndex = Random.Range(0, UncommonTrapButtons.Length);
                    newTrap = Instantiate(UncommonTrapButtons[randomIndex], new Vector3(-230f + 40f * trapCounter, -30, 0), Quaternion.identity) as GameObject;

                    newTrap.transform.SetParent(trapQueue.transform, false);

                    //Add click listeners for all trap buttons
                    newTrap.GetComponent<Button>().onClick.AddListener(() => OnClickTrapUncommon(randomIndex));
                    newTrap.GetComponent<ButtonIndex>().ButtonIndexing(trapCounter);
                    newTrap.GetComponent<Button>().onClick.AddListener(() => GetIndex(newTrap));

                    queue.Add(newTrap);
                }
                else if (TrapChance >= (CommonRarityChance + UncommonRarityChance))
                {
                    randomIndex = Random.Range(0, RareTrapButtons.Length);
                    newTrap = Instantiate(RareTrapButtons[randomIndex], new Vector3(-230f + 40f * trapCounter, -30, 0), Quaternion.identity) as GameObject;

                    newTrap.transform.SetParent(trapQueue.transform, false);

                    //Add click listeners for all trap buttons
                    newTrap.GetComponent<Button>().onClick.AddListener(() => OnClickTrapRare(randomIndex));
                    newTrap.GetComponent<ButtonIndex>().ButtonIndexing(trapCounter);
                    newTrap.GetComponent<Button>().onClick.AddListener(() => GetIndex(newTrap));

                    queue.Add(newTrap);
                }

                trapCounter++;
            }
        }
    }

    private void GenerateCommon(ref int trapCounter)
    {
        for (int i = 0; i < CommonTrapButtons.Length; i++)
        {
            if (trapCounter < queueSize)
            {
                int randomIndex = Random.Range(0, CommonTrapButtons.Length);
                GameObject newTrap = Instantiate(CommonTrapButtons[randomIndex], new Vector3(-230f + 40f * trapCounter, -30, 0), Quaternion.identity) as GameObject;

                newTrap.transform.SetParent(trapQueue.transform, false);

                //Add click listeners for all trap buttons
                newTrap.GetComponent<Button>().onClick.AddListener(() => OnClickTrapCommon(randomIndex));
                newTrap.GetComponent<ButtonIndex>().ButtonIndexing(trapCounter);
                newTrap.GetComponent<Button>().onClick.AddListener(() => GetIndex(newTrap));

                queue.Add(newTrap);
                trapCounter++;
            }
        }
    }

    private void GenerateUncommon(ref int trapCounter)
    {
        //One of each uncommon
        for (int i = 0; i < UncommonTrapButtons.Length; i++)
        {
            if (trapCounter < queueSize)
            {
                int randomIndex = Random.Range(0, UncommonTrapButtons.Length);
                GameObject newTrap = Instantiate(UncommonTrapButtons[randomIndex], new Vector3(-230f + 40f * trapCounter, -30, 0), Quaternion.identity) as GameObject;

                newTrap.transform.SetParent(trapQueue.transform, false);

                //Add click listeners for all trap buttons
                newTrap.GetComponent<Button>().onClick.AddListener(() => OnClickTrapUncommon(randomIndex));
                newTrap.GetComponent<ButtonIndex>().ButtonIndexing(trapCounter);
                newTrap.GetComponent<Button>().onClick.AddListener(() => GetIndex(newTrap));

                queue.Add(newTrap);
                trapCounter++;
            }
        }
    }

    private void GenerateRare(ref int trapCounter)
    {
        //One of each rare
        for (int i = 0; i < RareTrapButtons.Length; i++)
        {
            if (trapCounter < queueSize)
            {
                int randomIndex = Random.Range(0, RareTrapButtons.Length);
                GameObject newTrap = Instantiate(RareTrapButtons[randomIndex], new Vector3(-230f + 40f * trapCounter, -30, 0), Quaternion.identity) as GameObject;

                newTrap.transform.SetParent(trapQueue.transform, false);

                //Add click listeners for all trap buttons
                newTrap.GetComponent<Button>().onClick.AddListener(() => OnClickTrapRare(randomIndex));
                newTrap.GetComponent<ButtonIndex>().ButtonIndexing(trapCounter);
                newTrap.GetComponent<Button>().onClick.AddListener(() => GetIndex(newTrap));

                queue.Add(newTrap);
                trapCounter++;
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
        //queue[queueIndex].SetActive(false);
        queue[queueIndex].GetComponent<Button>().interactable = false;

        bool allUsed = true;
        for (int i = 0; i < queue.Count; i++)
        {
            if (queue[i].GetComponent<Button>().interactable)
            {
                allUsed = false;
            }
        }

        if(allUsed)
        {
            active = false;
            SetSelectedButton();

            DestroyGhost();
        }
    }

    //Set new selected button if the controller is being used.
    private void SetSelectedButton()
    {
        if (p2Controller)
        {
            if (active)
            {
                bool buttonSet = false;
                //Loop over rest of trap queue
                for (int i = previouslySelectedIndex; i < queue.Count; i++)
                {
                    if (queue[i].activeInHierarchy && !buttonSet && queue[i].GetComponent<Button>().interactable)
                    {
                        eventSystem.SetSelectedGameObject(queue[i]);
                        buttonSet = true;
                    }
                }

                //Loop over previous trap queue
                if(!buttonSet)
                {
                    for (int i = previouslySelectedIndex; i >= 0; i--)
                    {
                        if (queue[i].activeInHierarchy && !buttonSet && queue[i].GetComponent<Button>().interactable)
                        {
                            eventSystem.SetSelectedGameObject(queue[i]);
                            buttonSet = true;
                        }
                    }
                }
                //Loop over spells to see if anything available
                if (!buttonSet)
                {
                    for (int i = 0; i < cs.queue.Length; i++)
                    {
                        if (cs.queue[i] != null && cs.queue[i].GetComponent<Button>().interactable && cs.queue[i].activeInHierarchy && !buttonSet)
                        {
                            controllerCursor.transform.localPosition = new Vector3(0, -100);
                            eventSystem.SetSelectedGameObject(cs.queue[i]);
                            buttonSet = true;
                        }
                    }
                }

                if(!buttonSet)
                {
                    DestroyGhost();
                    eventSystem.SetSelectedGameObject(null);
                }

            }
        }
    }

    //Getter
    public bool LastFace()
    {
        if (numTimesRotated == 4 * (tower.GetComponentInChildren<NumberOfFloors>().NumFloors - 1) - 1)
        {
            atTop = true;
        }
        return atTop;
    }
    public int GetNumRotated()
    {
        return numTimesRotated;
    }

    // Helper
    public bool trapQueueIsEmpty()
    {
        foreach( GameObject trap in queue)
        {
            if (trap.GetComponent<Button>().interactable)
            {
                return false;
            }
        }
        return true;
    }
}


