using UnityEngine;

// Alexander
public class EnemyScript : MonoBehaviour
{
    [SerializeField] float DamageAmount = 20f;

    void OnCollisionEnter(Collision collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.ApplyDamage(DamageAmount);
            Debug.Log("Enemy collided with player, damage applied: " + DamageAmount);
        }
    }
}