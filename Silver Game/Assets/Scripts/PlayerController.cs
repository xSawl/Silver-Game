using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    private float movementInputDirection;

   
    public float movementSpeed = 10f;
    public float jumpForce = 4f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
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
        if(movementInputDirection < 0)
        {
            sprite.flipX = true;
        }
        else if(movementInputDirection > 0){
            sprite.flipX = false;
        }

    }

    private void Jump(){
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
}
