using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewEnemySpawn : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;
    [SerializeField] Transform player;

    [SerializeField] int minSpawnCount = 2;
    [SerializeField] int maxSpawnCount = 5;
    [SerializeField] float spawnRadius = 5f;
    [SerializeField] float sightRadius = 35f;
    [SerializeField, Tooltip("Radius for the hitbox")] float hitRadius = 2; 
    [SerializeField, Tooltip("Time it takes to attack")] float attackTime = 2f; 

    [SerializeField] Vector3 spawnPos;

    [SerializeField] LayerMask agroIgnoreLayers;

    List<GameObject> zombieList = new List<GameObject>();

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

    private void Start()
    {
        SpawnEnemies();
    }

    private void Update()
    {
        EnemyMovement();
    }

    void SpawnEnemies()
    {
        if (enemies == null || enemies.Length == 0) { return; }

        int enemyCount = Random.Range(minSpawnCount, maxSpawnCount + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 randomPosition = GetRandomPos();
            GameObject randomEnemy = GetRandomEnemyPrefab();
            GameObject newEnemy = Instantiate(randomEnemy, randomPosition, Quaternion.identity);

            zombieList.Add(newEnemy);
        }
    }

    void EnemyMovement()
    {
        foreach (GameObject enemy in zombieList)
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

    Vector3 GetRandomPos()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = new Vector3(randomCircle.x, 0, randomCircle.y);
        spawnPos += transform.position;
        return spawnPos;
    }

    GameObject GetRandomEnemyPrefab()
    {
        int index = Random.Range(0, enemies.Length);
        return enemies[index];

    }

    IEnumerator AttackingRoutine(EnemyScript enemyScript, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        enemyScript.TryAttackPlayer();
        enemyScript.SetAttacking(false);
    }
    IEnumerator WaitForAlertAnimation(EnemyScript enemyScript, float waitTime)
    {
        //Debug.Log(waitTime + " " + waitTime /2);
        yield return new WaitForSeconds(waitTime / 2);
        enemyScript.SetAgro(true);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        // Draw raycasts for each enemy
        Gizmos.color = Color.magenta;
        foreach (GameObject enemy in zombieList)
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
}