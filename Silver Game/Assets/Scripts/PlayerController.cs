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
    private float turnTimer;
    private float wallJumpTimer;
    private int lastWallJumpDirection;
    private float dashTimeLeft;
    private float lastImageExpos;
    private float lastDash = -100;

    private float movementInputDirection;
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canNormalJump;
    private bool canWallJump;
    private bool isAttemptingToJump;
    private bool checkJumpMiltiplier;
    private bool canMove;
    private bool canFlip;
    private bool hasWallJumped;
    private bool isTouchingLedge;
    private bool canClimbLedge = false;
    private bool ledgeDetected;
    private bool isDashing;

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
    public float turnTimerSet = 0.1f;
    public float wallJumpTimerSet = 0.5f;
    public float dashTime;
    public float dashSpeed;
    public float distanceBetweenImages;
    public float dashCoolDown;

    public float ledgeClimbXOffSet1 = 0f;
    public float ledgeClimbYOffSet1 = 0f;
    public float ledgeClimbXOffSet2 = 0f;
    public float ledgeClimbYOffSet2 = 0f;


    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    private Vector2 ledgePosBot;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;

    public LayerMask whatIsGround;

    public Transform groundCheck;
    public Transform wallCheck;
    public Transform ledgeCheck;

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
        CheckWallClimb();
        //CheckDash();
    }

    private void FixedUpdate() {
        applyMovement();
        CheckSuroundings();
        CheckDash();
    }

    private void applyMovement()
    {

        //don't move in Air
        if (!isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }

        //on ground
        else if(canMove)
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

        if(Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if(!isGrounded && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;

                turnTimer = turnTimerSet;
            }
        }

        if(checkJumpMiltiplier && !Input.GetButton("Jump"))
        {
            checkJumpMiltiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }

        if(turnTimer >= 0)
        {
            turnTimer -= Time.deltaTime;

            if(turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if(Input.GetButtonDown("Dash"))
        {
            Debug.Log("tu a appuyer sur Dash");
            if(Time.time >= (lastDash + dashCoolDown)) 
            {
                AttemptToDash();
            }
        }
    }

    private void CheckSuroundings()
    {
        //check if player is on ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        //check if player is on wall
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);

        //check if player is on ledge
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, whatIsGround);

        if(isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePosBot = wallCheck.position;
        }

    }

    private void CheckDash(){
        if(isDashing)
        {
            if(dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection, 0);
                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageExpos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageExpos = transform.position.x;
                }
            }

            if(dashTimeLeft <= 0 || isTouchingWall) 
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
            }
           
        }
    }

    private void CheckIfWallSliding()
    {
        if(isTouchingWall && movementInputDirection == facingDirection && rb.velocity.y < 0 && !canClimbLedge)
        {
            isWallSliding = true;
        }
        else{
            isWallSliding = false;
        }
    }

    private void CheckWallClimb()
    {
        if(ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;

            if(isFacingRight){
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffSet1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffSet1);

                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffSet2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffSet2);
            }
            else
            {
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffSet1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffSet1);

                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffSet2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffSet2);
            }

            canMove = false;
            canFlip = false;

            anim.SetBool("canClimbLedge", canClimbLedge);
        }

        if(canClimbLedge)
        {
            transform.position = ledgePos1;
        }
    }

    private void AttemptToDash() 
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
        
        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageExpos = transform.position.x;
    }
    
    public void FinishLedgeClimb()
    {
        canClimbLedge = false;
        transform.position = ledgePos2;
        canMove = true;
        canFlip = true;
        ledgeDetected = false;
        anim.SetBool("canClimbLedge", canClimbLedge);
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

            if(wallJumpTimer > 0)
            {
                if(hasWallJumped && movementInputDirection == -lastWallJumpDirection)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                    hasWallJumped = false;
                }
                else if(wallJumpTimer <= 0)
                {
                    hasWallJumped = false;
                }
                else
                {
                    wallJumpTimer -= Time.deltaTime;
                }
            }
        }
    }


    private void Flip()
    {
        if(!isWallSliding && canFlip)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
       
    }

    public void DisableFlip(){
        canFlip = false;
    }

    public void EnableFlip(){
        canFlip = true;
    }


    private void NormalJump()
    {
        if (canNormalJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMiltiplier = true;
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
            checkJumpMiltiplier = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;
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
