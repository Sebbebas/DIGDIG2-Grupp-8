using UnityEngine;

//Alexander

public class ZombieProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private GameObject sludgePrefab;
    private float slowMultiplier = 0.7f; //30% slowdown
    private int damage = 5;

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerMovement != null)
            {
                playerMovement.ApplySpeedMultiplier(slowMultiplier, 3f); //Slowdown for 3 seconds
            }
            if (playerHealth != null)
            {
                playerHealth.ApplyDamage(damage);
            }

            Destroy(gameObject);
        }
        else if (other.CompareTag("Floor") || other.CompareTag("Wall"))
        {
            Instantiate(sludgePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
