using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

//Alexander

public class EnemyScript : MonoBehaviour
{
    public event Action<GameObject> OnEnemyDeath;

    [Header("Player Affecting Values")]
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

    [Header("Loot Drop Values")]
    private LootSystem lootSystem;

    [Header("Scoring System")]
    [SerializeField] int scoreValue = 10; //Points per enemy killed

    NavMeshAgent agent;
    Rigidbody myRigidbody;

    [Header("Projectile Enemy Settings")]
    [SerializeField] private bool isProjectileEnemy = false;
    [SerializeField] private float attackDistance = 10f; //Distance where enemy stops & shoots
    [SerializeField] private float minSafeDistance = 4f; //Distance where enemy moves backwards
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootCooldown = 2f;

    private Transform player;
    private bool canShoot = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myRigidbody = GetComponent<Rigidbody>();
        enemySpeedAtStart = agent.speed;
        lootSystem = GetComponent<LootSystem>();

        //Find the player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (isProjectileEnemy && player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= minSafeDistance)
            {
                MoveAwayFromPlayer();
            }
            else if (distanceToPlayer <= attackDistance)
            {
                agent.isStopped = true;
                if (canShoot)
                {
                    StartCoroutine(ShootProjectile());
                }
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }

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

    private void MoveAwayFromPlayer()
    {
        Vector3 directionAway = (transform.position - player.position).normalized;
        Vector3 newPos = transform.position + directionAway * 2f; //Moves 2 units backward
        agent.isStopped = false;
        agent.SetDestination(newPos);
    }

    private IEnumerator ShootProjectile()
    {
        canShoot = false;

        if (projectilePrefab != null && shootPoint != null)
        {
            Instantiate(projectilePrefab, shootPoint.position, Quaternion.LookRotation(player.position - shootPoint.position));
        }

        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
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
        if (isDead) return;
        isDead = true;

        OnEnemyDeath?.Invoke(gameObject);
        ScoreManager.Instance.AddScore(scoreValue);

        // **Lägger till combo-systemet**
        if (ComboManager.instance != null)
        {
            ComboManager.instance.AddKill();
        }

        if (lootSystem != null)
        {
            lootSystem.DropLoot();
        }

        Destroy(gameObject);
    }

    //Gizmo retreat distance
    private void OnDrawGizmosSelected()
    {
        if (isProjectileEnemy)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDistance);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, minSafeDistance);
        }
    }
}