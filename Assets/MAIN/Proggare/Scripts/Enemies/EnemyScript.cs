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

    [Header("Enemy Health Values")] // HEALTH FOR BODY PARTS
    [SerializeField] float headHealth = 50f;
    [SerializeField] float torsoHealth = 100f;
    [SerializeField] float leftArmHealth = 30f;
    [SerializeField] float rightArmHealth = 30f;
    [SerializeField] float currentHealth = 100f;

    [Header("Enemy Behaviour")] // ANIMATIONS
    [SerializeField] bool agro = false;
    [SerializeField] bool attacking = false;
    [SerializeField] bool inAttackRange = false;
    [SerializeField] bool alertAnimationStarted = false;

    [Header("Body Parts")]
    [SerializeField] GameObject leftArm;
    [SerializeField] GameObject lostLeftArm;
    [SerializeField] GameObject rightArm;
    [SerializeField] GameObject lostRightArm;
    [SerializeField] ParticleSystem limbLossEffect;


    [Header("Scoring System")]
    [SerializeField] int scoreValue = 10; //Points per enemy killed

    [Header("Projectile Enemy Settings")]
    [SerializeField] private bool isProjectileEnemy = false;
    [SerializeField] private float attackDistance = 10f; //Distance where enemy stops & shoots
    [SerializeField] private float minSafeDistance = 4f; //Distance where enemy moves backwards
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootCooldown = 2f;

    [Header("Texture")]
    [SerializeField] Material[] textures;

    //Private?????
    [Header("Loot Drop Values")]
    private LootSystem lootSystem;
    
    //Private variables
    private Vector3 kickDirection;
    private float enemySpeedAtStart;
    private bool isStunned;
    private float currentStunTime;
    private bool isDead = false;
    private Transform player;
    private bool canShoot = true;
    private float originalSpeed;

    //Chaced References
    NavMeshAgent agent;
    Rigidbody myRigidbody;

    private void Awake()
    {
        // Initialize body parts
        bodyParts.Add(new BodyPart("Head", headHealth));
        bodyParts.Add(new BodyPart("Torso", torsoHealth));
        bodyParts.Add(new BodyPart("Left Arm", leftArmHealth));
        bodyParts.Add(new BodyPart("Right Arm", rightArmHealth));
        //bodyParts.Add(new BodyPart("Left Leg", 40));
        //bodyParts.Add(new BodyPart("Right Leg", 40));
    }

    private void Start()
    {
        RandomEnemyTexture();

        agent = GetComponent<NavMeshAgent>();
        myRigidbody = GetComponent<Rigidbody>();
        lootSystem = GetComponent<LootSystem>();
        enemySpeedAtStart = agent.speed;

        //Find the player
        originalSpeed = agent.speed;
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

    public void TryAttackPlayer()
    {
        if(!inAttackRange) { Debug.Log("cant reach player"); return; }
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        playerHealth.ApplyDamage(DamageAmount);
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
            if (leftArmHealth <= 0)
            {
                leftArm.SetActive(false);
                lostLeftArm.SetActive(true);
                Debug.Log("Left arm dead");
            }
        }
        else if (part.name == "Right Arm")
        {
            if (rightArmHealth <= 0)
            {
                rightArm.SetActive(false);
                lostRightArm.SetActive(true);
                Debug.Log("Right arm hit");
            }
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

    public void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        //Debug.Log(transform.gameObject.name + " took damage: " + damage + ", Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Die");
            Die();
        }
    }

    public void ApplyDamageHead(float damage)
    {
        headHealth -= damage;
        Debug.Log(transform.gameObject.name + " took damage: " + damage + ", Current Head Health: " + headHealth);

        if (headHealth <= 0)
        {
            Debug.Log("Head dead");
            //Die();
        }
    }

    public void ApplyTorsoDamage(float damage)
    {
        if (isDead) return;

        torsoHealth -= damage;
        Debug.Log(transform.gameObject.name + " took damage: " + damage + ", Current Torso Health: " + torsoHealth);

        if (torsoHealth <= 0)
        {
            Debug.Log("Torso dead");
            //Die();
        }
    }

    public void ApplyLeftArmDamage(float damage)
    {
        leftArmHealth -= damage;
        Debug.Log(transform.gameObject.name + " took damage: " + damage + ", Current Left Health: " + leftArmHealth);

        if (leftArmHealth <= 0)
        {
            Debug.Log("Left Arm Dead");
        }
    }

    public void ApplyRightArmDamage(float damage)
    {
        rightArmHealth -= damage;
        Debug.Log(transform.gameObject.name + " took damage: " + damage + ", Current Right Health: " + rightArmHealth);

        if (rightArmHealth <= 0)
        {
            Debug.Log("Right Arm Dead");
        }
    }

    public void Die()
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

        GetComponent<ZombieBodyParts>().SpawnGibbs();
        Debug.Log("Destroy enemy");
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

    #region Get Set
    //Attack Agro
    public bool GetAgro()
    {
        return agro;
    }
    public bool SetAgro(bool value)
    {
        return agro = value;
    }

    //Attack Bool
    public bool GetAttacking()
    {
        return attacking;
    }
    public bool SetAttacking(bool value)
    {
        return attacking = value;
    }

    //In Attack Range Bool
    public bool GetInAttackRange()
    {
        return inAttackRange;
    }
    public bool SetInAttackRange(bool value)
    {
        return inAttackRange = value;
    }

    //Alert Animation Started
    public bool GetAlertAnimationStarted()
    {
        return alertAnimationStarted;
    }
    public bool SetAlertAnimationStarted(bool value)
    {
        return alertAnimationStarted = value;
    }

    //Original Speed
    public float GetOriginalSpeed()
    {
        return originalSpeed;
    }
    #endregion
}