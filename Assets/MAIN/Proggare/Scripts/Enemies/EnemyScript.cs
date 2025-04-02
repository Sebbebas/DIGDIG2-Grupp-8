using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;
using System.Collections.Generic;

//Alexander

[System.Serializable]
public class BodyPart
{
    public string name;
    public float maxHealth;
    public float currentHealth;

    public BodyPart(string name, float maxHealth)
    {
        this.name = name;
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
    }

    public bool IsDestroyed()
    {
        return currentHealth <= 0;
    }
}

public class EnemyScript : MonoBehaviour
{
    public event Action<GameObject> OnEnemyDeath;
    public List<BodyPart> bodyParts = new List<BodyPart>();

    [Header("Player Affecting Values")]
    [SerializeField] float DamageAmount = 20f;
    [SerializeField] float stunTime = 1f;
    [SerializeField] float maxKnockbackVelocity = 5;
    [SerializeField] float damageCooldown = 2f;

    [SerializeField] float headHealth = 50f;
    [SerializeField] float torsoHealth = 100f;
    [SerializeField] float armHealth = 30f;
    [SerializeField] float currentHealth = 100f;

    [SerializeField] bool agro = false;

    private Vector3 kickDirection;
    private float enemySpeedAtStart;
    private bool isStunned;
    private float currentStunTime;
    private bool canDamage = true;
    private bool isDead = false;

    [Header("Body Parts")]
    [SerializeField] GameObject leftArm;
    [SerializeField] GameObject lostLeftArm;
    [SerializeField] GameObject rightArm;
    [SerializeField] GameObject lostRightArm;
    [SerializeField] ParticleSystem limbLossEffect;

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

    [Header("Texture")]
    [SerializeField] Material[] textures;

    private Transform player;
    private bool canShoot = true;

    private void Awake()
    {
        // Initialize body parts
        bodyParts.Add(new BodyPart("Head", headHealth));
        bodyParts.Add(new BodyPart("Torso", torsoHealth));
        bodyParts.Add(new BodyPart("Left Arm", armHealth));
        bodyParts.Add(new BodyPart("Right Arm", armHealth));
        //bodyParts.Add(new BodyPart("Left Leg", 40));
        //bodyParts.Add(new BodyPart("Right Leg", 40));
    }

    private void Start()
    {
        RandomEnemyTexture();

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

    private void RandomEnemyTexture()
    {
        if (textures.Length > 0)
        {
            int randomTexture = UnityEngine.Random.Range(0, textures.Length);
            
            SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material = textures[randomTexture];
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

    public void TakeDamage(string bodyPartName, float damage)
    {
        BodyPart part = bodyParts.Find(p => p.name == bodyPartName);
        if (part != null)
        {
            part.TakeDamage(damage);
            Debug.Log($"{part.name} took {damage} damage. Remaining health: {part.currentHealth}");

            if (part.IsDestroyed())
            {
                Debug.Log($"{part.name} is destroyed!");
                HandleLimbDestruction(part);
            }
        }
    }

    void HandleLimbDestruction(BodyPart part)
    {
        if (part.name == "Head")
        {
            Debug.Log("Zombie death");
            //Head explode, body falls apart
            Die();
        }
        else if (part.name == "Torso")
        {
            Debug.Log("Zombie death");
            //Whole zombie explode
            Die();
        }
        else if (part.name == "Left Arm")
        {
            Debug.Log("Left arm hit");
            if (armHealth <= 0)
            {
                leftArm.SetActive(false);
                lostLeftArm.SetActive(true);
                Debug.Log("Left arm dead");
            }
        }
        else if (part.name == "Right Arm")
        {
            rightArm.SetActive(false);
            lostRightArm.SetActive(true);
            Debug.Log("Right arm hit");
        }

        //else if (part.name == "Arms")
        //{
        //    Debug.Log("Arm hit");
        //    if(part.name == "Left Arm")
        //    {
        //        leftArm.SetActive(false);
        //        Debug.Log("Left arm hit");
        //    }
        //    if(part.name == "Right Arm")
        //    {
        //        rightArm.SetActive(false);
        //        Debug.Log("Right arm hit");
        //    }
        //}
    }

    public void ApplyDamageHead(float damage)
    {
        headHealth -= damage;
        Debug.Log(transform.gameObject.name + " took damage: " + damage + ", Current Health: " + headHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Headshot");
            //Die();
        }
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

    public bool GetAgro()
    {
        return agro;
    }
    public bool SetAgro(bool value)
    {
        return agro = value;
    }
}