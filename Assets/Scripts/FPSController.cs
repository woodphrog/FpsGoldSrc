using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

//Code from GDM, Mcgill
//Contains code from http://adrianb.io/2015/02/14/bunnyhop.html
public class FPSController : MonoBehaviour
{
    public GameObject cameraSubObject;
    public GameObject checkpointManager;

    private InputActions inputActions;
    private CharacterController charController;


    public Vector3 respawnPoint;

    public float lookSensitivity = 50f;

    private float pitch = 0f;
    private float yaw = 0f;

    private float roll = 0f;

    // movement code
    public float maximumSpeed = 7f; //Maximum linear speed
    public float maximumFinalSpeed = 5f; //Maximum speed with strafing accounted
    public float maximumAcceleration = 1f;
    public float ctFriction;

    //Velocity
    private Vector3 velocity;
    private Vector3 wishDir;

    //Gravity
    public float gravity = -9.8f;
    public float playerHeight = 0.3f;
    public bool isGrounded = false;
    public LayerMask groundLayer;

    public float jumpHeight = 1.2f;
    public GameObject groundCheck;


    private void FixedUpdate()
    {
        var isHit = Physics.Raycast(groundCheck.transform.position, Vector3.down, out var hit, float.MaxValue,
            layerMask: groundLayer);
        if (isHit)
        {
            Debug.DrawRay(hit.point, hit.normal.normalized, Color.green, 1f);
        }
    }


    public void SetRespawnPoint(Vector3 respawnPoint)
    {
        this.respawnPoint = respawnPoint;
    }

    public Vector3 GetRespawnPoint()
    {
        return this.respawnPoint;
    }


    private Vector3 Accelerate(Vector3 accelerationDirection, Vector3 previousVelocity, float acceleration,
        float maximumVelocity)
    {
        var projectionVelocity = Vector3.Dot(previousVelocity, accelerationDirection);
        var acceleratedVelocity = acceleration;

        if (projectionVelocity + acceleratedVelocity > maximumVelocity)
            acceleratedVelocity = maximumVelocity - projectionVelocity;
        //return new Vector3() { x=5, y=0, z=5};
        return previousVelocity + accelerationDirection * acceleratedVelocity;
    }


    private void Death()
    {
        GetComponent<CharacterController>().enabled = false;
        transform.position = respawnPoint; //= new Vector3(0f,5f,0f);
        cameraSubObject.transform.position = respawnPoint; //= new Vector3(0f,5f,0f);
        GetComponent<CharacterController>().enabled = true;
    }


