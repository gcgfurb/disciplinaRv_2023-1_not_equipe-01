using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float airControl = 6.96f;
    public float jumpForce =5.33f;
    public float mouseSensitivity = 100.0f;
    public float forwardSpeed = 1.0f;
    public float sideSpeed = 1.0f;
    public float groundedFriction = 0.8f;

    private Rigidbody rb;
    private Collider collider;
    private bool isGrounded;
    private bool firstAirborneFrame;
    private Vector3 velocityOnJump;
    public float groundedTimer = 0f;
    public float airTimer = 0f;

    private Camera camera;
    private float xRotation = 0f;

    public float actualSpeed = 0f;

    private Vector3 wishDir;
    public float maxSpeed = 13.97f;
    public float acceleration = 8.8f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
Application.targetFrameRate = 60; 
        camera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        CheckGrounded();
        Move();
        rb.angularVelocity = Vector3.zero;

        Glide();
        Jump();
        groundedTimer += Time.deltaTime;
        RotatePlayer();
        RotateCamera();

        if (!isGrounded)
        {
            airTimer += Time.deltaTime;
            groundedTimer = 0f;
        }
        else
        {
            airTimer = 0f;
        }
    }

    private void CheckGrounded()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, collider.bounds.extents.y + 0.25f);

        if (wasGrounded && !isGrounded)
        {
            firstAirborneFrame = true;
        }
        else if (!wasGrounded && isGrounded)
        {
            velocityOnJump = Vector3.zero;
        }
    }

    private void Move()
    {
        if (groundedTimer > 0.2f)
        {
            actualSpeed = speed;
        }
        else
        {
            actualSpeed = speed - groundedTimer * 12;
        }

        // Adjust speed when holding the Control key
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            actualSpeed *= 0.5f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate the wishDir vector
        wishDir = (transform.right * horizontal * sideSpeed) + (transform.forward * vertical * forwardSpeed);
        wishDir.Normalize();

        // Calculate the projection of velocity on the wishDir
        float currentSpeed = Vector3.Dot(rb.velocity, wishDir);
        float addspeed = maxSpeed - currentSpeed;
        if (addspeed <= 0)
            return;

        float accelspeed = acceleration * Time.deltaTime * maxSpeed;
        if (accelspeed > addspeed)
            accelspeed = addspeed;

        Vector3 velocityChange = wishDir * accelspeed;

        if (isGrounded)
        {
            if(groundedTimer < 0.5f || horizontal * vertical == 0){
            groundedFriction = 0.4f+groundedTimer;
            if(groundedFriction> 0.8f || groundedFriction < 0f)
             {
             groundedFriction = 0.8f;

              }
            rb.velocity = Vector3.Scale(rb.velocity, new Vector3(groundedFriction, 1, groundedFriction));
           }else
           {
            groundedFriction = 0.8f;
rb.velocity = Vector3.Scale(rb.velocity, new Vector3(groundedFriction, 1, groundedFriction));
            }
 
           rb.velocity += velocityChange;
        }
        else
        {
            rb.AddForce(velocityChange*airControl);
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void Glide()
    {
        if (!isGrounded && Input.GetKey(KeyCode.Space) && rb.velocity.y < 0)
        {
            rb.useGravity = false;
        }
        else if (Input.GetKeyUp(KeyCode.Space) || isGrounded)
        {
            rb.useGravity = true;
        }
    }

    private void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up, mouseX);
    }

    private void RotateCamera()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}