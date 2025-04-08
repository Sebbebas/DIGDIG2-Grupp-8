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

    [SerializeField] ParticleSystem bloodEffect;

    [Header("Gibbs")]
    [SerializeField] int numberOfGibbs = 5;
    [SerializeField] float gibbPower = 5f;
    [SerializeField] GameObject heart;
    [SerializeField] GameObject brain;
    [SerializeField] GameObject lungs;
    [SerializeField] GameObject meatOne;
    [SerializeField] GameObject meatTwo;
    [SerializeField] GameObject meatThree;
    [SerializeField] GameObject meatFour;

    EnemyScript enemyScript;

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

            //for (int i = 0; i < 3; i++)
            //{
            //    GameObject newGibb = Instantiate(heart, new Vector3(transform.position.x, transform.position.y + i, transform.position.z), Quaternion.identity);
            //    Rigidbody gibbRigidbody = newGibb.GetComponent<Rigidbody>();

            //    if (gibbRigidbody != null)
            //    {
            //        //gibbRigidbody.AddForce(new Vector3(direction * gibbPower, ForceMode.Impulse));
            //    }
            //}
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
}
