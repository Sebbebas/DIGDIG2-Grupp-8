using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

//Alexander

public class EnemyScript : MonoBehaviour
{
    public event Action<GameObject> OnEnemyDeath;

    [Header("Player affecting values")]
    [SerializeField] float DamageAmount = 20f;
    [SerializeField] float stunTime = 1f;
    [SerializeField] float maxKnockbackVelocity = 5;
    [SerializeField] float damageCooldown = 2f;
    [SerializeField] float currentHealth = 100f;

    private Vector3 kickDirection;
    private float enemySpeedAtStart;
    private bool isStunned;
    private float currentStunTime;
    private bool canDamage = true;
    private bool isDead = false;

    [Header("LootDrop values")]
    private LootSystem lootSystem;

    [Header("Scoring System")]
    [SerializeField] int scoreValue = 10; //Points per enemy killed

    NavMeshAgent agent;
    Rigidbody myRigidbody;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myRigidbody = GetComponent<Rigidbody>();
        enemySpeedAtStart = agent.speed;

        lootSystem = GetComponent<LootSystem>();
    }

    private void Update()
    {
        if (isStunned)
        {
            myRigidbody.AddForce(kickDirection, ForceMode.Force);

            if (currentStunTime > 0)
            {
                currentStunTime -= Time.deltaTime;
            }
            else
            {
                currentStunTime = 0;
                isStunned = false;
                agent.isStopped = false;
                agent.speed = enemySpeedAtStart;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null && canDamage)
        {
            playerHealth.ApplyDamage(DamageAmount);
            Debug.Log("Enemy collided with player, damage applied: " + DamageAmount);
            StartCoroutine(DamageCooldown());
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }

    public void Kicked(Vector3 direction)
    {
        isStunned = true;
        agent.isStopped = true;
        currentStunTime = stunTime;
        kickDirection = direction;
        agent.speed = 0;
    }

    public void ApplyDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log(transform.gameObject.name + " took damage: " + damageAmount + ", Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; //Ensures Die() is only called once
        isDead = true;

        OnEnemyDeath?.Invoke(gameObject);
        ScoreManager.Instance.AddScore(scoreValue);

        if (lootSystem != null)
        {
            lootSystem.DropLoot();
        }

        Destroy(gameObject);
    }
}