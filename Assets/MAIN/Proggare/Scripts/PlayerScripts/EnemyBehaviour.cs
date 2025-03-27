using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Elian

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField, Tooltip("Radius to spawn in prefabList")] float spawnRadius = 15f;
    [SerializeField, Tooltip("Makes so enemies doesn't spawn on player")] float nonSpawnRadius = 2f;
    [SerializeField, Tooltip("Radius for when prefabList see the player")] float sightRadius = 10f;
    [SerializeField] float despawnRadius = 25f;
    [SerializeField] int numberOfEnemies = 5;

    [SerializeField] GameObject enemyZero;
    [SerializeField] GameObject enemyOne;
    [SerializeField] GameObject enemyTwo;
    [SerializeField] GameObject enemyThree;
    [SerializeField] GameObject enemyFour;
    [SerializeField] Transform player;

    [Tooltip("Instansiate Gameobjects on transform for a clearer Hierarchy")] GameObject antiHierarchySpam;

    List<GameObject> prefabList = new List<GameObject>();
    List<GameObject> enemyPrefabs = new List<GameObject>();

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
        // Define a LayerMask that excludes the "Plank" layer
        int plankLayer = LayerMask.NameToLayer("Plank");
        int layerMask = ~(1 << plankLayer);

        foreach (GameObject enemy in prefabList)
        {
            if (enemy != null)
            {
                NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
                Animator animator = enemy.GetComponentInChildren<Animator>();

                if (agent != null && enemyScript != null)
                {
                    if (enemyScript.GetAgro())
                    {
                        // If agro is true, move towards the player
                        float distanceToPlayer = Vector3.Distance(player.position, enemy.transform.position);
                        if (distanceToPlayer <= sightRadius)
                        {
                            agent.SetDestination(player.position);
                        }
                        else
                        {
                            if (!agent.pathPending)
                            {
                                agent.ResetPath();
                                animator.SetTrigger("NoSight");
                            }
                        }
                    }
                    else
                    {
                        Vector3 directionToPlayer = (player.position - enemy.transform.position).normalized;

                        // Perform a raycast from the enemy to the player, ignoring the "Plank" layer
                        if (Physics.Raycast(enemy.transform.position, directionToPlayer, out RaycastHit hit, sightRadius, layerMask))
                        {
                            // Check if the raycast hit the player
                            if (hit.transform == player)
                            {
                                // Player is visible, set agro to true
                                enemyScript.SetAgro(true);

                                //Play enemy alert animation
                                animator.SetTrigger("Alert");

                                // Move towards the player
                                agent.SetDestination(player.position);

                                animator.SetTrigger("Walk");
                            }
                        }
                        else
                        {
                            // Player is out of sight radius, reset the path
                            if (!agent.pathPending)
                            {
                                agent.ResetPath();
                                animator.SetTrigger("NoSight");
                            }
                        }
                    }
                }
            }
        }
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
}