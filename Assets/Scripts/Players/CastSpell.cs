using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CastSpell : MonoBehaviour {
    [SerializeField] private GameObject[] spellButtons;
    [SerializeField] private SpellBase[] spellPrefabs;
    [SerializeField] private Image controllerCursor;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameManager gm;
    [SerializeField] private Camera cam;
    [SerializeField] private Camera cam2;
    [SerializeField] private GameObject playerOne;


    [SerializeField] private int cursorSpeed;
    [SerializeField] private int gridSize;

    [SerializeField] private float spellSpeed;

    [SerializeField] private int queueSize = 7;
    private List<GameObject> queue = new List<GameObject>();
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
    private int spellDirection;
    private int PlayerOneState = 1;
    private Vector3 movementVector = new Vector3(0, 0, 0);
    private Rigidbody rb;

    private List<Camera> allCameras = new List<Camera>();
    private bool active = true;

    void Start()
    {
        //Handle cursor or set buttons if controller connected
        p2Controller = gm.GetControllerTwoState();
        if (p2Controller)
        {
            controllerCursor.enabled = true;
        }
        else
        {
            controllerCursor.enabled = false;
        }

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
                SpellCast();
            }
        }

        PlayerOneState = playerOne.GetComponent<CameraOneRotator>().GetState();

        MoveTarget();

        if (spell != null && spellTarget != null) CheckValidLocation();

        if (Input.GetButtonDown("Submit_Joy_2"))
        {
            DestroyTarget();
            //For testing purposes currently
            ClearSpellQueue();
            CreateSpellQueue();
        }

        if (Input.GetButtonDown("Swap_Queue"))
        {
            SwitchQueue();
        }
    }

    void FixedUpdate()
    {
        if (castedSpell != null)
        {
            rb.velocity = movementVector;
        }
    }

    private Vector3? GetGridPosition()
    {
        if (RaycastFromCam() != null)
        {
            RaycastHit hit = RaycastFromCam().Value;
            int hitX = -1;
            int hitZ = -1;
            switch (playerOne.GetComponent<CameraOneRotator>().GetState())
            {
                case 1:
                    hitX = Mathf.RoundToInt(hit.point.x);
                    hitZ = Mathf.RoundToInt(hit.point.z);
                    break;
                case 2:
                    hitX = Mathf.RoundToInt(hit.point.x);
                    hitZ = Mathf.RoundToInt(hit.point.z);
                    break;
                case 3:
                    hitX = Mathf.RoundToInt(hit.point.x);
                    hitZ = Mathf.RoundToInt(hit.point.z);
                    break;
                case 4:
                    hitX = Mathf.RoundToInt(hit.point.x);
                    hitZ = Mathf.RoundToInt(hit.point.z);
                    break;
            }
            int hitY = Mathf.RoundToInt(hit.point.y - 1);
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
                if (spellDirection == 8)
                {
                    switch (PlayerOneState)
                    {
                        case 1:
                            castedSpell = spell.InstantiateSpell(50, playerOne.GetComponent<PlayerOneMovement>().transform.position.y, -42);
                            movementVector = new Vector3(-spellSpeed, 0, 0);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 2:
                            castedSpell = spell.InstantiateSpell(42, playerOne.GetComponent<PlayerOneMovement>().transform.position.y, 50);
                            movementVector = new Vector3(0, 0, -spellSpeed);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 3:
                            castedSpell = spell.InstantiateSpell(-50, playerOne.GetComponent<PlayerOneMovement>().transform.position.y, 42);
                            movementVector = new Vector3(spellSpeed, 0, 0);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                        case 4:
                            castedSpell = spell.InstantiateSpell(-42, playerOne.GetComponent<PlayerOneMovement>().transform.position.y, -50);
                            movementVector = new Vector3(0, 0, spellSpeed);
                            rb = castedSpell.GetComponent<Rigidbody>();
                            break;
                    }
                }

                spell = null;
                ClearButton();
                DestroyTarget();
                

                if (p2Controller)
                {
                    //eventSystem.SetSelectedGameObject(previouslySelected);
                    for (int i = queue.Count - 1; i >= 0; i--)
                    {
                        if (queue[i].activeInHierarchy)
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
        int floor = playerOne.GetComponent<CameraOneRotator>().GetFloor();
        float upperLimit = floor * 20;
        float lowerLimit = upperLimit - 20;

        Debug.Log(lowerLimit);
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
                        //eventSystem.SetSelectedGameObject(previouslySelected);
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

        eventSystem.SetSelectedGameObject(null);
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

    private void SetSelectedButton(int spellNum)
    {
        eventSystem.SetSelectedGameObject(spellButtons[spellNum].gameObject);
    }

    private void CreateSpellQueue()
    {
        for (int i = 0; i < queueSize; i++)
        {
            int random = 0; //Random.Range(0, spellButtons.Length);
            GameObject newSpell = Instantiate(spellButtons[random], new Vector3(-125f + 50f * i, 25f, 0), Quaternion.identity) as GameObject;
            newSpell.transform.SetParent(spellQueue.transform, false);

            if (i == 0)
            {
                eventSystem.firstSelectedGameObject = spellButtons[i].gameObject;
                eventSystem.SetSelectedGameObject(newSpell.gameObject);
            }

            //Add click listeners for all trap buttons
            newSpell.GetComponent<Button>().onClick.AddListener(() => OnClickSpell(random));
            newSpell.GetComponent<ButtonIndex>().ButtonIndexing(i);
            newSpell.GetComponent<Button>().onClick.AddListener(() => GetIndex(newSpell));

            queue.Add(newSpell);
        }

    }

    private void ClearSpellQueue()
    {
        for (int i = 0; i < queue.Count; i++)
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
            for (int i = 0; i < queue.Count; i++)
            {
                queue[i].GetComponent<Button>().interactable = false;
            }
        }

        if (active == false)
        {
            spellQueue.transform.SetAsLastSibling();
            spellQueue.transform.position -= new Vector3(15f, 15f, 0);
            for (int i = 0; i < queue.Count; i++)
            {
                queue[i].GetComponent<Button>().interactable = true;
            }
        }
        active = !active;
    }
}
