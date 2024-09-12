using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float damageAmount = 10f;

    void OnCollisionEnter(Collision collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.ApplyDamage(damageAmount);
        }
    }
}