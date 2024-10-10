using UnityEngine;
using UnityEngine.AI;
using System.Collections;

// Alexander
public class EnemyScript : MonoBehaviour
{
    // Configurable Parameters
    [Header("Player affecting values")]
    [SerializeField] float DamageAmount = 20f;
    [SerializeField] float stunTime = 1f;
    [SerializeField] float maxKnockbackVelocity = 5;
    [SerializeField] float damageCooldown = 2f; //Cooldown time between attacks

    // Private Variables
    private Vector3 kickDirection;
    private float enemySpeedAtStart;
    private bool isStunned;
    private float currentStunTime;
    private bool canDamage = true; //Tracks if the enemy can damage the player

    //Cached References
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

        //DELETE THIS
        if (Input.GetKeyDown(KeyCode.B))
        {
            Kicked(new Vector3(5, 5, 5));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null && canDamage)
        {
            playerHealth.ApplyDamage(DamageAmount);
            Debug.Log("Enemy collided with player, damage applied: " + DamageAmount);

            //Start cooldown before enemy can deal damage again
            StartCoroutine(DamageCooldown());
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDamage = false; //Prevent further damage
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true; //Allow damage again
    }

    public void Kicked(Vector3 direction)
    {
        isStunned = true;
        agent.isStopped = true;
        currentStunTime = stunTime;
        kickDirection = direction;
        agent.speed = 0;
    }
}