    private void OnJump()
    {
        //var moveDelta = inputActions.Player.Movement.ReadValue<Vector2>();
        //Debug.Log("Jumping!");
        //if (moveDelta.magnitude == 0) return;
        //var trans = transform;
        //var translationDelta = (trans.right * moveDelta.x) + (trans.forward * moveDelta.y);
        //charController.Move(translationDelta * movementSpeed * Time.deltaTime);
        isGrounded = Physics.CheckSphere(groundCheck.transform.position, 0.2f, groundLayer,
            QueryTriggerInteraction.Ignore);
        if (isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -1f * gravity);
            //charController.Move(velocity * Time.deltaTime);
        }
    }


    private static void OnShoot()
    {
        //Debug.Log("Shooting!");
    }


    private void OnEnable()
    {
        inputActions ??= new InputActions();
        inputActions.Enable();

        inputActions.Player.Jump.performed += eventCtx => OnJump();
        inputActions.Player.Shoot.performed += eventCtx => OnShoot();
        inputActions.Player.Scroll.performed += eventCtx => OnJump();
#if DEBUG
        inputActions.Player.DebugPop.performed += evenCtx =>
        {
            checkpointManager.GetComponent<CheckpointManager>().Pop();
        };
        inputActions.Player.DebugRespawn.performed += evenCtx => { Death(); };
#endif
    }

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        charController = GetComponent<CharacterController>();
        if (charController == null)
        {
            Debug.LogError("Missing character controller on player!");
        }
    }


    // Update is called once per frame
    private void Update()
    {
        //Rotating camera with look axis
        var lookDelta = inputActions.Player.Look.ReadValue<Vector2>();
        //if there is any new look
        //Debug.Log(transform.position);
        if (lookDelta.magnitude != 0)
        {
            lookDelta *= lookSensitivity * Time.deltaTime;

            pitch -= lookDelta.y;
            yaw += lookDelta.x;

            pitch = Mathf.Clamp(pitch, -89f, 89f);

            cameraSubObject.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
            transform.Rotate(Vector3.up * lookDelta.x);
        }


        Debug.DrawRay(Vector3.zero, Vector3.forward, Color.green);

        isGrounded = Physics.CheckSphere(groundCheck.transform.position, 0.2f, groundLayer,
            QueryTriggerInteraction.Ignore);
        //for movement for now
        var moveDelta = inputActions.Player.Movement.ReadValue<Vector2>();
        //if there is new movement

#if false
        if (moveDelta.magnitude != 0)
                {
                                
                    //var translationDelta = (transform.right * moveDelta.x) + (transform.forward * moveDelta.y);
                    var wishDir = (transform.right * moveDelta.x) + (transform.forward * moveDelta.y);
                    Debug.DrawRay(transform.position,wishDir,Color.red, 1.0f);
                    
                    var cc = GetComponent<CharacterController>();
                    var currVel = cc.velocity;
                    //currVel.Normalize();
                    
                    
                    Debug.Log(transform.position);
                    var currSpeed = Vector3.Dot(currVel, wishDir);
                    
                    var add_speed = Math.Min((15f - Math.Abs(currSpeed)),(maximumSpeed));
                    
                    velocity.x = currVel.x +  wishDir.x * add_speed;
                    velocity.z = currVel.z +  wishDir.z * add_speed;
                    
                    //velocity.x += add_speed * wishDir.x;
                    //velocity.z += add_speed * wishDir.z;
                    
                    //velocity += add_speed * wishDir;//this
                    //velocity.x = 1.4f*(currVel.x + add_speed  * wishDir.x);
                    //velocity.z = 1.4f*(currVel.z + add_speed * wishDir.z);
                    var tmpV2 = new Vector2
                    {
                        x = velocity.x,
                        y = velocity.z
                    };
                    
                    //tmpV2.Normalize();
        
                    //velocity.x = tmpV2.x* 0.5f;
                    //velocity.z = tmpV2.y * 0.5f;
                    velocity = 30f * velocity.normalized;//helikopter helikopter
                    Debug.DrawRay(transform.position,currVel,Color.blue, 1.0f);
                    //velocity.Normalize();
                    //charController.Move(velocity * Time.deltaTime);
                    //charController.Move(velocity*Time.deltaTime);
        
                }
#else
        if (moveDelta.magnitude != 0)
        {
            var wishDir = (transform.right * moveDelta.x) + (transform.forward * moveDelta.y);
            var cc = GetComponent<CharacterController>();
            var prVel = cc.velocity;
            var speed = prVel.magnitude;
            Debug.DrawRay(transform.position, wishDir, Color.red, 1.0f);
            Debug.DrawRay(transform.position, prVel, Color.blue, 1.0f);
            switch (isGrounded)
            {
                case true:
                {
                    if (speed != 0)
                    {
                        var drop = speed * ctFriction; // * Time.deltaTime;

                        prVel *= Mathf.Max(speed - drop, 0) / speed;
                    }

                    var tmp3 = Accelerate(wishDir, prVel, maximumAcceleration, maximumSpeed);
                    if (tmp3.magnitude > maximumFinalSpeed)
                    {
                        tmp3 = maximumFinalSpeed * tmp3.normalized;
                    }

                    velocity.x = tmp3.x;
                    velocity.z = tmp3.z;
                    //GetComponent<CharacterController>().velocity.Set(velocity.x, velocity.y, velocity.z);
                    break;
                }
                case false:
                {
                    var tmp3 = Accelerate(wishDir, prVel, maximumAcceleration, maximumSpeed);
                    if (tmp3.magnitude > maximumFinalSpeed)
                    {
                        tmp3 = maximumFinalSpeed * tmp3.normalized;
                    }


                    velocity.x = tmp3.x;
                    velocity.z = tmp3.z;
                    //GetComponent<CharacterController>().velocity.Set(velocity.x, velocity.y, velocity.z);
                    break;
                }
            }
        }


#endif


        //increment downwards vel
        velocity.y += 2.5f * gravity * Time.deltaTime;
        // if exists obj within sphere around us, is part of ground layer mask
        //reset down vel to 0


        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
            velocity.x *= 0.89f;
            velocity.z *= 0.89f;
        }


        /*var isJumping = inputActions.Player.Jump.triggered;
        
        if (isJumping && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -1f * gravity);
        }
*/
        charController.Move(velocity * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            Death();
        }
    }
}