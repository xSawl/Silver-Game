using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private float maxHeath;

    [SerializeField]
    private GameObject
        deathChunkParticle,
        deathBloodParticle;

    private float currentHealth;

    private GameManager GM;

    private void Start()
    {
        currentHealth = maxHeath;
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Instantiate(deathBloodParticle, transform.position, deathBloodParticle.transform.rotation);
        Instantiate(deathChunkParticle, transform.position, deathChunkParticle.transform.rotation);
        GM.Respawn();
        Destroy(gameObject);
    }
}
