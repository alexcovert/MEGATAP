using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoveControllerCursor : MonoBehaviour {
    [Header("Programmers - GameObjects/Scripts -----")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Image controllerCursor;
    private CheckControllers checkControllers;
    private PauseMenu pause;

    [Header("Controller Values -----")]
    [SerializeField] private float cursorDelayHorizontal; //How long the script waits before moving cursor to next grid pos while stick is held down.
    [SerializeField] private float cursorDelayVertical;
    [SerializeField] private float freeRoamSpellSpeed;
    [SerializeField][Range(0, 1)] private float spellBarSpeedMultiplier;
    [SerializeField] private float cursorGrid;  //Size of the cursor grid - DIFFERENT from the size of the worldspace/trap grid. Should be fine at 23 but might need to be tweaked if we change UI.
    [Tooltip("Higher # = Lower Sensitivity")] [SerializeField] private float stickSensitivity; //Between 0-1 ; how far the player needs to push the stick for it to move the cursor
                                                                                               //Needed to set it higher because some controller sticks naturally move left/right a little.

    [Header("Audio-------------")]
    [SerializeField] private AudioClip spaceSelectionSFX;
    //private AudioSource audioSource;

    private bool p2Controller;
    private bool cursorHorizontalMove = true;
    private bool cursorVerticalMove = true;

    [HideInInspector]
    public bool MovingTraps = true;
    [HideInInspector]
    public SpellDirection SpellCastDirection = SpellDirection.Instant;

    private float screenWidth, screenHeight;

    void Start () {
        pause = gameManager.GetComponent<PauseMenu>();
        checkControllers = gameManager.GetComponent<CheckControllers>();
        //audioSource = GetComponent<AudioSource>();
        p2Controller = checkControllers.GetControllerTwoState();


        //Screen size
        CanvasScaler scaler = controllerCursor.GetComponentInParent<CanvasScaler>();
        float expectedAspectRatio = scaler.referenceResolution.x / scaler.referenceResolution.y;
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        screenWidth = (aspectRatio / expectedAspectRatio) * (scaler.referenceResolution.x / 2);
        screenHeight = scaler.referenceResolution.y / 2;
    }
	
    //Move cursor with grid
	void FixedUpdate () {
        p2Controller = checkControllers.GetControllerTwoState();

        //CONTROLS FOR MOVING TRAPS---------------------------------------------------------------------------------------------
        if (p2Controller && !pause.GameIsPaused && MovingTraps)
        {
            Vector3 cursorPos = controllerCursor.GetComponent<RectTransform>().localPosition;
            if (Input.GetAxisRaw("Horizontal_Joy_2") > stickSensitivity && cursorHorizontalMove && cursorPos.x < screenWidth)
            {
                controllerCursor.GetComponent<RectTransform>().localPosition += new Vector3(cursorGrid, 0, 0);
                cursorHorizontalMove = false;
                StartCoroutine(EnableHorizontalCursorMove());

                //audioSource.PlayOneShot(spaceSelectionSFX);
            }
            else if (Input.GetAxisRaw("Horizontal_Joy_2") < -stickSensitivity && cursorHorizontalMove && cursorPos.x > -screenWidth)
            {
                controllerCursor.GetComponent<RectTransform>().localPosition -= new Vector3(cursorGrid, 0, 0);
                cursorHorizontalMove = false;
                StartCoroutine(EnableHorizontalCursorMove());
                //audioSource.PlayOneShot(spaceSelectionSFX);
            }
            else if (Input.GetAxisRaw("Vertical_Joy_2") > stickSensitivity && cursorVerticalMove && cursorPos.y < screenHeight)
            {
                controllerCursor.GetComponent<RectTransform>().localPosition += new Vector3(0, cursorGrid, 0);
                cursorVerticalMove = false;
                StartCoroutine(EnableVerticalCursorMove());
               // audioSource.PlayOneShot(spaceSelectionSFX);
            }
            else if (Input.GetAxisRaw("Vertical_Joy_2") < -stickSensitivity && cursorVerticalMove && cursorPos.y > 70)
            {
                controllerCursor.GetComponent<RectTransform>().localPosition -= new Vector3(0, cursorGrid, 0);
                cursorVerticalMove = false;
                StartCoroutine(EnableVerticalCursorMove());
                //audioSource.PlayOneShot(spaceSelectionSFX);
            }
        }
        //CONTROLS FOR MOVING SPELLS---------------------------------------------------------------------------------------------
        else if (p2Controller && !pause.GameIsPaused && !MovingTraps)
        {
            if (SpellCastDirection == SpellDirection.Instant)
            {
                Vector3 cursorPos = controllerCursor.GetComponent<RectTransform>().localPosition;
                if (Input.GetAxisRaw("Vertical_Joy_2") > stickSensitivity && cursorPos.y < -55)
                {
                    Vector3 pos = controllerCursor.transform.localPosition;
                    pos.z = 35;
                    
                    controllerCursor.transform.Translate(new Vector3(0f, Input.GetAxisRaw("Vertical_Joy_2") * freeRoamSpellSpeed, 0f));
                    
                }
                if (Input.GetAxisRaw("Vertical_Joy_2") < -stickSensitivity && cursorPos.y > -screenHeight)
                {
                    Vector3 pos = controllerCursor.transform.localPosition;
                    pos.z = 35;

                    controllerCursor.transform.Translate(new Vector3(0f, Input.GetAxisRaw("Vertical_Joy_2") * freeRoamSpellSpeed, 0f));

                }
                if (Input.GetAxisRaw("Horizontal_Joy_2") < -stickSensitivity)
                {
                    Vector3 pos = controllerCursor.transform.localPosition;
                    pos.z = 35;
                    if(controllerCursor.transform.localPosition.x > -screenWidth)
                    {
                        controllerCursor.transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal_Joy_2") * freeRoamSpellSpeed, 0f, 0f));
                    }
                }
                if(Input.GetAxisRaw("Horizontal_Joy_2") > stickSensitivity)
                {
                    Vector3 pos = controllerCursor.transform.localPosition;
                    pos.z = 35;
                    if (controllerCursor.transform.localPosition.x < screenWidth)
                    {
                        controllerCursor.transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal_Joy_2") * freeRoamSpellSpeed, 0f, 0f));
                    }
                }
            }
            else if(SpellCastDirection == SpellDirection.Right)
            {

                Vector3 cursorPos = controllerCursor.GetComponent<RectTransform>().localPosition;
                if (Input.GetAxisRaw("Vertical_Joy_2") > stickSensitivity && cursorVerticalMove && cursorPos.y < -55)
                {
                    controllerCursor.GetComponent<RectTransform>().localPosition += new Vector3(0, cursorGrid * spellBarSpeedMultiplier, 0);
                }
                else if (Input.GetAxisRaw("Vertical_Joy_2") < -stickSensitivity && cursorVerticalMove && cursorPos.y > -screenHeight)
                {
                    controllerCursor.GetComponent<RectTransform>().localPosition -= new Vector3(0, cursorGrid * spellBarSpeedMultiplier, 0);
                }
            }
            else if(SpellCastDirection == SpellDirection.Ceiling)
            {
                Vector3 cursorPos = controllerCursor.GetComponent<RectTransform>().localPosition;
                if (Input.GetAxisRaw("Horizontal_Joy_2") > stickSensitivity && cursorHorizontalMove && cursorPos.x < screenWidth)
                {
                    controllerCursor.GetComponent<RectTransform>().localPosition += new Vector3(cursorGrid * spellBarSpeedMultiplier, 0, 0);
                }
                else if (Input.GetAxisRaw("Horizontal_Joy_2") < -stickSensitivity && cursorHorizontalMove && cursorPos.x > -screenWidth)
                {
                    controllerCursor.GetComponent<RectTransform>().localPosition -= new Vector3(cursorGrid * spellBarSpeedMultiplier, 0, 0);
                }
            }
        }


    }

    IEnumerator EnableHorizontalCursorMove()
    {
        yield return new WaitForSeconds(cursorDelayHorizontal);
        cursorHorizontalMove = true;
    }

    IEnumerator EnableVerticalCursorMove()
    {
        yield return new WaitForSeconds(cursorDelayVertical);
        cursorVerticalMove = true;
    }
}
