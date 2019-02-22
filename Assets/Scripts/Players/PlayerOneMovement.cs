using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOneMovement : MonoBehaviour {
    //movement vars serialized for designers
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpHeight;

    //other movement vars
    private Vector3 movementVector;
    private bool crouching;
    private bool grounded;
    private bool jumping;
    private bool landing;

    //Control if player can have input
    private bool move;

    private float speed; //Change this when crouching, etc.; set it back to moveSpeed when done
    private float jumpH; // change this when in sap etc.; set it back to jumpHeight when done

    //camera
    [SerializeField] private CameraOneRotator cam;
    [SerializeField] private float distanceFromGround = 2f;
    private int camOneState = 1;

    [SerializeField] private GameObject gameManager;

    private float inputAxis; //used to get input axis from controller/keyboard

	private Rigidbody rb;


    private CheckControllers checkControllers;
    private Animator animator;

    void Start() {
		rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        checkControllers = gameManager.GetComponent<CheckControllers>();

        speed = moveSpeed;
        jumpH = jumpHeight;

        move = true;
    }

    private void Update()
    {
        camOneState = cam.GetState();

        if (move == true)
        {
            inputAxis = checkControllers.GetInputAxis();

            //jumping
            if (Input.GetButtonDown("Jump_Joy_1") && grounded)
            {
                jumping = true;
                animator.SetTrigger("Jump");
                animator.SetBool("Running", false);
            }

            //crouch
            if (Input.GetButton("Crouch_Joy_1") && grounded)
            {
                crouching = true;
            }
            if (Input.GetButtonUp("Crouch_Joy_1") && grounded)
            {
                crouching = false;
            }

        }

        switch (camOneState)
        {
            case 1:
                movementVector = new Vector3(inputAxis * speed, rb.velocity.y, 0);
                if (inputAxis > 0)
                {
                    transform.eulerAngles = new Vector3(0, 90, 0);
                    animator.SetFloat("Velocity", speed);
                    if (grounded)
                    {
                        animator.SetBool("Running", true);
                    }
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
                    animator.SetBool("Running", false);
                }
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
                break;
            case 2:
                movementVector = new Vector3(0, rb.velocity.y, inputAxis * speed);
                if (inputAxis > 0)
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    animator.SetFloat("Velocity", speed);
                    animator.SetBool("Running", true);
                }
                else if (inputAxis < 0)
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                    animator.SetFloat("Velocity", speed);
                    animator.SetBool("Running", true);
                }
                else
                {
                    animator.SetFloat("Velocity", 0);
                    animator.SetBool("Running", false);
                }
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
                break;
            case 3:
                movementVector = new Vector3(-inputAxis * speed, rb.velocity.y, 0);
                if (inputAxis > 0)
                {
                    transform.eulerAngles = new Vector3(0, 270, 0);
                    animator.SetFloat("Velocity", speed);
                    animator.SetBool("Running", true);
                }
                else if (inputAxis < 0)
                {
                    transform.eulerAngles = new Vector3(0, 90, 0);
                    animator.SetFloat("Velocity", -speed);
                    animator.SetBool("Running", true);
                }
                else
                {
                    animator.SetFloat("Velocity", 0);
                    animator.SetBool("Running", false);
                }
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
                break;
            case 4:
                movementVector = new Vector3(0, rb.velocity.y, -inputAxis * speed);
                if (inputAxis > 0)
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                    animator.SetFloat("Velocity", -speed);
                    animator.SetBool("Running", true);
                }
                else if (inputAxis < 0)
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    animator.SetFloat("Velocity", speed);
                    animator.SetBool("Running", true);
                }
                else
                {
                    animator.SetFloat("Velocity", 0);
                    animator.SetBool("Running", false) ;
                }
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
                break;
        }
    }

    private void FixedUpdate()
    {
        if(jumping)
        {
            movementVector = new Vector3(movementVector.x, jumpH, movementVector.z);
            animator.Play("Armature|JumpStart", 0);
            jumping = false;
            landing = false;
        }
        else if(crouching)
        {
            //TODO
        }
        if (move == false)
        {
            movementVector = new Vector3(0, movementVector.y, 0);
        }
        rb.velocity = movementVector;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out hit, distanceFromGround, LayerMask.GetMask("Platform")) && grounded == false)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.up) * hit.distance, Color.yellow);
            landing = true;
        }
        else
        {
        }
        
        animator.SetBool("Landing", landing);
        animator.SetBool("Grounded", grounded);
        animator.SetFloat("YVelocity", rb.velocity.y);
    }


    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Platform")
        {
            grounded = true;
            animator.SetBool("Jumping", false);
        }
        else if (Physics.Raycast(transform.position, -transform.right, 1))
        {
        }
        else
        {

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            grounded = false;
        }
    }

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

    public void SetSpeed(float s)
    {
        speed = s;
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
}
