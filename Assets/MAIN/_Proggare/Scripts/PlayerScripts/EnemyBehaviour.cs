using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Elian

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] float hitRadius = 2; // Radius for the hitbox
    [SerializeField] float attackTime = 2f; // Time it takes to attack

    [Space]

    [SerializeField, Tooltip("Radius to spawn in prefabList RED")] float spawnRadius = 15f;
    [SerializeField, Tooltip("Makes so enemies doesn't spawn on player BLUE")] float nonSpawnRadius = 2f;
    [SerializeField, Tooltip("Radius for when prefabList see the player YELLOW")] float sightRadius = 10f;
    [SerializeField, Tooltip("GREEN")] float despawnRadius = 25f;
    [SerializeField] int numberOfEnemies = 5;

    [SerializeField] GameObject enemyZero;
    [SerializeField] GameObject enemyOne;
    [SerializeField] GameObject enemyTwo;
    [SerializeField] GameObject enemyThree;
    [SerializeField] GameObject enemyFour;
    [SerializeField] Transform player;

    [Tooltip("Instansiate Gameobjects on transform for a clearer Hierarchy")] GameObject antiHierarchySpam;

    [Header("Raycast")]
    [SerializeField] LayerMask agroIgnoreLayers; // Serialized LayerMask for raycast detection

    List<GameObject> prefabList = new List<GameObject>();
    List<GameObject> enemyPrefabs = new List<GameObject>();

    // Static list to track which zombies are aggroed
    private static List<GameObject> agroZombies = new List<GameObject>();

    // Static variable to track the previous count of aggroed zombies
    private static int previousAgroCount = 0;

    // Public static method to get the count of aggroed zombies
    public static int GetZombiesAgroCount()
    {
        // Recalculate the aggro count by cleaning up the list
        agroZombies.RemoveAll(zombie => zombie == null || !zombie.GetComponent<EnemyScript>().GetAgro());
        return agroZombies.Count;
    }

    // Public static method to get the list of aggroed zombies
    public static List<GameObject> GetAgroZombies()
    {
        // Clean up the list before returning it
        agroZombies.RemoveAll(zombie => zombie == null || !zombie.GetComponent<EnemyScript>().GetAgro());
        return agroZombies;
    }

    // Method to update the agroZombies list
    public static void UpdateZombiesAgro(GameObject zombie, bool isAgro)
    {
        // Add the zombie to the list if it is aggroed and not already in the list
        if (isAgro && !agroZombies.Contains(zombie))
        {
            agroZombies.Add(zombie);
        }

        // Recalculate the aggro count by cleaning up the list
        agroZombies.RemoveAll(z => z == null || !z.GetComponent<EnemyScript>().GetAgro());

        // Log the updated aggro count
        Debug.Log($"Updated aggro count: {agroZombies.Count}");
    }

    void Start()
    {
        if (enemyZero != null) enemyPrefabs.Add(enemyZero);
        if (enemyOne != null) enemyPrefabs.Add(enemyOne);
        if (enemyTwo != null) enemyPrefabs.Add(enemyTwo);
        if (enemyThree != null) enemyPrefabs.Add(enemyThree);
        if (enemyFour != null) enemyPrefabs.Add(enemyFour);

        if (enemyPrefabs.Count > 0)
        {
            SpawnEnemies(numberOfEnemies);
        }
    }

    void Update()
    {
        EnemyMovement();
        DespawnEnemy();
    }

    void SpawnEnemies(int amount)
    {
        //Doesnt work when reloading scene
        //if (antiHierarchySpam == null) { antiHierarchySpam = GameObject.FindGameObjectWithTag("antiHierarchySpam"); }

        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPos;
            bool validPosition = false;

            //Try to find a valid position within spawnRadius and outside nonSpawnRadius
            do
            {
                Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
                randomPos.y = 0;
                spawnPos = transform.position + randomPos;

                float distanceToSpawner = Vector3.Distance(transform.position, spawnPos);

                //Check if the position is within spawnRadius and outside nonSpawnRadius
                if (distanceToSpawner >= nonSpawnRadius && distanceToSpawner <= spawnRadius)
                {
                    validPosition = true;
                }
            } while (!validPosition);

            if (enemyPrefabs.Count > 0)
            {
                GameObject randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                GameObject enemy = Instantiate(randomEnemy, spawnPos, Quaternion.identity/*, antiHierarchySpam.transform*/);

                prefabList.Add(enemy);
                EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();

                if (enemyScript != null)
                {
                    enemyScript.OnEnemyDeath += HandleEnemyDeath;
                }
            }
        }
    }

    void HandleEnemyDeath(GameObject enemy)
    {
        if (prefabList.Contains(enemy))
        {
            prefabList.Remove(enemy);
            SpawnEnemies(1);
        }
    }

    void EnemyMovement()
    {
        foreach (GameObject enemy in prefabList)
        {
            if (enemy != null)
            {
                NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
                Animator animator = enemy.GetComponentInChildren<Animator>();

                if (agent != null && enemyScript != null)
                {
                    // Ensure the agent is on a NavMesh
                    if (!agent.isOnNavMesh)
                    {
                        Debug.LogWarning($"Enemy {enemy.name} is not on a NavMesh.");
                        continue;
                    }

                    //If the enemy has spotted the player
                    if (enemyScript.GetAgro() && !enemyScript.GetStunned())
                    {
                        animator.ResetTrigger("Idel");

                        //If agro is true, move towards the player
                        float distanceToPlayer = Vector3.Distance(player.position, enemy.transform.position);

                        //Check if Enemy is within hitRadius
                        if (distanceToPlayer <= hitRadius) { enemyScript.SetInAttackRange(true); }
                        else { enemyScript.SetInAttackRange(false); }

                        //Attack Logic
                        if (enemyScript.GetAttacking() == true)
                        {
                            return;
                        }

                        //In attack range
                        else if (enemyScript.GetInAttackRange())
                        {
                            //If the player is within hitRadius stop moving
                            agent.ResetPath();
                            animator.ResetTrigger("Walk");

                            //RANDOM ATTACK ANIMATION
                            int i = Random.Range(0, 2);
                            if (i == 0)
                            {
                                animator.SetTrigger("AttackLeft");
                            }
                            else
                            {
                                animator.SetTrigger("AttackRight");
                            }

                            //Attack!
                            enemyScript.SetAttacking(true);

                            //Start attacking routine aka. wait for animation to finish before dealing damage
                            StartCoroutine(AttackingRoutine(enemyScript, attackTime));
                        }
                        else if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Alert" && animator.GetCurrentAnimatorClipInfo(0)[0].clip.empty == false)
                        {
                            animator.SetTrigger("Walk");
                        }
                        else if (distanceToPlayer <= sightRadius && !enemyScript.GetInAttackRange() && !enemyScript.GetAttacking() && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AttackLeft" && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AttackRight")
                        {
                            agent.speed = enemyScript.GetOriginalSpeed();
                            agent.SetDestination(player.position);
                            animator.SetTrigger("Walk");
                        }
                    }
                    else if (enemyScript.GetAgro() && enemyScript.GetStunned())
                    {
                        //If the enemy is stunned, stop moving
                        animator.ResetTrigger("Walk");
                        animator.SetTrigger("Idel");
                        agent.ResetPath();
                    }
                    else
                    {
                        //Get direction to the player
                        Vector3 directionToPlayer = (player.position - enemy.transform.position).normalized;

                        //Perform a raycast from the enemy to the player, ignoring the layers specified in agroIgnoreLayers
                        if (Physics.Raycast(enemy.transform.position, directionToPlayer, out RaycastHit hit, sightRadius, ~agroIgnoreLayers))
                        {
                            //Check if the raycast hit the player
                            if (hit.transform == player)
                            {
                                //Play enemy alert animation
                                animator.SetTrigger("Alert");

                                agent.SetDestination(player.position);
                                agent.speed = 0f;

                                StartCoroutine(WaitForAlertAnimation(enemyScript, animator.GetCurrentAnimatorClipInfo(0)[0].clip.length));
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator AttackingRoutine(EnemyScript enemyScript, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (!enemyScript.GetStunned())
        {
            enemyScript.TryAttackPlayer();
            enemyScript.SetAttacking(false);
        }
    }
    IEnumerator WaitForAlertAnimation(EnemyScript enemyScript, float waitTime)
    {
        yield return new WaitForSeconds(waitTime/2);
        enemyScript.SetAgro(true);
    }

    void DespawnEnemy()
    {
        for (int i = prefabList.Count - 1; i >= 0; i--)
        {
            GameObject enemy = prefabList[i];
            if (enemy != null)
            {
                float distanceToSpawner = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToSpawner > despawnRadius)
                {
                    Destroy(enemy);
                    prefabList.RemoveAt(i);
                    SpawnEnemies(1);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, nonSpawnRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, despawnRadius);

        // Draw raycasts for each enemy
        Gizmos.color = Color.magenta;
        foreach (GameObject enemy in prefabList)
        {
            if (enemy != null)
            {
                Vector3 directionToPlayer = (player.position - enemy.transform.position).normalized;
                if (Physics.Raycast(enemy.transform.position, directionToPlayer, out RaycastHit hit, sightRadius))
                {
                    // Draw a line from the enemy to the hit point
                    Gizmos.DrawLine(enemy.transform.position, hit.point);

                    // If the raycast hit the player, draw a sphere at the hit point
                    if (hit.transform == player)
                    {
                        Gizmos.DrawSphere(hit.point, 0.2f);
                    }
                }
            }
        }
    }
    void ApplyKnockback(GameObject enemy, Vector3 direction, float force)
    {
        if (enemy == null) return;

        // Get references to the enemy's components
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        Rigidbody enemyRigidbody = enemy.GetComponent<Rigidbody>();
        EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();

        if (enemyScript == null || enemyRigidbody == null || agent == null) return;

        // Temporarily disable NavMeshAgent to allow physics-based knockback
        agent.enabled = false;

        // Apply knockback force
        enemyRigidbody.AddForce(direction.normalized * force, ForceMode.Impulse);

        // Re-enable NavMeshAgent after a delay
        StartCoroutine(ReenableNavMeshAgent(agent));
    }

    IEnumerator ReenableNavMeshAgent(NavMeshAgent agent)
    {
        yield return new WaitForSeconds(0.5f); // Adjust delay as needed
        agent.enabled = true;
    }
}
