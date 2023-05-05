using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    private int amountOfJumpsLeft;
    private int facingDirection = 1;
    private float jumpTimer;

    private float movementInputDirection;
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canNormalJump;
    private bool canWallJump;
    private bool isAttemptingToJump;

    public int amountOfJumps = 1;
    public float movementSpeed = 10f;
    public float jumpForce = 4f;
    public float  variableJumpHeightMultiplier = 0.5f;
    public float wallSlideSpeed;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float movementForceInAir;
    public float airDragMultiplier = 0.95f;
    public float wallHopForce;
    public float wallJumpForce;
    public float jumpTimerSet = 0.15f;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public LayerMask whatIsGround;

    public Transform groundCheck;
    public Transform wallCheck;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    void Update()
    {
        checkInput();
        CheckMovementDirection();
        UpdateAnimation();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckJump();
    }

    private void FixedUpdate() {
        applyMovement();
        CheckSuroundings();
    }

    private void applyMovement()
    {

        //don't move in Air
        if (!isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }

        //on ground
        else
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y); 
        }

        //on a wall
        if(isWallSliding)
        {
            if(rb.velocity.y < wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void checkInput() 
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if(Input.GetButtonDown("Jump"))
        {
            if(isGrounded || (amountOfJumpsLeft > 0 && isTouchingWall))
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }

        }

        if(Input.GetButtonUp("Jump")){
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }
    }

    private void CheckSuroundings()
    {
        //check if player is on ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        //check if player is on wall
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
        
    }

    private void CheckIfWallSliding()
    {
        if(isTouchingWall && movementInputDirection == facingDirection)
        {
            isWallSliding = true;
        }
        else{
            isWallSliding = false;
        }
    }


    private void CheckMovementDirection(){
        if(isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if(!isFacingRight && movementInputDirection > 0){
            Flip();
        }

        if (movementInputDirection != 0)
        {
            isWalking = true;
        }

        else
        {
            isWalking = false;
        }

    }

    private void CheckIfCanJump()
    {
        //reinitialize amount of jump left if we are grounded
        if (isGrounded && rb.velocity.y <= 0.01f)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if(isTouchingWall)
        {
            canWallJump = true;
        }

        if (amountOfJumpsLeft <= 0)
        {
            canNormalJump = false;
        }
        else
        {
            canNormalJump = true;
        }
    }

    private void CheckJump()
    {
        if(jumpTimer > 0) 
        {
            //WallJump
            if(!isGrounded && isTouchingWall && movementInputDirection !=0 && movementInputDirection != facingDirection) 
            {
                WallJump();
            }
            else if (isGrounded)
            {
                NormalJump();
            }
            
            if(isAttemptingToJump)
            {
                jumpTimer -= Time.deltaTime;
            }
        }
    }


    private void Flip()
    {
        if(!isWallSliding)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
       
    }

    private void NormalJump()
    {
        if (canNormalJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
            jumpTimer = 0;
            isAttemptingToJump = false;
        }
    }

    private void WallJump(){
        if (canWallJump)
        {
            rb.velocity = new Vector2 (rb.velocity.x, 0.0f);
            isWallSliding = false;
            amountOfJumpsLeft = amountOfJumps;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            jumpTimer = 0;
            isAttemptingToJump = false;
        }
    }

    /*
    private void HopJump()
    {
        if (isWallSliding && movementInputDirection == 0 && canJump) //for Wall Hop
        {
            isWallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);

        }
    }
    */

    private void UpdateAnimation()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
    }

    private void OnDrawGizmos()
    {
        //Gizmos for on ground check
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        //Gizmos for on ground check
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}
