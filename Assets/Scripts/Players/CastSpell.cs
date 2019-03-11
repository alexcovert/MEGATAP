using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CastSpell : MonoBehaviour {
    [Header("Design Values -------------")]
    [SerializeField] private int cursorDistFromCenter;
    [SerializeField] private int queueSize;

    [Header("Programmers - GameObjects/Scripts -----")]
    [SerializeField] private GameObject tower;

    [SerializeField] private GameObject[] spellButtons;
    [SerializeField] private SpellBase[] spellPrefabs;
    [SerializeField] private GameObject spellQueue;

    [SerializeField] private Image controllerCursor;

    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject gameManager;
    [SerializeField] private Camera cam;
    [SerializeField] private Camera cam2;

    [SerializeField] private GameObject playerOne;
    [SerializeField] private GameObject[] targeting;

    private PlaceTrap pt;
    private PauseMenu pause;
    private List<Camera> allCameras = new List<Camera>();

    //Andy's Queue Stuff
    public GameObject[] queue { get; private set; }
    private int queueIndex;
    [HideInInspector] public bool active { get; private set; }

    //Spell Stuff
    private SpellBase spell;
    private SpellDirection spellDirection;
    private GameObject spellTarget;
    private GameObject castedSpell;
    private float spellSpeed;

    //for spell movement and spawning
    private int ValidLocation;
    private int PlayerOneState = 1;
    private Vector3 movementVector = new Vector3(0, 0, 0);
    private Rigidbody rb;
   
    //Controller Stuff
    private bool p2Controller;
    private bool placeEnabled;

    void Start()
    {
        //Get references
        pause = gameManager.GetComponent<PauseMenu>();
        pt = GetComponent<PlaceTrap>();
        allCameras.Add(cam);
        allCameras.Add(cam2);

        //Queue Initialization
        active = true;
        queue = new GameObject[queueSize];
        CreateSpellQueue();
        spellQueue.transform.SetAsLastSibling();

        //Handle cursor or set buttons if controller connected
        p2Controller = gameManager.GetComponent<CheckControllers>().GetControllerTwoState();
        placeEnabled = false;
    }


    void Update()
    {
        //Move target with cursor
        MoveTarget();

        //CONTROLLER ONLY Spell Casting Check
        if (p2Controller && !pause.GameIsPaused)
        {
            if (Input.GetButtonDown("Place_Joy_2") && placeEnabled && spellTarget != null)
            {
                SpellCast();
            }
        }

        PlayerOneState = playerOne.GetComponent<CameraOneRotator>().GetState();

      //  if (spell != null && spellTarget != null) CheckValidLocation();

        if (Input.GetButtonDown("Submit_Joy_2") && !pause.GameIsPaused && !(cam.GetComponent<CameraTwoRotator>().GetFloor() == tower.GetComponent<NumberOfFloors>().NumFloors && cam.GetComponent<CameraTwoRotator>().GetState() == 4))
        {
            DestroyTarget();
            CreateSpellQueue();
        }


        if (Input.GetMouseButtonDown(1) && ValidLocation == 1)
        {
            SpellCast();
        }

    }

    void FixedUpdate()
    {
        if (castedSpell != null && castedSpell.GetComponent<SpellBase>().CastDirection != SpellDirection.Instant)
        {
            rb.velocity = movementVector;
        }
    }

    private Vector3? GetGridPosition()
    {
        if (RaycastFromCam() != null)
        {
            RaycastHit hit = RaycastFromCam().Value;
            return new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }
        else return null;
    }

    private RaycastHit? RaycastFromCam()
    {
        RaycastHit hit;
        Ray ray;
        if (p2Controller)
        {
            if (GetCameraForMousePosition() == cam2)
            {
                ray = cam2.ScreenPointToRay(controllerCursor.transform.position);
            }
            else
            {
                ray = cam.ScreenPointToRay(controllerCursor.transform.position);
            }
        }
        else
        {
            if (GetCameraForMousePosition() == cam2)
            {
                ray = cam2.ScreenPointToRay(Input.mousePosition);
            }
            else
            {
                ray = cam.ScreenPointToRay(Input.mousePosition);
            }
        }
        if (Physics.Raycast(ray, out hit, float.MaxValue,LayerMask.GetMask("Tower")))
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
        if (!Input.GetMouseButtonUp(1) && ValidLocation == 1)
        {
            SpellCast();
        }
    }

    public void OnClickPlayer()
    {
        if (!Input.GetMouseButtonUp(1) && ValidLocation == 2)
        {
            SpellCast();
        }
    }

    private void SpellCast()
    {
        if (GetGridPosition() != null)
        {
            Vector3 position = GetGridPosition().Value;
            if (spellTarget != null && CheckFloor(position.y))
            {
                //Spell comes from right side
                if (spellDirection == SpellDirection.Right)
                {
                    switch (PlayerOneState)
                    {
                        case 1:
                            castedSpell = spell.InstantiateSpell(50, spellTarget.transform.position.y, -42);
                            movementVector = new Vector3(-spellSpeed, 0, 0);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 2:
                            castedSpell = spell.InstantiateSpell(42, spellTarget.transform.position.y, 50);
                            movementVector = new Vector3(0, 0, -spellSpeed);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 3:
                            castedSpell = spell.InstantiateSpell(-50, spellTarget.transform.position.y, 42);
                            movementVector = new Vector3(spellSpeed, 0, 0);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 4:
                            castedSpell = spell.InstantiateSpell(-42, spellTarget.transform.position.y, -50);
                            movementVector = new Vector3(0, 0, spellSpeed);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                    }
                    castedSpell.GetComponent<SpellBase>().SpellCast = true;
                }
                if(spellDirection == SpellDirection.Instant)
                {
                    castedSpell = spell.InstantiateSpell(spell.transform.position);
                    castedSpell.GetComponent<SpellBase>().SpellCast = true;
                }
                if(spellDirection == SpellDirection.Ceiling)
                {
                    switch (PlayerOneState)
                    {
                        case 1:
                            castedSpell = spell.InstantiateSpell(spellTarget.transform.position.x, spellTarget.transform.position.y + 200, -42);
                            movementVector = new Vector3(0, -spellSpeed, 0);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 2:
                            castedSpell = spell.InstantiateSpell(42, spellTarget.transform.position.y + 200, spellTarget.transform.position.z);
                            movementVector = new Vector3(0, -spellSpeed, 0);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 3:
                            castedSpell = spell.InstantiateSpell(spellTarget.transform.position.x, spellTarget.transform.position.y + 200, 42);
                            movementVector = new Vector3(0, -spellSpeed, 0);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 4:
                            castedSpell = spell.InstantiateSpell(-42, spellTarget.transform.position.y + 200, spellTarget.transform.position.z);
                            movementVector = new Vector3(0, -spellSpeed, 0);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                    }
                    castedSpell.GetComponent<SpellBase>().SpellCast = true;
                }

                spell = null;
                ClearButton();
                DestroyTarget();
                

                if (p2Controller)
                {
                    bool buttonSet = false;
                    for (int i = queue.Length - 1; i >= 0; i--)
                    {
                        if (queue[i] != null && queue[i].activeInHierarchy && !buttonSet)
                        {
                            eventSystem.SetSelectedGameObject(queue[i]);
                            buttonSet = true;
                        }
                    }
                    if(!buttonSet)
                    {
                        for(int i = 0; i < pt.queue.Count; i++)
                        {
                            if(pt.queue[i] != null && pt.queue[i].activeInHierarchy && !buttonSet)
                            {

                                controllerCursor.transform.localPosition = new Vector3(0, 130);
                                eventSystem.SetSelectedGameObject(pt.queue[i]);
                                buttonSet = true;
                            }
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
        int floor = playerOne.GetComponent<CameraOneRotator>().GetFloor();
        float upperLimit = floor * 20;
        float lowerLimit = upperLimit - 20;

        return (hitY >= lowerLimit && hitY <= upperLimit);
    }

    //Check  if it's being placed on correct object
    private bool CheckValidLocation()
    {
        return true;

    }

    private void SetTarget()
    {
        if (spell != null)
        {
            ValidLocation = spell.GetComponent<SpellBase>().GetLocation();
            spellDirection = spell.GetComponent<SpellBase>().GetDirection();

            Vector3 pos = Vector3.zero;
            if (spellTarget != null)
            {
                pos = spellTarget.transform.position;
                Destroy(spellTarget.gameObject);
                
            }
            if (spellDirection == SpellDirection.Right || spellDirection == SpellDirection.Left)
            {
                spellTarget = Instantiate(targeting[1], pos, Quaternion.identity);
            }
            else if (spellDirection == SpellDirection.Ceiling || spellDirection == SpellDirection.Floor)
            {
                spellTarget = Instantiate(targeting[0], pos, Quaternion.identity);
            }
            else
            {
                spellTarget = spell.InstantiateSpell(Vector3.zero);
            }
            Destroy(spellTarget.GetComponent<Collider>());
        }

    }

    private void MoveTarget()
    {
        if (spellTarget != null)
        {
            if (GetGridPosition() != null)
            {
                Vector3 position = GetGridPosition().Value;
                if (spellDirection == SpellDirection.Right || spellDirection == SpellDirection.Left)
                {
                    GetComponent<MoveControllerCursor>().SpellCastDirection = SpellDirection.Right;
                    switch (PlayerOneState)
                    {
                        case 1:
                            spellTarget.transform.eulerAngles = new Vector3(0, 0, 90);
                            spellTarget.transform.position = new Vector3(transform.position.x, position.y, -42);
                            break;
                        case 2:
                            spellTarget.transform.eulerAngles = new Vector3(180, 90, 90);
                            spellTarget.transform.position = new Vector3(42, position.y, transform.position.z);
                            break;
                        case 3:
                            spellTarget.transform.eulerAngles = new Vector3(180, 0, 90);
                            spellTarget.transform.position = new Vector3(transform.position.x, position.y, 42);
                            break;
                        case 4:
                            spellTarget.transform.eulerAngles = new Vector3(0, 90, 90);
                            spellTarget.transform.position = new Vector3(-42, position.y, transform.position.z);
                            break;
                    }
                }

                else if (spellDirection == SpellDirection.Ceiling || spellDirection == SpellDirection.Floor)
                {
                    GetComponent<MoveControllerCursor>().SpellCastDirection = SpellDirection.Ceiling;
                    int playerFloor = playerOne.GetComponent<CameraOneRotator>().GetFloor() * 10;
                    switch (PlayerOneState)
                    {
                        case 1:
                            spellTarget.transform.position = new Vector3(position.x, playerFloor, -42);
                            break;
                        case 3:
                            spellTarget.transform.eulerAngles = new Vector3(0, 180, 0);
                            spellTarget.transform.position = new Vector3(position.x, playerFloor, 42);
                            break;
                        case 2:
                            spellTarget.transform.eulerAngles = new Vector3(0, -90, 0);
                            spellTarget.transform.position = new Vector3(42, playerFloor, position.z);
                            break;
                        case 4:
                            spellTarget.transform.eulerAngles = new Vector3(0, 90, 0);
                            spellTarget.transform.position = new Vector3(-42, playerFloor, position.z);
                            break;
                    }

                }
                else
                {
                    GetComponent<MoveControllerCursor>().SpellCastDirection = SpellDirection.Instant;
                    spellTarget.transform.position = position;
                }

                if (Input.GetMouseButton(1) || Input.GetButton("Cancel_Joy_2"))
                {
                    DestroyTarget();

                    if (p2Controller)
                    {
                        if(active)
                        {
                            bool buttonSet = false;
                            for(int i = 0; i < queue.Length; i++)
                            {
                                if(queue[i].activeInHierarchy && !buttonSet && queue[i] != null)
                                {
                                    eventSystem.SetSelectedGameObject(queue[i]);
                                    buttonSet = true;
                                }
                            }

                        }
                        placeEnabled = false;
                    }
                }
            }
        }
    }


    public void DestroyTarget()
    {
        if (spellTarget != null)
        {
            Destroy(spellTarget);
            ValidLocation = 0;
            spellDirection = 0;
            spellSpeed = 0;
            spellTarget = null;
        }
    }

    private void OnClickSpell(int spellNum)
    {
        spell = spellPrefabs[spellNum];

        //eventSystem.SetSelectedGameObject(null);
        StartCoroutine(EnableInput());
        
        DestroyTarget();
        GetComponent<PlaceTrap>().DestroyGhost();
        SetTarget();
        spellSpeed = spell.GetComponent<SpellBase>().GetSpeed();
    }

    private void GetIndex(GameObject spell)
    {
        queueIndex = spell.GetComponent<ButtonIndex>().GetIndex();
    }


    private void CreateSpellQueue()
    {
        for (int i = 0; i < queueSize; i++)
        {
            if (queue[i] == null)
            {
                int random = Random.Range(0, spellButtons.Length);
                GameObject newSpell = Instantiate(spellButtons[random], new Vector3(-170f + 40f * i, 20f, 0), Quaternion.identity) as GameObject;
                newSpell.transform.SetParent(spellQueue.transform, false);


                //Add click listeners for all trap buttons
                newSpell.GetComponent<Button>().onClick.AddListener(() => OnClickSpell(random));
                newSpell.GetComponent<ButtonIndex>().ButtonIndexing(i);
                newSpell.GetComponent<Button>().onClick.AddListener(() => GetIndex(newSpell));

                queue[i] = newSpell;

                //if (active == false)
                //{
                //    queue[i].GetComponent<Button>().interactable = false;
                //}
            }
        }
    }

    /*private void ClearSpellQueue()
    {
        for (int i = 0; i < queue.Length; i++)
        {
            if (!queue[i].activeSelf)
            {
               Destroy(queue[i]);
            }
        }
    }*/

    private void ClearButton()
    {
        // queue[queueIndex].SetActive(false);
        Destroy(queue[queueIndex]);
        queue[queueIndex] = null;

    }

    //Make player wait .5 seconds after pressing button to be able to place trap.
    //Gets rid of controller bug where pressing A to select a trap also immediately places it
    IEnumerator EnableInput()
    {
        //placeEnabled = false;
        yield return new WaitForSeconds(0.5f);
        placeEnabled = true;
    }

    private Camera GetCameraForMousePosition()
    {
        foreach (Camera camera in allCameras)
        {
            Vector3 point = new Vector3(0, 0, 0);
            if (p2Controller)
            {
                point = camera.ScreenToViewportPoint(controllerCursor.transform.position);
            }
 
            else
            {
                point = camera.ScreenToViewportPoint(Input.mousePosition);
            }
            if (point.x >= 0 && point.x <= 1 && point.y >= 0 && point.y <= 1)
            {
                return camera;
            }
        }
        return null;
    }

    private void SwitchQueue()
    {
        if (active == true)
        {
            spellQueue.transform.SetAsFirstSibling();
            spellQueue.transform.position += new Vector3(15f, 15f, 0);
            for (int i = 0; i < queue.Length; i++)
            {
                if (queue[i] != null)
                {
                    queue[i].GetComponent<Button>().interactable = false;
                }
            }
        }

        if (active == false)
        {
            controllerCursor.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 - cursorDistFromCenter, 0);
            GetComponent<MoveControllerCursor>().MovingTraps = false;
            bool buttonSet = false;
            spellQueue.transform.SetAsLastSibling();
            spellQueue.transform.position -= new Vector3(15f, 15f, 0);
            for (int i = 0; i < queue.Length; i++)
            {
                if (queue[i] != null)
                {
                    queue[i].GetComponent<Button>().interactable = true;


                    if (queue[i].activeInHierarchy && !buttonSet)
                    {
                        eventSystem.SetSelectedGameObject(queue[i]);
                        buttonSet = true;
                    }
                }
            }
        }
        active = !active;
    }
}
