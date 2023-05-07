using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    private enum State {
        Moving,
        Knockback,
        Dead
    }

    private State currentState;

    [SerializeField]
    private Transform
        groundCheck,
        wallCheck,
        touchDamageCheck;

    [SerializeField]
    private float
        groundCheckDistance,
        wallCheckDistance,
        movementSpeed,
        maxHealth,
        KnockbackDuration,
        lastTouchDamagaTime,
        touchDamageCooldpw,
        touchDamage,
        touchDamageWidth,
        touchDamageHeight;

    [SerializeField]
    private LayerMask 
        whatIsGround,
        whatIsPlayer;

    [SerializeField]
    private Vector2 knockbackSpeed;

    [SerializeField]
    private GameObject
        hitParticle,
        deathChunkParticle,
        deathBloodParticle;

    private bool
        groundDetected,
        wallDetected;

    private int 
        facingDirection,
        damageDirection;

    private float
        currentHealth,
        knockbackStartTime;

    private float[] attackDetails = new float[2];

    private Vector2
        movement,
        touchDamageBotLeft,
        touchDamageTopRight;

    private GameObject alive;
    private Rigidbody2D aliveRB;
    private Animator aliveAnim;


    #region Main Function

    private void Start() 
    {
        alive = transform.Find("Alive").gameObject;
        aliveRB = alive.GetComponent<Rigidbody2D>();
        aliveAnim = alive.GetComponent<Animator>();

        currentHealth = maxHealth;
        facingDirection = 1;
    }

    private void Update() 
    {
        switch(currentState)
        {
            case State.Moving:
            UpdateMovingState();
            break;

            case State.Knockback:
            UpdateKnockbackState();
            break;

            case State.Dead:
            UpdateDeadState();
            break;

        }
    }

    #endregion

    //-----------Moving state-----------------
    #region Moving state
    
    private void EnterMovingState(){

    }

    private void UpdateMovingState(){
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down,   
        groundCheckDistance,      
        whatIsGround);

        wallDetected = Physics2D.Raycast(wallCheck.position, transform.right,   
        wallCheckDistance,      
        whatIsGround);

        CheckTouchDamage();

        if(!groundDetected || wallDetected )
        {
            Flip();
        }
        else
        {
            movement.Set(movementSpeed * facingDirection, aliveRB.velocity.y);
            aliveRB.velocity = movement;
        }
    }

    private void ExittMovingState(){

    }

    #endregion

    //---------Knockback state--------------
    #region Knockback state
    
    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        aliveRB.velocity = movement;
        aliveAnim.SetBool("knockback", true);
    }

    private void UpdateKnockbackState()
    {
        if(Time.time >= (knockbackStartTime + KnockbackDuration))
        {
            SwitchState(State.Moving);
        }
    }

    private void ExittKnockbackState()
    {
        aliveAnim.SetBool("knockback", false);
    }

    #endregion

    //---------Daeth state--------------
    #region Daeth state
    private void EnterDeadState(){
        Instantiate(deathChunkParticle, alive.transform.position, 
        deathChunkParticle.transform.rotation);

        Instantiate(deathBloodParticle, alive.transform.position, 
        deathBloodParticle.transform.rotation);

        Destroy(gameObject);
    }

    private void UpdateDeadState(){

    }

    private void ExittDeadState(){

    }

    #endregion

    //-----------Other Functions------------------

    private void Damage(float[] attackDetails)
    {
        currentHealth -= attackDetails[0];

        Instantiate(hitParticle, alive.transform.position, Quaternion.Euler(0.0f, 0.0f, 
        Random.Range(0.0f, 360.0f)));

        if(attackDetails[1] > alive.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        //Hit Particles

        if(currentHealth > 0.0f) 
        {
            SwitchState(State.Knockback);
        }
        else if(currentHealth <= 0.0f) 
        {
            SwitchState(State.Dead);
        }
    }

    private void CheckTouchDamage()
    {
        if(Time.time >= lastTouchDamagaTime + touchDamageCooldpw) 
        {
            touchDamageBotLeft.Set(touchDamageCheck.position.x -        
            (touchDamageWidth / 2), touchDamageCheck.position.y -   
            (touchDamageHeight / 2));

            touchDamageTopRight.Set(touchDamageCheck.position.x +        
            (touchDamageWidth / 2), touchDamageCheck.position.y +   
            (touchDamageHeight / 2));

            Collider2D hit = Physics2D.OverlapArea(touchDamageBotLeft,  
            touchDamageTopRight, whatIsPlayer);

            if(hit != null) 
            {
                lastTouchDamagaTime = Time.time;
                attackDetails[0] = touchDamage;
                attackDetails[1] = alive.transform.position.x;
                hit.SendMessage("Damage", attackDetails); 
            }
        }
    }


    private void Flip()
    {
        facingDirection *= -1;
        alive.transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void SwitchState(State state)
    {
        switch(currentState)
        {
            case State.Moving:
            ExittMovingState();
            break;

            case State.Knockback:
            ExittKnockbackState();
            break;

            case State.Dead:
            ExittDeadState();
            break;

        }

         switch(state)
        {
            case State.Moving:
            EnterMovingState();
            break;

            case State.Knockback:
            EnterKnockbackState();
            break;

            case State.Dead:
            EnterDeadState();
            break;
        }

        currentState = state;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, 
        groundCheck.position.y - groundCheckDistance));

        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, 
        wallCheck.position.y));

        Vector2 bottomLeft = new Vector2(touchDamageCheck.position.x -        
            (touchDamageWidth / 2), touchDamageCheck.position.y -   
            (touchDamageHeight / 2));

        Vector2 bottomRight = new Vector2(touchDamageCheck.position.x +        
            (touchDamageWidth / 2), touchDamageCheck.position.y -   
            (touchDamageHeight / 2));

        Vector2 topLeft = new Vector2(touchDamageCheck.position.x -        
            (touchDamageWidth / 2), touchDamageCheck.position.y +   
            (touchDamageHeight / 2));

        Vector2 topRight = new Vector2(touchDamageCheck.position.x +        
            (touchDamageWidth / 2), touchDamageCheck.position.y +   
            (touchDamageHeight / 2));

            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);

    }
    
}
