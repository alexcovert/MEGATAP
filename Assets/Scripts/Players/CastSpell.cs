using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CastSpell : MonoBehaviour
{
    [Header("Design Values -------------")]
    [SerializeField] private int queueSize;
    [SerializeField] private int verticalSpellSpawnHeight;
    [SerializeField] private float CooldownReductionPercentage;

    [Header("Percentage is X/100 currently.")]
    [SerializeField] private int CommonRarityChance = 50;
    [SerializeField] private int UncommonRarityChance = 35;
    [SerializeField] private int RareRarityChance = 15;

    //[SerializeField] [Tooltip("Must be in SAME ORDER and SAME AMOUNT of spell prefabs and spell buttons arrays.")]
    //private float[] spellCooldowns;


    [Header("Programmers - GameObjects/Scripts -----")]
    [SerializeField] private GameObject tower;
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


    [Header("Spell Buttons & Prefabs")]
    [SerializeField] private GameObject[] CommonSpellButtons;
    [SerializeField] private GameObject[] UncommonSpellButtons;
    [SerializeField] private GameObject[] RareSpellButtons;

    [SerializeField] private SpellBase[] CommonSpellPrefabs;
    [SerializeField] private SpellBase[] UncommonSpellPrefabs;
    [SerializeField] private SpellBase[] RareSpellPrefabs;

    private bool tutorial;
    private int tutorialCounter = 0;

    //Queue Stuff
    public GameObject[] queue { get; private set; }
    private int queueIndex;
    [HideInInspector] public bool active { get; private set; }


    //Spell Stuff
    private SpellBase spell;
    private SpellDirection spellDirection;
    private GameObject spellTarget;
    private GameObject castedSpell;
    private float spellSpeed;
    private bool atTop = false;

    //for spell movement and spawning
    private int ValidLocation;
    private int PlayerOneState = 1;
    private Vector3 movementVector = new Vector3(0, 0, 0);
    private Rigidbody rb;

    //Controller Stuff
    private bool p2Controller;
    public bool placeEnabled;
    public bool InputEnabled = true;
    private int previouslySelectedIndex;
    private InputManager inputManager;
    private CheckControllers cc;

    private GameObject currentSelectedGameObject;

    private void Awake()
    {
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        cc = inputManager.GetComponent<CheckControllers>();
    }

    void Start()
    {
        //Get references
        pause = gameManager.GetComponent<PauseMenu>();
        pt = GetComponent<PlaceTrap>();

        allCameras.Add(cam);
        allCameras.Add(cam2);

        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            tutorial = true;
        }

        //Queue Initialization
        queue = new GameObject[queueSize];
        if(!tutorial) CreateSpellQueue();
        spellQueue.transform.SetAsLastSibling();

        //Handle cursor or set buttons if controller connected
        //p2Controller = gameManager.GetComponent<CheckControllers>().GetTopPlayerControllerState();
        placeEnabled = false;
        InputEnabled = true;
    }


    void Update()
    {
        //Tutorial checks
        if (inputManager.GetButtonDown(InputCommand.TopPlayerRotate) && !pause.GameIsPaused && tutorial && tutorialCounter < 2)
        {
            tutorialCounter++;
            if(tutorialCounter == 2)
            {
                CreateSpellQueue();
            }
        }

        p2Controller = cc.GetTopPlayerControllerState();


        //Move target with cursor
        MoveTarget();

        //CONTROLLER ONLY Spell Casting Check
        if (p2Controller && !pause.GameIsPaused)
        {
            if (inputManager.GetButtonDown(InputCommand.TopPlayerSelect) && placeEnabled && InputEnabled && spellTarget != null)
            {
                SpellCast();
            }
        }

        PlayerOneState = playerOne.GetComponent<CameraOneRotator>().GetState();


        //Cancel spell
        if(Input.GetMouseButtonDown(1) && spellTarget != null && !cc.topPlayersController)
        {
            DestroyTarget();
        }

        //Safety check to make sure the player's cursor isn't lost / nothing is selected
        if (eventSystem.currentSelectedGameObject != null)
        {
            currentSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
        
        if(Input.GetMouseButtonDown(0) && cc.topPlayersController)
        {
            eventSystem.SetSelectedGameObject(currentSelectedGameObject);
        }
        if (cc.topPlayersController && eventSystem.currentSelectedGameObject == null)
        {
            SetSelectedButton();
        }
        atTop = GetComponent<PlaceTrap>().LastFace();
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
        Vector3 pos;
        if (p2Controller) pos = controllerCursor.transform.position;
        else pos = Input.mousePosition;
        pos.z = 30;
        return cam2.ScreenToWorldPoint(pos);
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
        if (!Input.GetMouseButtonUp(1) && ValidLocation == 1 && !p2Controller && Input.mousePosition.y <= Screen.height / 2)
        {
            SpellCast();
        }
    }

    public void OnClickPlayer()
    {
        if (!Input.GetMouseButtonUp(1) && ValidLocation == 2 && !p2Controller)
        {
            SpellCast();
        }
    }

    private void SpellCast()
    {
        if (GetGridPosition() != null)
        {
            Vector3 position = GetGridPosition().Value;
            if (spellTarget != null)
            {
                //Spell comes from right side
                if (spellDirection == SpellDirection.Right)
                {
                    float pos;
                    if(spell.ToString() == "Wind (SpellBase)")
                    {
                        pos = 45;
                    }                
                    else
                    {
                        pos = 50;
                    }
                    switch (PlayerOneState)
                    {
                        case 1:
                            castedSpell = spell.InstantiateSpell(pos, spellTarget.transform.position.y, -42);
                            movementVector = new Vector3(-spellSpeed, 0, 0);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 2:
                            castedSpell = spell.InstantiateSpell(42, spellTarget.transform.position.y, pos);
                            movementVector = new Vector3(0, 0, -spellSpeed);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 3:
                            castedSpell = spell.InstantiateSpell(-pos, spellTarget.transform.position.y, 42);
                            movementVector = new Vector3(spellSpeed, 0, 0);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 4:
                            castedSpell = spell.InstantiateSpell(-42, spellTarget.transform.position.y, -pos);
                            movementVector = new Vector3(0, 0, spellSpeed);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                    }
                    castedSpell.GetComponent<SpellBase>().SpellCast = true;
                }
                if (spellDirection == SpellDirection.Instant)
                {
                    castedSpell = spell.InstantiateSpell(spell.transform.position);
                    castedSpell.GetComponent<SpellBase>().SpellCast = true;
                }
                if (spellDirection == SpellDirection.Ceiling)
                {
                    float verticalOffset = 5;
                    switch (PlayerOneState)
                    {
                        case 1:
                            castedSpell = spell.InstantiateSpell(spellTarget.transform.position.x, spellTarget.transform.position.y - verticalOffset, -42);
                            movementVector = Vector3.zero;
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 2:
                            castedSpell = spell.InstantiateSpell(42, spellTarget.transform.position.y - verticalOffset, spellTarget.transform.position.z);
                            movementVector = Vector3.zero;
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 3:
                            castedSpell = spell.InstantiateSpell(spellTarget.transform.position.x, spellTarget.transform.position.y - verticalOffset, 42);
                            movementVector = Vector3.zero;
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 4:
                            castedSpell = spell.InstantiateSpell(-42, spellTarget.transform.position.y - verticalOffset, spellTarget.transform.position.z);
                            movementVector = Vector3.zero;
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                    }
                    castedSpell.GetComponent<SpellBase>().SpellCast = true;
                }
                if (atTop == false)
                {
                    StartCoroutine(StartCooldown(spell.GetComponent<SpellBase>().CooldownTime, queue[queueIndex].transform.localPosition, queueIndex));
                }
                else
                {
                    StartCoroutine(StartCooldown((spell.GetComponent<SpellBase>().CooldownTime - (spell.GetComponent<SpellBase>().CooldownTime * CooldownReductionPercentage)), queue[queueIndex].transform.localPosition, queueIndex));
                }

                previouslySelectedIndex = queueIndex;

                spell = null;

                ClearButton();

                DestroyTarget();

                if (p2Controller)
                {
                    SetSelectedButton();
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
                spellTarget = Instantiate(targeting[2], pos, Quaternion.identity);
                //spellTarget = spell.InstantiateSpell(Vector3.zero);
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
                Vector3 center = new Vector3(cam2.pixelWidth / 2, cam2.pixelHeight / 2, 0);
                if (spellDirection == SpellDirection.Right || spellDirection == SpellDirection.Left)
                {
                    GetComponent<MoveControllerCursor>().SpellCastDirection = SpellDirection.Right;
                    switch (PlayerOneState)
                    {
                        case 1:
                            spellTarget.transform.eulerAngles = new Vector3(0, 0, -90);
                            //spellTarget.transform.position = new Vector3(transform.position.x, position.y, -45);
                            spellTarget.transform.position = new Vector3(cam2.ScreenToWorldPoint(center).x + 5, position.y, -45);
                            break;
                        case 2:
                            spellTarget.transform.eulerAngles = new Vector3(180, 90, 90);
                            //spellTarget.transform.position = new Vector3(45, position.y, transform.position.z);
                            spellTarget.transform.position = new Vector3(45, position.y, cam2.ScreenToWorldPoint(center).z + 5);
                            break;
                        case 3:
                            spellTarget.transform.eulerAngles = new Vector3(180, 0, 90);
                            //spellTarget.transform.position = new Vector3(transform.position.x, position.y, 45);
                            spellTarget.transform.position = new Vector3(cam2.ScreenToWorldPoint(center).x - 5, position.y, 45);
                            break;
                        case 4:
                            spellTarget.transform.eulerAngles = new Vector3(0, 90, -90);
                            //spellTarget.transform.position = new Vector3(-45, position.y, transform.position.z);
                            spellTarget.transform.position = new Vector3(-45, position.y, cam2.ScreenToWorldPoint(center).z - 5);
                            break;
                    }
                }

                else if (spellDirection == SpellDirection.Ceiling || spellDirection == SpellDirection.Floor)
                {
                    GetComponent<MoveControllerCursor>().SpellCastDirection = SpellDirection.Ceiling;
                    //int playerFloor = playerOne.GetComponent<CameraOneRotator>().GetFloor() * 20 - 10;
                    switch (PlayerOneState)
                    {
                        case 1:
                            spellTarget.transform.position = new Vector3(position.x, cam2.ScreenToWorldPoint(center).y + 2.5f, -45);
                            spellTarget.transform.rotation = Quaternion.identity;
                            break;
                        case 3:
                            spellTarget.transform.eulerAngles = new Vector3(0, 180, 0);
                            spellTarget.transform.position = new Vector3(position.x, cam2.ScreenToWorldPoint(center).y + 2.5f, 45);
                            break;
                        case 2:
                            spellTarget.transform.eulerAngles = new Vector3(0, -90, 0);
                            spellTarget.transform.position = new Vector3(45, cam2.ScreenToWorldPoint(center).y + 2.5f, position.z);
                            break;
                        case 4:
                            spellTarget.transform.eulerAngles = new Vector3(0, 90, 0);
                            spellTarget.transform.position = new Vector3(-45, cam2.ScreenToWorldPoint(center).y + 2.5f, position.z);
                            break;
                    }

                }
                else
                {
                    GetComponent<MoveControllerCursor>().SpellCastDirection = SpellDirection.Instant;
                    spellTarget.transform.position = position;
                    switch (PlayerOneState)
                    {
                        case 1:
                            break;
                        case 3:
                            spellTarget.transform.eulerAngles = new Vector3(0, 180, 0);
                            break;
                        case 2:
                            spellTarget.transform.eulerAngles = new Vector3(0, -90, 0);
                            break;
                        case 4:
                            spellTarget.transform.eulerAngles = new Vector3(0, 90, 0);
                            break;
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

    private void OnClickSpellCommon(int spellNum)
    {
        spell = CommonSpellPrefabs[spellNum];

        StartCoroutine(EnableInput());

        DestroyTarget();
        GetComponent<PlaceTrap>().DestroyGhost();
        SetTarget();
        spellSpeed = spell.GetComponent<SpellBase>().GetSpeed();
    }

    private void OnClickSpellUncommon(int spellNum)
    {
        spell = UncommonSpellPrefabs[spellNum];

        StartCoroutine(EnableInput());

        DestroyTarget();
        GetComponent<PlaceTrap>().DestroyGhost();
        SetTarget();
        spellSpeed = spell.GetComponent<SpellBase>().GetSpeed();
    }

    private void OnClickSpellRare(int spellNum)
    {
        spell = RareSpellPrefabs[spellNum];

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

    //Called when the spell cooldown is over
    private void GenerateNewSpell(Vector3 position, int index)
    {
        //Instantiate Spell Button
        int randomIndex;
        GameObject newSpell;
        int SpellChance = Random.Range(1, 100);

        if (SpellChance <= CommonRarityChance)
        {
            randomIndex = Random.Range(0, CommonSpellButtons.Length);
            newSpell = Instantiate(CommonSpellButtons[randomIndex], position, Quaternion.identity) as GameObject;
            newSpell.transform.SetParent(spellQueue.transform, false);

            //Add Click Listener
            newSpell.GetComponent<Button>().onClick.AddListener(() => OnClickSpellCommon(randomIndex));
            newSpell.GetComponent<ButtonIndex>().ButtonIndexing(index);
            newSpell.GetComponent<Button>().onClick.AddListener(() => GetIndex(newSpell));

            queue[index] = newSpell;
        }
        else if (SpellChance > CommonRarityChance && SpellChance < (100 - RareRarityChance))
        {
            randomIndex = Random.Range(0, UncommonSpellButtons.Length);
            newSpell = Instantiate(UncommonSpellButtons[randomIndex], position, Quaternion.identity) as GameObject;
            newSpell.transform.SetParent(spellQueue.transform, false);

            //Add Click Listener
            newSpell.GetComponent<Button>().onClick.AddListener(() => OnClickSpellUncommon(randomIndex));
            newSpell.GetComponent<ButtonIndex>().ButtonIndexing(index);
            newSpell.GetComponent<Button>().onClick.AddListener(() => GetIndex(newSpell));

            queue[index] = newSpell;
        }
        else if (SpellChance >= (CommonRarityChance + UncommonRarityChance))
        {
            randomIndex = Random.Range(0, RareSpellButtons.Length);
            newSpell = Instantiate(RareSpellButtons[randomIndex], position, Quaternion.identity) as GameObject;
            newSpell.transform.SetParent(spellQueue.transform, false);

            //Add Click Listener
            newSpell.GetComponent<Button>().onClick.AddListener(() => OnClickSpellRare(randomIndex));
            newSpell.GetComponent<ButtonIndex>().ButtonIndexing(index);
            newSpell.GetComponent<Button>().onClick.AddListener(() => GetIndex(newSpell));

            queue[index] = newSpell;
        }
    }

    //Called on start only now
    private void CreateSpellQueue()
    {
        for (int i = 0; i < queueSize; i++)
        {
            if (queue[i] == null)
            {
                int randomIndex;
                GameObject newSpell;
                int SpellChance = Random.Range(1, 100);

                if (SpellChance <= CommonRarityChance)
                {
                    randomIndex = Random.Range(0, CommonSpellButtons.Length);
                    newSpell = Instantiate(CommonSpellButtons[randomIndex], new Vector3(100f + 40f * i, 20f, 0), Quaternion.identity) as GameObject;
                    newSpell.transform.SetParent(spellQueue.transform, false);

                    //Add Click Listener
                    newSpell.GetComponent<Button>().onClick.AddListener(() => OnClickSpellCommon(randomIndex));
                    newSpell.GetComponent<ButtonIndex>().ButtonIndexing(i);
                    newSpell.GetComponent<Button>().onClick.AddListener(() => GetIndex(newSpell));

                    queue[i] = newSpell;
                }
                else if (SpellChance > CommonRarityChance && SpellChance < (100 - RareRarityChance))
                {
                    randomIndex = Random.Range(0, UncommonSpellButtons.Length);
                    newSpell = Instantiate(UncommonSpellButtons[randomIndex], new Vector3(100f + 40f * i, 20f, 0), Quaternion.identity) as GameObject;
                    newSpell.transform.SetParent(spellQueue.transform, false);

                    //Add Click Listener
                    newSpell.GetComponent<Button>().onClick.AddListener(() => OnClickSpellUncommon(randomIndex));
                    newSpell.GetComponent<ButtonIndex>().ButtonIndexing(i);
                    newSpell.GetComponent<Button>().onClick.AddListener(() => GetIndex(newSpell));

                    queue[i] = newSpell;
                }
                else if (SpellChance >= (CommonRarityChance + UncommonRarityChance))
                {
                    randomIndex = Random.Range(0, RareSpellButtons.Length);
                    newSpell = Instantiate(RareSpellButtons[randomIndex], new Vector3(100f + 40f * i, 20f, 0), Quaternion.identity) as GameObject;
                    newSpell.transform.SetParent(spellQueue.transform, false);

                    //Add Click Listener
                    newSpell.GetComponent<Button>().onClick.AddListener(() => OnClickSpellRare(randomIndex));
                    newSpell.GetComponent<ButtonIndex>().ButtonIndexing(i);
                    newSpell.GetComponent<Button>().onClick.AddListener(() => GetIndex(newSpell));

                    queue[i] = newSpell;
                }
            }
        }
    }

    private void ClearButton()
    {
        queue[queueIndex].GetComponent<Button>().interactable = false;
    }


    //Set new selected button if the controller is being used.
    private void SetSelectedButton()
    {

        if (p2Controller)
        {
            bool buttonSet = false;

            //Loop over remaining spell queue to see if any are available
            for (int i = previouslySelectedIndex; i < queue.Length; i++)
            {
                if (queue[i] != null && queue[i].activeInHierarchy && queue[i].GetComponent<Button>().interactable && !buttonSet)
                {
                    //eventSystem.SetSelectedGameObject(queue[i]);
                    queue[i].GetComponent<Button>().Select();
                    queue[i].GetComponent<Button>().OnSelect(new BaseEventData(eventSystem));
                    controllerCursor.transform.localPosition = new Vector3(0, -130);
                    buttonSet = true;
                }
            }

            //Loop over previous of spell queue to see if any are available
            if(!buttonSet)
            {
                for (int i = previouslySelectedIndex; i >= 0; i--)
                {
                    if (queue[i] != null && queue[i].activeInHierarchy && queue[i].GetComponent<Button>().interactable && !buttonSet)
                    {
                        //eventSystem.SetSelectedGameObject(queue[i]);
                        queue[i].GetComponent<Button>().Select();
                        queue[i].GetComponent<Button>().OnSelect(new BaseEventData(eventSystem));
                        controllerCursor.transform.localPosition = new Vector3(0, -130);
                        buttonSet = true;
                    }
                }
            }

            //Loop over traps to set available button
            if (!buttonSet)
            {
                for (int i = 0; i < pt.queue.Count; i++)
                {
                    if (pt.queue[i] != null && pt.queue[i].activeInHierarchy && !buttonSet && pt.queue[i].GetComponent<Button>().interactable)
                    {

                        controllerCursor.transform.localPosition = new Vector3(0, 130);
                        //eventSystem.SetSelectedGameObject(pt.queue[i]);
                        pt.queue[i].GetComponent<Button>().Select();
                        pt.queue[i].GetComponent<Button>().OnSelect(new BaseEventData(eventSystem));
                        buttonSet = true;
                    }
                }
            }
            placeEnabled = false;
        }
    }

    //Mostly for controller - wait between inputs to prevent spamming and some button selection bugs
    IEnumerator EnableInput()
    {
        yield return new WaitForSeconds(0.5f);
        placeEnabled = true;
    }

    //Called from pause script to re-enable input after pressing "Resume"
    public IEnumerator ResumeInput()
    {
        yield return new WaitForSeconds(0.5f);
        InputEnabled = true;
    }
    
    //Start a cooldown on the button pressed. Needs current button position and queue index to replace.
    private IEnumerator StartCooldown(float cooldownTime, Vector3 buttonPosition, int index)
    {
        float cooldownTimePassed = 0;
        Destroy(queue[index]);
        GenerateNewSpell(buttonPosition, index);

        Button button = queue[index].GetComponent<Button>();

        Image[] images = queue[index].GetComponentsInChildren<Image>();
        Image fillImage = images[0];
        foreach(Image image in images)
        {
            if(image.type == Image.Type.Filled)
            {
                fillImage = image;
                fillImage.fillAmount = 0;
            }
        }

        button.interactable = false;

        while (cooldownTimePassed <= cooldownTime)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            cooldownTimePassed += Time.deltaTime;
            fillImage.fillAmount = cooldownTimePassed / cooldownTime;
            
            if(cooldownTimePassed >= cooldownTime)
            {
                button.interactable = true;

                if (eventSystem.currentSelectedGameObject == null && p2Controller)
                {
                    SetSelectedButton();
                }
            }
        }
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
}
