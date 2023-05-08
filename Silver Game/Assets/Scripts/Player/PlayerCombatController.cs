using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField]
    private bool combatEnable;
    [SerializeField]
    private float inputTimer, attack1Radius, attack1Damage;
    [SerializeField]
    private Transform attackHitBoxPos;
    [SerializeField]
    private LayerMask whatISDamageable;

    private bool gotInput, isAttacking, isFirstAttack;

    private float lastInputTime = Mathf.NegativeInfinity;

    private float[] attackDetails = new float [2];

    private Animator anim;

    private PlayerController PC;
    private PlayerStats PS;

    private void Start() 
    {
        anim = GetComponent<Animator>();
        anim.SetBool("canAttack", combatEnable);
        PC = GetComponent<PlayerController>();
        PS = GetComponent<PlayerStats>();   
    }

    private void Update() 
    {
        CheckCombatInput();
        CheckAttacks();
    }


    private void CheckCombatInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(combatEnable){
                gotInput = true;
                lastInputTime = Time.time;
            }
        }
    }

    private void CheckAttacks(){
        if(gotInput) 
        {
            //perform Attack1
            if(!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;
                anim.SetBool("attack1", true);
                anim.SetBool("firstAttack", isFirstAttack);
                anim.SetBool("isAttacking", isAttacking);

            }

            if(Time.time >= (lastInputTime + inputTimer))
            {
                //wait for new input
                gotInput = false;
            }
        }
    }

    private void CheckAttackHitBox()
    {
        Collider2D[] detectedObjects =    
        Physics2D.OverlapCircleAll(attackHitBoxPos.position, attack1Radius, 
        whatISDamageable);

        attackDetails[0] = attack1Damage;
        attackDetails[1] = transform.position.x;

        foreach (Collider2D collider in detectedObjects) 
        {
            collider.transform.parent.SendMessage("Damage", attackDetails);

            //instantiate hit particle
        }
    }

    private void FinishAttack1() 
    {
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("attack1", false);
        
    }

    private void Damage(float[] attackDetails)
    {

        if(!PC.getDashStatus()) 
        {
            int directiion;

            PS.DecreaseHealth(attackDetails[0]);

            if(attackDetails[1] < transform.position.x)
            {
                directiion = 1;
            }

            else
            {
                directiion = -1;
            }

            PC.Knockback(directiion);
        }
        
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawWireSphere(attackHitBoxPos.position, attack1Radius);
    }
}
