using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOneMovement : MonoBehaviour {
    //movement vars serialized for designers
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpHeight;
    [Tooltip("How long the wall jump force lasts.")] [SerializeField] private float wallJumpTime;
    [Tooltip("WallJumpForce = jumpHeight / this")] [SerializeField] private float wallJumpDivider;
    [Tooltip("How far between -transform.fwd & transform.up the angle is.")] [SerializeField] private float wallJumpDirectionDivider;

    //other movement vars
    private Vector3 movementVector;
    private Vector3 wallJumpVector;
    public bool crouching;
    private bool grounded;
    private bool jumping;
    private bool landing;
    private bool wallJumping;
    private bool cantStandUp;
    //private bool slowed = false;
    private bool spedUp = false;
    public bool InputEnabled = true;

    //Default Movement Penalties
    private float SlowPenaltyTier1 = 1;
    private float CrouchPenalty = 1;
    private float StunPenalty = 1;
    private float SuperSpeed = 1;

    private float SlowJumpPenalty = 1;

    private float SuperJump = 1;

    //Movement Penalty Multiplier
    private float crouchSlow = 0.5f;

    //Pickup stuff
    [SerializeField] private Image[] pickupImages;
    [SerializeField] private Sprite pickupEmpty;

    //Control if player can have input
    private bool move = true;

    private float speed;
    private float jumpH; // change this when in sap etc.; set it back to jumpHeight when done
    private float jump;

    //camera
    [SerializeField] private CameraOneRotator cam;
    //[SerializeField] private float distanceFromGround = 2f;
    private int camOneState = 1;

    [SerializeField] private GameObject gameManager;

    // sound 
    [SerializeField] private AudioClip speedBoostSFX;
    [SerializeField] private AudioClip[] speedBoostYES;
    private AudioSource audioSource;
    private int yesVoice = 0;

    private float inputAxis; //used to get input axis from controller/keyboard
    private InputManager inputManager;

    private Rigidbody rb;

    private PauseMenu pause;
    private Animator animator;
    private CapsuleCollider col;
    private CapsuleCollider[] colArray;

    private SphereCollider[] sphere;
    private GhostTrail ghost;
    private bool once = false;

    // used for tutorial
    GameObject tutorialOverlay;

    private ParticleSystem dustParticles;
    private ParticleSystemRenderer[] particleRenderer;
    private ParticleSystemRenderer stun;
    private ParticleSystem sapBubbles;
    private ParticleSystemRenderer slowAura;
    private ParticleSystemRenderer slowSwirl;

    private ParticleSystemRenderer speedUpSwirl;
    private ParticleSystemRenderer speedUpBeams;

    private bool sap = false;

    private GameOverMenu gameOver;

    //For Petrify flicker
    private float PetrifyTime = -1;
    private float StunTimeInitial = 0;
    private bool unPetrify = true;

    //For slow effect flicker
    private float SlowSpellTime = -1;
    private float SlowTimeInitial = 0;
    private bool unSlow = true;
    private bool slowed = false;

    private void Awake()
    {
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        audioSource = GetComponent<AudioSource>();

        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in particles)
        {
            if (p.name == "DustParticles")
            {
                dustParticles = p;
            }

            if (p.name == "sap slow trail")
            {
                sapBubbles = p;
            }
        }
        ParticleSystemRenderer[] particleRenderer = GetComponentsInChildren<ParticleSystemRenderer>();
        foreach (ParticleSystemRenderer p in particleRenderer)
        {
            if (p.name == "green swirls 2")
            {
                slowAura = p;
            }
            if (p.name == "animated swirls")
            {
                slowSwirl = p;
            }
            if (p.name == "Stun Particles")
            {
                stun = p;
            }
            if (p.name == "blue swirls")
            {
                speedUpSwirl = p;
            }
            if (p.name == "Beams")
            {
                speedUpBeams = p;
            }
        }
    }

    void Start() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        //checkControllers = gameManager.GetComponent<CheckControllers>();
        col = GetComponent<CapsuleCollider>();
        colArray = GetComponents<CapsuleCollider>();
        stun = GetComponentInChildren<ParticleSystemRenderer>();
        pause = gameManager.GetComponent<PauseMenu>();
        gameOver = gameManager.GetComponent<GameOverMenu>();
        sphere = GetComponents<SphereCollider>();
        ghost = GetComponent<GhostTrail>();

        stun.enabled = false;
        sapBubbles.Stop();
        slowAura.enabled = false;
        slowSwirl.enabled = false;
        speedUpSwirl.enabled = false;
        speedUpBeams.enabled = false;

        crouching = false;
        animator.SetBool("Grounded", grounded);

        speed = (moveSpeed * SlowPenaltyTier1 * StunPenalty * CrouchPenalty) * SuperSpeed;
        jumpH = jumpHeight;
        jump = (jumpHeight * SlowJumpPenalty) * SuperJump;

        move = true;
        tutorialOverlay = GameObject.Find("ToolTipBottomGoal");
    }

    private void Update()
    {
        if (!gameOver.GameOver)
        {
            camOneState = cam.GetState();
            grounded = GetComponentInChildren<PlayerGrounded>().IsGrounded();
            if (move == true && !pause.GameIsPaused && InputEnabled && !gameOver.GameOver)
            {
                if (Mathf.Abs(inputManager.GetAxis(InputCommand.BottomPlayerMoveStick)) > 0.4)
                {
                    inputAxis = inputManager.GetAxis(InputCommand.BottomPlayerMoveStick);
                }
                else if (Mathf.Abs(inputManager.GetAxis(InputCommand.BottomPlayerMoveKeyboard)) > 0)
                {
                    inputAxis = inputManager.GetAxis(InputCommand.BottomPlayerMoveKeyboard);
                }
                else
                {
                    inputAxis = 0;
                }
                //jumping
                if (inputManager.GetButtonDown(InputCommand.BottomPlayerJump) && grounded && crouching == false)
                {
                    jumping = true;
                }

                if (inputManager.GetButtonDown(InputCommand.BottomPlayerJump) && grounded && crouching == true && cantStandUp == false)
                {
                    jumping = true;
                }

                //crouch
                if (inputManager.GetButtonDown(InputCommand.BottomPlayerCrouch) && grounded)
                {
                    crouching = true;
                }
                if (inputManager.GetButtonUp(InputCommand.BottomPlayerCrouch) || (!inputManager.GetButton(InputCommand.BottomPlayerCrouch) && cantStandUp == false))
                {
                    if (cantStandUp == true)
                    {
                        crouching = true;
                        CrouchPenalty = crouchSlow;
                    }
                    if (cantStandUp == false)
                    {
                        crouching = false;
                        CrouchPenalty = 1;
                    }

                }
                // Animation parameters update
                animator.SetBool("Jumping", jumping);
                if (jumping)
                {
                    animator.SetBool("Running", false);
                }
                //animator.SetBool("Running", move);
                animator.SetBool("Stunned", false);
                stun.enabled = false;
            }

            switch (camOneState)
            {
                case 1:
                    movementVector = new Vector3(inputAxis * speed, rb.velocity.y, 0);
                    if (inputAxis > 0)
                    {
                        transform.eulerAngles = new Vector3(0, 90, 0);
                        animator.SetFloat("Velocity", speed);
                        if (grounded) animator.SetBool("Running", true);
                    }
                    else if (inputAxis < 0)
                    {
                        transform.eulerAngles = new Vector3(0, 270, 0);
                        animator.SetFloat("Velocity", -speed);

                        if (grounded) animator.SetBool("Running", true);
                    }
                    else
                    {
                        animator.SetFloat("Velocity", 0);

                        if (grounded) animator.SetBool("Running", false);
                    }
                    rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
                    break;
                case 2:
                    movementVector = new Vector3(0, rb.velocity.y, inputAxis * speed);
                    if (inputAxis > 0)
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        animator.SetFloat("Velocity", speed);
                        if (grounded) animator.SetBool("Running", true);
                    }
                    else if (inputAxis < 0)
                    {
                        transform.eulerAngles = new Vector3(0, 180, 0);
                        animator.SetFloat("Velocity", -speed);
                        if (grounded) animator.SetBool("Running", true);
                    }
                    else
                    {
                        animator.SetFloat("Velocity", 0);
                        if (grounded) animator.SetBool("Running", false);
                    }
                    rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
                    break;
                case 3:
                    movementVector = new Vector3(-inputAxis * speed, rb.velocity.y, 0);
                    if (inputAxis > 0)
                    {
                        transform.eulerAngles = new Vector3(0, 270, 0);
                        animator.SetFloat("Velocity", -speed);
                        if (grounded) animator.SetBool("Running", true);
                    }
                    else if (inputAxis < 0)
                    {
                        transform.eulerAngles = new Vector3(0, 90, 0);
                        animator.SetFloat("Velocity", speed);
                        if (grounded) animator.SetBool("Running", true);
                    }
                    else
                    {
                        animator.SetFloat("Velocity", -rb.velocity.x);
                        if (grounded) animator.SetBool("Running", false);
                    }
                    rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
                    break;
                case 4:
                    movementVector = new Vector3(0, rb.velocity.y, -inputAxis * speed);
                    if (inputAxis > 0)
                    {
                        transform.eulerAngles = new Vector3(0, 180, 0);
                        animator.SetFloat("Velocity", -speed);
                        if (grounded) animator.SetBool("Running", true);
                    }
                    else if (inputAxis < 0)
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        animator.SetFloat("Velocity", speed);
                        if (grounded) animator.SetBool("Running", true);
                    }
                    else
                    {
                        animator.SetFloat("Velocity", 0);
                        if (grounded) animator.SetBool("Running", false);
                    }
                    rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
                    break;
            }

            if (crouching == true)
            {
                CrouchPenalty = crouchSlow;
                col.height = 2.25f;
                col.center = new Vector3(0, 1.1f, 0);
                colArray[1].height = 2f;
                colArray[1].center = new Vector3(0, 1f, 0.21f);
                sphere[0].center = new Vector3(0, 1f, 0);
            }

            if (crouching == false || grounded == false)
            {
                col.height = 4.5f;
                col.center = new Vector3(0, 2.2f, 0);
                colArray[1].height = 4f;
                colArray[1].center = new Vector3(0, 2.2f, 0.21f);
                sphere[0].center = new Vector3(0, 3f, 0);
            }

            if(!pause.GameIsPaused) Move();
            // remove tutorial overlay
            tutorialOverlay = GameObject.Find("ToolTipBottomGoal");
            if (tutorialOverlay != null && tutorialOverlay.activeSelf == true && inputManager.GetButtonDown(InputCommand.BottomPlayerJump))
            {
                tutorialOverlay.SetActive(false);
            }
            cantStandUp = gameObject.GetComponentInChildren<Colliding>().GetCollision();

            if (!pause.GameIsPaused && !gameOver.GameOver) Move();

            if (spedUp == false && GameObject.FindWithTag("Player").GetComponent<PlayerOneStats>().pickupCount >= 3)
            {
                speedUpBeams.enabled = true;
                speedUpSwirl.enabled = true;
            }

            // initiate speed up
            if (GameObject.FindWithTag("Player").GetComponent<PlayerOneStats>().pickupCount >= 3 && inputManager.GetButtonDown(InputCommand.BottomPlayerBoost) && once == false)
            {
                int VoiceYES = yesVoice % 2;
                yesVoice++;
                AudioClip clip = speedBoostYES[VoiceYES];
                audioSource.PlayOneShot(clip);
                spedUp = true;
                audioSource.PlayOneShot(speedBoostSFX);
                speedUpBeams.enabled = false;
                speedUpSwirl.enabled = false;
                ghost.On = true;
                once = true;
                StartCoroutine(SpeedBoost(GameObject.FindWithTag("PickUp").GetComponent<PickUp>().speedUpMultiplier, GameObject.FindWithTag("PickUp").GetComponent<PickUp>().speedUpDuration, GameObject.FindWithTag("PickUp").GetComponent<PickUp>().speedUpJumpMultipler));
            }
            //New Speed Function
            speed = (moveSpeed * SlowPenaltyTier1 * StunPenalty * CrouchPenalty) * SuperSpeed;
            //New Jump Function
            jump = (jumpHeight * SlowJumpPenalty) * SuperJump;

            //Petrify flicker fix
            StunTimeInitial += Time.deltaTime;
            if (StunTimeInitial >= PetrifyTime)
            {
                unPetrify = true;
            }
            else
            {
                unPetrify = false;
            }

            //Slow flicker fix
            SlowTimeInitial += Time.deltaTime;
            if (SlowTimeInitial >= SlowSpellTime)
            {
                unSlow = true;
            }

            if (unSlow == false)
            {
                slowSwirl.enabled = true;
                slowAura.enabled = true;
            }
            else
            {
                slowSwirl.enabled = false;
                slowAura.enabled = false;
            }

            if (slowed == true)
            {
                if (sap == false)
                {
                    sapBubbles.Play();
                    sap = true;
                }
            }
            else
            {
                sap = false;
                sapBubbles.Stop();
            }

            //WallJump Check at feet
            //Debug.DrawRay(transform.position, transform.forward, Color.green);
            //WallJump Check at knee
            //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.forward, Color.blue);
            //WallJump Check at chest
            //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), transform.forward, Color.red);
            //WallJump Check at nose/head
            //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 3, transform.position.z), transform.forward, Color.yellow);

            //Distance from feet to platform
            //Debug.DrawRay(transform.position, -transform.up, Color.yellow, distanceFromGround);

            animator.SetBool("Grounded", grounded);
            animator.SetBool("Crouched", crouching);
            animator.SetFloat("YVelocity", rb.velocity.y);
        }
        else if(gameOver.GameOver)
        {
            movementVector = Vector3.zero;
            move = false;
            jumping = false;
            crouching = false;

        }
        
    }


    private void Move()
    {
        //Stuff that used to be in fixedupdate
        if (jumping)
        {
            crouching = false;
            movementVector = new Vector3(movementVector.x, jump, movementVector.z);
            if (move == true && wallJumping == false)
            {
                animator.Play("Armature|JumpStart", 0);
            }
            jumping = false;
            //landing = false;
            speed = (moveSpeed * SlowPenaltyTier1 * StunPenalty * CrouchPenalty) * SuperSpeed;
            //animator.SetBool("Landing", landing);
            //StartCoroutine(CheckLanding());
        }
        else if (crouching)
        {
            //TODO
        }
        if (move == false)
        {
            movementVector = new Vector3(0, Physics.gravity.y * 0.255f, 0);
            stun.enabled = true;
            StunPenalty = 0;
        }
        else
        {
            StunPenalty = 1;
        }

        if(slowed == false && unSlow == true)
        {
            SlowPenaltyTier1 = 1;
            SlowJumpPenalty = 1;
            sap = false;
            sapBubbles.Stop();
            slowSwirl.enabled = false;
            slowAura.enabled = false;
        }

        if (!wallJumping) rb.velocity = movementVector;
        else rb.velocity = wallJumpVector;
    }


    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Trap" || collision.gameObject.tag == "TrapHitbox")
        {
            //CHECK WALL JUMP
            RaycastHit hit;
            RaycastHit downHit;
            bool raycastDown = Physics.Raycast(transform.position, -transform.up, out downHit, 1);
            if ((Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.forward, out hit, 2f) || Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.forward, out hit, 2f) || 
                    Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), transform.forward, out hit, 2f) 
                    || Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.forward, out hit, 2f)) && !raycastDown)
            {
                if (hit.transform.tag == "Platform" && inputManager.GetButtonDown(InputCommand.BottomPlayerJump) && grounded == false && move == true)
                {
                    animator.Play("Wall Jump", 0);
                    wallJumpVector = (-transform.forward + transform.up / wallJumpDirectionDivider).normalized * (jumpH / wallJumpDivider);
                    wallJumping = true;
                    //jumping = false;
                    dustParticles.Play();
                    StartCoroutine(DisableWallJump());
                }
            }
            //NOT WALL JUMPING
        }
    }

    private IEnumerator DisableWallJump()
    {
        yield return new WaitForSeconds(wallJumpTime);
        wallJumping = false;
        wallJumpVector = Vector3.zero;
    }

    //Called from pause script to re-enable input after pressing "Resume"
    public IEnumerator ResumeInput()
    {
        yield return new WaitForSeconds(0.5f);
        InputEnabled = true;
    }

    public IEnumerator SpeedBoost(float speedUpMultiplier, float speedUpDuration, float speedUpJumpMultipler)
    {
        SuperSpeed = speedUpMultiplier;
        SuperJump = speedUpJumpMultipler;

        float timePerPickup = speedUpDuration / 3;

        yield return new WaitForSeconds(timePerPickup);
        pickupImages[2].sprite = pickupEmpty;
        pickupImages[2].rectTransform.sizeDelta = new Vector2(50, 40);
        yield return new WaitForSeconds(timePerPickup);
        pickupImages[1].sprite = pickupEmpty;
        pickupImages[1].rectTransform.sizeDelta = new Vector2(50, 40);
        yield return new WaitForSeconds(timePerPickup);
        pickupImages[0].sprite = pickupEmpty;
        pickupImages[0].rectTransform.sizeDelta = new Vector2(50, 40);

        spedUp = false;
        once = false;
        SuperSpeed = 1;
        SuperJump = 1;
        ghost.On = false;
        gameObject.GetComponent<PlayerOneStats>().pickupCount = 0;
    }

   /* private IEnumerator CheckLanding()
    {
        while (landing == false)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit, distanceFromGround) && grounded == false)
            {
                landing = true;
                animator.SetBool("Landing", landing);
            }
            yield return null;
        }
    }*/

    /////////////////////////////////////////////
    // GETTERS AND SETTERS                     //
    /////////////////////////////////////////////
    public int GetState()
    {
        return camOneState;
    }

    public float GetJumpHeight()
    {
        return jumpH;
    }

    public void SetJumpHeight(float j)
    {
        jumpH = j;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetSpedUp(bool s)
    {
        spedUp = s;
    }

    public bool GetSpedUp()
    {
        return spedUp;
    }

    public float GetConstantSpeed()
    {
        return moveSpeed;
    }

    public float GetConstantJumpHeight()
    {
        return jumpHeight;
    }

    public void SetMove(bool m)
    {
        move = m;
    }

    public Animator GetAnim()
    {
        return animator;
    }

    public float GetInputAxis()
    {
        return inputAxis;
    }

    public bool GetSlowed()
    {
        return slowed;
    }

    public void SetSlowed(bool slow)
    {
        slowed = slow;
    }

    public bool IsCrouched()
    {
        return crouching;
    }

    public bool IsStunned()
    {
        return !move;
    }

    //For effect flickering
    public float GetPetrifyTime()
    {
        return PetrifyTime;
    }

    public void SetPetrifyTime(float f)
    {
        PetrifyTime = f;
    }

    public float GetStunTimeInitial()
    {
        return StunTimeInitial;
    }
    
    public void SetStunTimeInitial(float f)
    {
        StunTimeInitial = f;
    }

    public bool GetUnPetrify()
    {
        return unPetrify;
    }
    
    public void SetUnPetrify(bool b)
    {
        unPetrify = b;
    }


    public float GetSlowSpellTime()
    {
        return SlowSpellTime;
    }

    public void SetSlowSpellTime(float f)
    {
        SlowSpellTime = f;
    }

    public float GetSlowTimeInitial()
    {
        return SlowTimeInitial;
    }

    public void SetSlowTimeInitial(float f)
    {
        SlowTimeInitial = f;
    }

    public bool GetUnSlow()
    {
        return unSlow;
    }

    public void SetUnSlow(bool b)
    {
        unSlow = b;
    }

    //Speed Penalties and Bonuses
    public float GetSlowPenalty()
    {
        return SlowPenaltyTier1;
    }
    
    public void SetSlowPenalty(float penalty)
    {
        SlowPenaltyTier1 = penalty;
    }

    public float GetSlowJumpPenalty()
    {
        return SlowJumpPenalty;
    }
    
    public void SetSlowJumpPenalty(float penalty)
    {
        SlowJumpPenalty = penalty;
    }
}
