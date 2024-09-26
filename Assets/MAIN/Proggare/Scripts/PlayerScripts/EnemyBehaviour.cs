using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Elian

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField, Tooltip("Radius to spawn in enemies")] float spawnRadius = 15f;
    [SerializeField, Tooltip("Radius for when enemies see the player")] float sightRadius = 10f;
    [SerializeField] float despawnRadius = 25f;
    [SerializeField] int numberOfEnemies = 5;

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform player;

    [Tooltip("Instansiate Gameobjects on transform for a clearer Hierarchy")] GameObject antiHierarchySpam;
    List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        SpawnEnemies(numberOfEnemies);
    }

    void Update()
    {
        EnemyMovement();
        DespawnEnemy();
    }

    void SpawnEnemies(int amount)
    {
        if (antiHierarchySpam == null) { antiHierarchySpam = GameObject.FindGameObjectWithTag("antiHierarchySpam"); }

        for (int i = 0; i < amount; i++)
        {
            Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
            randomPos.y = 0;
            Vector3 spawnPos = transform.position + randomPos;
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, antiHierarchySpam.transform);
            enemies.Add(enemy);
        }
    }

    void EnemyMovement()
    {
        //Moves enemies towards the player
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                float distanceToPlayer = Vector3.Distance(player.position, enemy.transform.position);

                //Check if the player is within the sight radius
                if (distanceToPlayer <= sightRadius)
                {
                    NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                    if (agent != null)
                    {
                        agent.SetDestination(player.position);
                    }
                }
                else
                {
                    //Stop moving if the player is out of range
                    NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                    if (agent != null && !agent.pathPending)
                    {
                        agent.ResetPath();
                    }
                }
            }
        }
    }

    void DespawnEnemy()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = enemies[i];
            if (enemy != null)
            {
                float distanceToSpawner = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToSpawner > despawnRadius)
                {
                    Destroy(enemy); 
                    enemies.RemoveAt(i); 
                    SpawnEnemies(1); 
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, despawnRadius);
    }
}