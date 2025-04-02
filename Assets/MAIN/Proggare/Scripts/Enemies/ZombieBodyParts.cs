using UnityEngine;

public class ZombieBodyParts : MonoBehaviour
{
    [HideInInspector] public string bodyPartTorso = "Torso";
    [HideInInspector] public string bodyPartHead = "Head";
    [HideInInspector] public string bodyPartLeftArm = "Left Arm";
    [HideInInspector] public string bodyPartRightArm = "Right Arm";

    [HideInInspector] public EnemyScript headHealth;
    [HideInInspector] public EnemyScript torsoHealth;
    [HideInInspector] public EnemyScript leftArmHealth;
    [HideInInspector] public EnemyScript rightArmHealth;

    void Start()
    {
        headHealth = GetComponentInParent<EnemyScript>();
        torsoHealth = GetComponentInParent<EnemyScript>();
        leftArmHealth = GetComponentInParent<EnemyScript>();
        rightArmHealth = GetComponentInParent<EnemyScript>();
    }

    void OnCollisionEnter(Collision collision)
    {
        torsoHealth.TakeDamage(bodyPartTorso, GetComponent<Shotgun>().pelletDamage);
        headHealth.TakeDamage(bodyPartHead, GetComponent<Shotgun>().pelletDamage);
        leftArmHealth.TakeDamage(bodyPartLeftArm, GetComponent<Shotgun>().pelletDamage);
        rightArmHealth.TakeDamage(bodyPartRightArm, GetComponent<Shotgun>().pelletDamage);
    }
}
