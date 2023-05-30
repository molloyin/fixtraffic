using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Car Controller for player car
public class CarController : MonoBehaviour
{
    public Rigidbody theRB;

    public float maxSpeed;

    public float forwardAcceleration = 8f, reverseAcceleration = 4f;
    private float speedInput;

    public float turnStrength = 180f;
    private float turnInput;

    private bool grounded;
    public Transform groundRayPoint, groundRayPoint2;
    public LayerMask whatIsGround;
    public float groundRayLength = 0.75f;

    private float dragOnGround;
    public float gravityMod = 10f;

    public Transform leftFrontWheel, rightFrontWheel;
    public float maxWheelTurn = 25f;

    public ParticleSystem[] dustTrail;
    public float maxEmission = 25f, emissionFadeSpeed = 20f;
    private float emissionRate;

    public AudioSource engineSound, skidSound;
    public float skidFadeSpeed;
    

    public InputAction playerMovement;
    Vector2 moveDir = Vector2.zero;

    public float timer;

    public void OnEnable()
    {
        playerMovement.Enable();
    }

    public void OnDisable()
    {
        playerMovement.Disable();
    }


    // Start is called before the first frame update
    void Start()
    {
        theRB.transform.parent = null;

        //set initial dragOnGround to Unity default
        this.dragOnGround = theRB.drag;

        
    }

    // Update is called once per frame
    void Update()
    {
        //takes player input as Vector2
        moveDir = playerMovement.ReadValue<Vector2>();

        speedInput = 0f;

        if (moveDir[1] > 0)
        {
            speedInput = moveDir[1] * forwardAcceleration;

        }
        else if (moveDir[1] < 0)
        {
            speedInput = moveDir[1] * reverseAcceleration;
        }

        turnInput = moveDir[0];

        //turning wheels on car model
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn - 180), leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);

        //control particle emissions with fade out
        emissionRate = Mathf.MoveTowards(emissionRate, 0f, emissionFadeSpeed * Time.deltaTime);

        //dust emits from car tyres where there is sharp turning or when vehicle is accelerating (not at full speed though)
        if(grounded && (Mathf.Abs(turnInput) > 0.5f || (theRB.velocity.magnitude < maxSpeed * 0.5f && theRB.velocity.magnitude != 0)))
        {
            emissionRate = maxEmission; 
        }

        //dust emission stops given velocity
        if(theRB.velocity.magnitude <= 0.6f)
        {
            emissionRate = 0;
        }

        //
        for(int i = 0; i < dustTrail.Length; i++)
        {
            var emissionModule = dustTrail[i].emission;
            emissionModule.rateOverTime = emissionRate;
        }

        if(engineSound != null)
        {
            engineSound.pitch = 1.0f + (theRB.velocity.magnitude / maxSpeed) * 2.9f;

        }

        PlaySkidSound();
        /*
        if(skidSound != null)
        {
            if((Mathf.Abs(turnInput) > 0.1) && speedInput != 0 && turnStrength > 300)
            {
                //Debug.Log("TURN INPUT: "+ turnInput);
                //Debug.Log("SPEED INPUT: " + speedInput);
                skidSound.volume = 1.0f;
            }
            else
            {
                skidSound.volume = Mathf.MoveTowards(skidSound.volume, 0f, skidFadeSpeed * Time.deltaTime);
            }
        }*/
    }

    private void FixedUpdate()
    {
        grounded = false;

        RaycastHit hit;
        Vector3 normalTarget = Vector3.zero;

        //creates normal target based on ground ray hit point
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            //normal to the RaycastHit hit
            normalTarget = hit.normal;
        }

        //when first ground ray point touches incline but second point does not, the hit.normal/2 is a opposite direction/negative number making the normalTarget smaller, car less of inclineessentially 
        if (Physics.Raycast(groundRayPoint2.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            normalTarget = normalTarget + hit.normal / 2;
        }

        //when on ground, rotate to normal
        if (grounded)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation;
        }

        //accelerates car forward
        if (grounded)
        {
            theRB.drag = dragOnGround;//sets drag to the ground drag value
            theRB.AddForce(transform.forward * speedInput * 500f);//accelerates car forward
        }
        else
        {
            theRB.drag = 0.05f;//sets drag to low value
            theRB.AddForce(-Vector3.up * gravityMod * 80f);//create downward grvitational force
        }

        //prevents car travelling above maximum maxSpeed
        if(theRB.velocity.magnitude > maxSpeed)
        {
            theRB.velocity = theRB.velocity.normalized * maxSpeed;
        }

        //remove debug code at the end
        Debug.Log(theRB.velocity.magnitude);

        //moved from Update() to FixedUpdate()
        transform.position = theRB.position;

        turnStrength = 220f;//return turn strength to baseline
        if (grounded && moveDir[1] != 0)
        {
            //increases turn strength at low velocity to allow navigation of intersections
            if (theRB.velocity.magnitude < maxSpeed * 0.3f && theRB.velocity.magnitude != 0)
            {
                turnStrength *= 5;
            }
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude / maxSpeed), 0f));
        }

    }

    public void PlaySkidSound()
    {
        if (skidSound != null)
        {
            if ((Mathf.Abs(turnInput) > 0.1) && speedInput != 0 && turnStrength > 300)
            {
                //Debug.Log("TURN INPUT: "+ turnInput);
                //Debug.Log("SPEED INPUT: " + speedInput);
                skidSound.volume = 1.0f;
            }
            else
            {
                skidSound.volume = Mathf.MoveTowards(skidSound.volume, 0f, skidFadeSpeed * Time.deltaTime);
            }
        }
    }


}
