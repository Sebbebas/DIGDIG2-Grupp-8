using System.Collections.Generic;
using UnityEngine;

public class ZombieBodyParts : MonoBehaviour
{
    /*[HideInInspector] public string bodyPartTorso = "Torso";
    //[HideInInspector] public string bodyPartHead = "Head";
    //[HideInInspector] public string bodyPartLeftArm = "Left Arm";
    //[HideInInspector] public string bodyPartRightArm = "Right Arm";

    //[HideInInspector] public EnemyScript headHealth;
    //[HideInInspector] public EnemyScript torsoHealth;
    //[HideInInspector] public EnemyScript leftArmHealth;
    //[HideInInspector] public EnemyScript rightArmHealth;

    //void Start()
    //{
    //    headHealth = GetComponentInParent<EnemyScript>();
    //    torsoHealth = GetComponentInParent<EnemyScript>();
    //    leftArmHealth = GetComponentInParent<EnemyScript>();
    //    rightArmHealth = GetComponentInParent<EnemyScript>();
    //}

    //void OnCollisionEnter(Collision collision)
    //{
    //    torsoHealth.TakeDamage(bodyPartTorso, GetComponent<Shotgun>().pelletDamage);
    //    headHealth.TakeDamage(bodyPartHead, GetComponent<Shotgun>().pelletDamage);
    //    leftArmHealth.TakeDamage(bodyPartLeftArm, GetComponent<Shotgun>().pelletDamage);
    //    rightArmHealth.TakeDamage(bodyPartRightArm, GetComponent<Shotgun>().pelletDamage);
    //}*/

    [SerializeField] float minForce = 5f;
    [SerializeField] float maxForce = 10f;
    [SerializeField] float spawnGibbOffset = .5f;
 
    [Header("Gibbs")]
    [SerializeField] int numberOfMinGibbs = 4;
    [SerializeField] int numberOfMaxGibbs = 6;
    [SerializeField] float gibbPower = 5f;
    [SerializeField] GameObject[] gibbs;
    [SerializeField] ParticleSystem bloodEffect;
    //[SerializeField] GameObject heart;
    //[SerializeField] GameObject brain;
    //[SerializeField] GameObject lungs;
    //[SerializeField] GameObject meatOne;
    //[SerializeField] GameObject meatTwo;
    //[SerializeField] GameObject meatThree;
    //[SerializeField] GameObject meatFour;

    EnemyScript enemyScript;
    Destroy destroy;

    private void Awake()
    {
        enemyScript = GetComponent<EnemyScript>();
    }

    public void PartDetected()
    {
        if (transform.tag == "Enemy Head")
        {
            Debug.Log("give Head");
            enemyScript.Die();
        }

        if (transform.tag == "Enemy Torso")
        {
            Debug.Log("Torso shot");
            enemyScript.Die();
        }

        if (transform.tag == "Enemy Left Arm")
        {
            Debug.Log("Left Arm shot");
        }

        if (transform.tag == "Enemy Right Arm")
        {
            Debug.Log("Right Arm shot");
        }
    }

    public void SpawnGibbs()
    {
        int numberOfGibbsSpawned = Random.Range(numberOfMinGibbs, numberOfMaxGibbs);

        List<int> availableGibbsIndices = new List<int>();
        for (int i = 0; i < gibbs.Length; i++)
        {
            availableGibbsIndices.Add(i);
        }

        for (int i = 0; i < availableGibbsIndices.Count; i++)
        {
            int temp = availableGibbsIndices[i];
            int randomIndex = Random.Range(i, availableGibbsIndices.Count);
            availableGibbsIndices[i] = availableGibbsIndices[randomIndex];
            availableGibbsIndices[randomIndex] = temp;
        }

        for (int i = 0; i < numberOfGibbsSpawned && availableGibbsIndices.Count > 0; i++)
        {
            int randomIndex = availableGibbsIndices[0];  
            availableGibbsIndices.RemoveAt(0);  

            GameObject selectedGibb = gibbs[randomIndex];

            Rigidbody gibbRigidbody = selectedGibb.GetComponent<Rigidbody>();

            GameObject spawnedGibb = Instantiate(selectedGibb, new Vector3(transform.position.x, transform.position.y + i, transform.position.z), Quaternion.identity);

            Vector3 direction = (GameObject.Find("Player").transform.position - transform.position).normalized;
            Vector3 randomOffset = Random.insideUnitSphere * spawnGibbOffset;
            direction += randomOffset;
            direction.Normalize();
            float force = Random.Range(minForce, maxForce);
            gibbRigidbody = spawnedGibb.GetComponent<Rigidbody>();
            gibbRigidbody.AddForce(-direction * force, ForceMode.Impulse);

            if (bloodEffect != null)
            {
                Instantiate(bloodEffect, transform.position, Quaternion.identity);
            }
        }
    }
}
