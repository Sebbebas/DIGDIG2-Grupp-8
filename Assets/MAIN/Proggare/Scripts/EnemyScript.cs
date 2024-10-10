using UnityEngine;
using UnityEngine.AI;

// Alexander
public class EnemyScript : MonoBehaviour
{
    //Configurable Perameters
    [SerializeField] float DamageAmount = 20f;
    [SerializeField] float stunTime = 1f;
    [SerializeField] float maxKnockbackVelocity = 5;

    //Private Variabels
    private Vector3 kickDirection;
    private float enemySpeedAtStart;
    private bool isStuned;
    private float currentStunTime;
    
    //Chaced References
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
        if (isStuned)
        {
            myRigidbody.AddForce(kickDirection, ForceMode.Force);

            if (currentStunTime > 0) { currentStunTime -= Time.deltaTime; }
            else 
            { 
                currentStunTime = 0; 
                isStuned = false;
                agent.isStopped = false;
                agent.speed = enemySpeedAtStart;
            }
        }
        if (Input.GetKeyDown(KeyCode.B)) { Kicked(new(5, 5, 5)); } // DELETE THIS
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.ApplyDamage(DamageAmount);
            Debug.Log("Enemy collided with player, damage applied: " + DamageAmount);
        }
    }

    public void Kicked(Vector3 direction)
    {
        //Stun
        isStuned = true;
        agent.isStopped = true;
        currentStunTime = stunTime;
        kickDirection = direction;
        agent.speed = 0;
    }
}