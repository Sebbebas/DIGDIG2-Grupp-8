using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

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

    NavMeshAgent agent;
    Rigidbody myRigidbody;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myRigidbody = GetComponent<Rigidbody>();
        enemySpeedAtStart = agent.speed;
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

    private void OnCollisionEnter(Collision collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

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
        currentHealth -= damageAmount;
        Debug.Log("Enemy took damage: " + damageAmount + ", Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnEnemyDeath?.Invoke(gameObject);
        Destroy(gameObject);
    }
}