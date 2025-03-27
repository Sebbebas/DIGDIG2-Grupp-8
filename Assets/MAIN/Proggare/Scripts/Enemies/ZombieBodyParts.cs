using UnityEngine;

public class ZombieBodyParts : MonoBehaviour
{
    [HideInInspector] public string bodyPartTorso = "Torso";
    [HideInInspector] public string bodyPartHead = "Head";
    [HideInInspector] public string bodyPartArms = "Arm";

    [HideInInspector] public EnemyScript enemyHealth;

    void Start()
    {
        enemyHealth = GetComponentInParent<EnemyScript>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            enemyHealth.TakeDamage(bodyPartTorso, GetComponent<Shotgun>().pelletDamage);
            enemyHealth.TakeDamage(bodyPartHead, GetComponent<Shotgun>().pelletDamage);
            enemyHealth.TakeDamage(bodyPartArms, GetComponent<Shotgun>().pelletDamage);
        }
    }
}
