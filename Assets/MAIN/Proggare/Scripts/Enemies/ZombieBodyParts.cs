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

    [SerializeField] float force = 10f;
    [SerializeField] float rotation = 5f;
 
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

        for (int i = 0; i < numberOfGibbsSpawned; i++)
        { 
            int randomIndex = Random.Range(0, gibbs.Length);
            GameObject selectedGibb = gibbs[randomIndex];

            GameObject spawnedGibb = Instantiate(selectedGibb, new Vector3(transform.position.x, transform.position.y + i, transform.position.z), Quaternion.identity);
            Rigidbody gibbRigidbody = spawnedGibb.GetComponent<Rigidbody>();

            if (bloodEffect != null)
            {
                Instantiate(bloodEffect); 
            }

            if (gibbRigidbody != null)
            {
                gibbRigidbody.AddForce(transform.forward * force, ForceMode.Impulse);
                gibbRigidbody.AddTorque(new Vector3(0f, rotation, 0f), ForceMode.Impulse);
            }
        }

        destroy = GetComponent<Destroy>();
        if (destroy != null)
        {
            destroy.enabled = true;
            destroy.Destruct();
        }

        //GameObject newPlank = Instantiate(plankObject, new Vector3(transform.position.x, transform.position.y + i, transform.position.z), Quaternion.identity);
        //Rigidbody plankRigidbody = newPlank.GetComponent<Rigidbody>();

        //if (plankRigidbody != null)
        //{
        //    plankRigidbody.AddForce(direction * power, ForceMode.Impulse);
        //}
    }
}
