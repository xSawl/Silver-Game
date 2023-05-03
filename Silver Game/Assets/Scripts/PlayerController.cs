using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    private float movementInputDirection;
    private bool isFacingRight = true;

    public float movementSpeed = 10f;
    public float jumpForce = 4f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        checkInput();
        CheckMovementDirection();
    }

    private void FixedUpdate() {
        applyMovement();
    }

    private void checkInput() 
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if(Input.GetButtonDown("Jump")){
            Jump();
        }
    }

    private void applyMovement()
    {
        rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
    }

    private void CheckMovementDirection(){
        if(isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if(!isFacingRight && movementInputDirection > 0){
            Flip();
        }

    }

    private void Flip(){
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void Jump(){
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
}
