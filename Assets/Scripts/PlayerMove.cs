using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Basic movement variables
    [SerializeField] float horizontalMoveSpeed;
    [SerializeField] float jumpForce;
    bool isGrounded = true;
    bool jump;
    bool jumpCancel = false;
    bool doubleJump = false;
    bool isJumping = false;
    bool isDoubleJumping = false;
    [SerializeField] float gravityMultiplier = 3f;
    [SerializeField] Rigidbody2D _rb;
    float horizontalMove;
    float verticalMove;
    float climbSpeed = 5f;

    bool isRunning=false;
    [SerializeField] float runSpeed = 2f;

    bool isClimbing = false;

    // Powerup movement variables
    [SerializeField] bool canDoubleJump; 
    [SerializeField] bool canInfiniteJump;
    [SerializeField] bool canRun;
    [SerializeField] bool canClimbWalls;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Handlers in Update() set state

        // Horizontal movement input handler
        horizontalMove = Input.GetAxisRaw("Horizontal");

        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jump = true;
            }
        }

        // Run handler
        if (canRun)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isRunning = true;
            }
            else
            {
                isRunning = false;
            }
        }

        // Jump and double jump handler
        if (isJumping)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                jumpCancel = true;
            }
        }

        if (canDoubleJump && isJumping && jumpCancel)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpCancel = false;
                doubleJump = true;
            }
        }

        // Climb handler
        if (canClimbWalls && isClimbing)
        {
            verticalMove = Input.GetAxisRaw("Vertical");
        }
    }

    private void FixedUpdate()
    {
        // Horizontal move execution
        if (horizontalMove!= 0)
        {
            if (isRunning)
            {
                _rb.AddForce(Vector3.right * horizontalMove * horizontalMoveSpeed * runSpeed);
            }
            else
            {
                _rb.AddForce(Vector3.right * horizontalMove * horizontalMoveSpeed);
            }
        }

        // Jump execution
        if (jump)
        {
            isGrounded = false;
            isJumping = true;
            jump = false;
            _rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
        else if ((jumpCancel || _rb.velocity.y < 0) && !isClimbing)
        {
            jump = false;
            _rb.AddForce(Vector3.down * jumpForce * gravityMultiplier, ForceMode2D.Force);
        }

        if (doubleJump && !isDoubleJumping)
        {
            doubleJump = false;
            isDoubleJumping = true;
            _rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }

        // Climb execution
        if (canClimbWalls && isClimbing)
        {
            _rb.transform.position = new Vector2(_rb.transform.position.x, _rb.transform.position.y + (verticalMove*climbSpeed*Time.deltaTime));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded= true;
            isJumping = false;
            isDoubleJumping = false;
            jumpCancel = false;
            doubleJump = false;
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            if (canClimbWalls)
            {
                _rb.gravityScale = 0f;
                isGrounded = true; // Consider removing this
                isClimbing = true;
                isJumping = false;
                isDoubleJumping = false;
                doubleJump = false;
                jumpCancel = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            _rb.gravityScale = 1f;
            isClimbing= false;
        }
    }

    public void ToggleCanDoubleJump()
    {
        canDoubleJump = !canDoubleJump;
    }

    public void ToggleCanRun() 
    { 
        canRun = !canRun;
    }

    public void ToggleCanClimb()
    {
        canClimbWalls = !canClimbWalls;
    }
}
