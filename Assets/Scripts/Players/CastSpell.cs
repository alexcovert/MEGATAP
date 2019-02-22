using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CastSpell : MonoBehaviour {
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject[] spellButtons;
    [SerializeField] private SpellBase[] spellPrefabs;
    [SerializeField] private Image controllerCursor;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject gameManager;
    [SerializeField] private Camera cam;
    [SerializeField] private Camera cam2;
    [SerializeField] private GameObject playerOne;

    private float spellSpeed;

    [SerializeField] private int queueSize = 7;
    public GameObject[] queue { get; private set; }
    [SerializeField] private GameObject spellQueue;
    private int queueIndex;

    private SpellBase spell;
    private GameObject spellTarget;
    private GameObject previouslySelected;
    private GameObject castedSpell;

    private bool p2Controller;
    private bool placeEnabled;

    //for spell movement and spawning
    private int ValidLocation;
    private SpellDirection spellDirection;
    private int PlayerOneState = 1;
    private Vector3 movementVector = new Vector3(0, 0, 0);
    private Rigidbody rb;

    private List<Camera> allCameras = new List<Camera>();
    public bool active { get; private set; }

    private PauseMenu pause;

    void Start()
    {
        active = true;
        queue = new GameObject[queueSize];
        pause = gameManager.GetComponent<PauseMenu>();
        //Handle cursor or set buttons if controller connected
        p2Controller = gameManager.GetComponent<CheckControllers>().GetControllerTwoState();

        //For testing purposes right now
        CreateSpellQueue();

        placeEnabled = false;

        allCameras.Add(cam);
        allCameras.Add(cam2);

        spellQueue.transform.SetAsFirstSibling();
        SwitchQueue();
    }


    void Update()
    {
        if (p2Controller && !pause.GameIsPaused)
        {
            if (Input.GetButton("Place_Joy_2") && placeEnabled)
            {
                SpellCast();
            }
        }

        PlayerOneState = playerOne.GetComponent<CameraOneRotator>().GetState();

        MoveTarget();

        if (spell != null && spellTarget != null) CheckValidLocation();

        if (Input.GetButtonDown("Submit_Joy_2") && !pause.GameIsPaused && !(cam.GetComponent<CameraTwoRotator>().GetFloor() == tower.GetComponent<NumberOfFloors>().NumFloors && cam.GetComponent<CameraTwoRotator>().GetState() == 4))
        {
            DestroyTarget();
            //For testing purposes currently
            CreateSpellQueue();
            if(active)
            {
                eventSystem.SetSelectedGameObject(queue[0]);
            }
        }

        if (Input.GetButtonDown("Swap_Queue") && !pause.GameIsPaused)
        {
            DestroyTarget();
            SwitchQueue();
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
                            castedSpell = spell.InstantiateSpell(50, playerOne.GetComponent<PlayerOneMovement>().transform.position.y + 1.5f, -42);
                            movementVector = new Vector3(-spellSpeed, 0, 0);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 2:
                            castedSpell = spell.InstantiateSpell(42, playerOne.GetComponent<PlayerOneMovement>().transform.position.y + 1.5f, 50);
                            movementVector = new Vector3(0, 0, -spellSpeed);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 3:
                            castedSpell = spell.InstantiateSpell(-50, playerOne.GetComponent<PlayerOneMovement>().transform.position.y + 1.5f, 42);
                            movementVector = new Vector3(spellSpeed, 0, 0);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 4:
                            castedSpell = spell.InstantiateSpell(-42, playerOne.GetComponent<PlayerOneMovement>().transform.position.y + 1.5f, -50);
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

                spell = null;
                ClearButton();
                DestroyTarget();
                

                if (p2Controller)
                {
                    //eventSystem.SetSelectedGameObject(previouslySelected);
                    bool buttonSet = false;
                    for (int i = queue.Length - 1; i >= 0; i--)
                    {
                        if (queue[i] != null && queue[i].activeInHierarchy && !buttonSet)
                        {
                            eventSystem.SetSelectedGameObject(queue[i]);
                            buttonSet = true;
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
            spellTarget = spell.InstantiateSpell(Vector3.zero);
            ValidLocation = spell.GetComponent<SpellBase>().GetLocation();
            spellDirection = spell.GetComponent<SpellBase>().GetDirection(); 
        }
        Destroy(spellTarget.GetComponent<Collider>());    

    }

    private void MoveTarget()
    {
        if (spellTarget != null)
        {
            if (GetGridPosition() != null)
            {
                Vector3 position = GetGridPosition().Value;
                spellTarget.transform.position = position;

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
                                if(queue[i].activeInHierarchy && !buttonSet)
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


    private void DestroyTarget()
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

        eventSystem.SetSelectedGameObject(null);
        StartCoroutine(EnableInput());
        
        DestroyTarget();
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
                GameObject newSpell = Instantiate(spellButtons[random], new Vector3(-108f + 40f * i, 20f, 0), Quaternion.identity) as GameObject;
                newSpell.transform.SetParent(spellQueue.transform, false);


                //Add click listeners for all trap buttons
                newSpell.GetComponent<Button>().onClick.AddListener(() => OnClickSpell(random));
                newSpell.GetComponent<ButtonIndex>().ButtonIndexing(i);
                newSpell.GetComponent<Button>().onClick.AddListener(() => GetIndex(newSpell));

                queue[i] = newSpell;

                if (active == false)
                {
                    queue[i].GetComponent<Button>().interactable = false;
                }
                return;
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
        placeEnabled = false;
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
