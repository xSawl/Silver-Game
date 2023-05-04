using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    private int amountOfJumpsLeft;

    private float movementInputDirection;
    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool canJump;

    public int amountOfJumps = 1;
   
    public float movementSpeed = 10f;
    public float jumpForce = 4f;
    public float groundCheckRadius;
    public float wallCheckDistance;

    public LayerMask whatIsGround;

    public Transform groundCheck;
    public Transform wallCheck;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
    }

    void Update()
    {
        checkInput();
        CheckMovementDirection();
        UpdateAnimation();
        CheckIfCanJump();
    }

    private void FixedUpdate() {
        applyMovement();
        CheckSuroundings();
    }

    private void applyMovement()
    {
        rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
    }

    private void checkInput() 
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if(Input.GetButtonDown("Jump")){
            Jump();
        }
    }

    private void CheckSuroundings()
    {
        //check if player is on ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        //check if player is on wall
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
        
    }


    private void CheckMovementDirection(){
        if(movementInputDirection < 0)
        {
            sprite.flipX = true;
        }
        else if(movementInputDirection > 0){
            sprite.flipX = false;
        }

        if (rb.velocity.x != 0)
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
        if (isGrounded && rb.velocity.y <= 0)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if (amountOfJumpsLeft <= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
    }

    private void Jump()
    {
        if(canJump){
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
        }
    }

    private void UpdateAnimation()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
    }

    private void OnDrawGizmos()
    {
        //Gizmos for on ground check
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        //Gizmos for on ground check
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}
