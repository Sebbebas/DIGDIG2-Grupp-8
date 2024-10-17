using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Elian

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField, Tooltip("Radius to spawn in prefabList")] float spawnRadius = 15f;
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
        // Add all non-null enemies to the enemyPrefabs list
        if (enemyZero != null) enemyPrefabs.Add(enemyZero);
        if (enemyOne != null) enemyPrefabs.Add(enemyOne);
        if (enemyTwo != null) enemyPrefabs.Add(enemyTwo);
        if (enemyThree != null) enemyPrefabs.Add(enemyThree);
        if (enemyFour != null) enemyPrefabs.Add(enemyFour);

        // Ensure that there are enemies to spawn
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
        if (antiHierarchySpam == null) { antiHierarchySpam = GameObject.FindGameObjectWithTag("antiHierarchySpam"); }

        for (int i = 0; i < amount; i++)
        {
            Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
            randomPos.y = 0;
            Vector3 spawnPos = transform.position + randomPos;

            if (enemyPrefabs.Count > 0)
            {
                // Randomly select an enemy from the list
                GameObject randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

                GameObject enemy = Instantiate(randomEnemy, spawnPos, Quaternion.identity, antiHierarchySpam.transform);
                prefabList.Add(enemy);
            }
        }
    }

    void EnemyMovement()
    {
        //Moves enemyList towards the player
        foreach (GameObject enemy in prefabList)
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

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, despawnRadius);
    }
}