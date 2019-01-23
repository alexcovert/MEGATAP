using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOne : MonoBehaviour {
	// test comment

    //camera
    [SerializeField] private CameraOneRotator cam;
    private int state = 1;

	//movement
	[SerializeField] private float jumpForce;
    [SerializeField] private float moveSpeed;
	[Range(0.0f, 1.0f)][SerializeField] private float inAirSpeed; // slow player's side-to-side movement in the air
	[SerializeField] private float fallMultiplier;
	[SerializeField] private float lowerJumpMultiplier;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float maxJumpVelocity;
    [SerializeField] private GameManager gameManager;
    private float inputAxis;

    //timer and death
    [SerializeField] private float time = 500f;
    private bool lose;
    public Text TimerText;
    

    private Vector3 tempVelocity;
    private bool isGrounded;
	private Rigidbody rb;

	private float movementMultiplier = 1f; // modify movement speed 

	void Start() {
		rb = GetComponent<Rigidbody> ();
        lose = false;
        SetTimerText();
        inputAxis = gameManager.GetInputAxis();
    }


	void OnCollisionEnter() {
		isGrounded = true;
        rb.velocity = tempVelocity;
	}

	void FixedUpdate () {
        state = cam.GetState();

		if (isGrounded) {
			movementMultiplier = 10;
		} else if (!isGrounded) {
			movementMultiplier = inAirSpeed;
		}

        inputAxis = gameManager.GetInputAxis();
        switch (state)
        {
            case 1:
                rb.AddForce(inputAxis * moveSpeed * movementMultiplier, 0, 0, ForceMode.Impulse);
                rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                break;
            case 2:
                rb.AddForce(0, 0, inputAxis * moveSpeed * movementMultiplier, ForceMode.Impulse);
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
                break;
            case 3:
                rb.AddForce(-inputAxis * moveSpeed * movementMultiplier, 0, 0, ForceMode.Impulse);
                rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                break;
            case 4:
                rb.AddForce(0, 0, -inputAxis * moveSpeed * movementMultiplier, ForceMode.Impulse);
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
                break;
        }

        //Return to 0 (Handle drag manually)
        if(Mathf.Abs(inputAxis) <= 0.5f && Mathf.Abs(inputAxis) >= 0f)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        //Return to max velocity
        if (rb.velocity.magnitude >= maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }

        //jump
        if (Input.GetButton("Jump_Joy_1") && isGrounded) {
			isGrounded = false;
            tempVelocity = rb.velocity;
            if (rb.velocity.magnitude >= maxJumpVelocity)
            {
                rb.velocity = rb.velocity.normalized * maxJumpVelocity;
            }
            rb.AddForce (0, jumpForce, 0, ForceMode.Impulse);
            
		}

        //crouch
		if (Input.GetButton("Crouch_Joy_1") && isGrounded) {
			Debug.Log("Crouching");
		}	
		if (Input.GetButtonUp("Crouch_Joy_1") && isGrounded) {
			Debug.Log("Done Crouching");
		}


        //fall faster
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump_Joy_1"))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowerJumpMultiplier - 1) * Time.deltaTime;
        }

        //timer countdown
        time -= Time.deltaTime;
        SetTimerText();
        if(time < 0)
        {
            lose = true;
            GameOver();
        }
    }

    void SetTimerText()
    {
        TimerText.text = "Time: " + time.ToString("F0");
    }

    public bool GameOver()
    {
        return lose;
    }
}
