using Unity.Mathematics;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    //Configurable Parameters
    [Header("General")]
    [SerializeField] ParticleSystem explosionParticle;
    [SerializeField] GameObject temporaryExplosion;
    [SerializeField] AudioSource explosion;
    [SerializeField] LayerMask slowdownLayer;

    [Header("Grenade Behaviour")]
    [SerializeField] float speed = 8f;
    [SerializeField] float upwardForce = 5f;
    [SerializeField] float customGravityMultiplier = 2f;
    [SerializeField] float grenadeSlowdownRate = 3f;
    [SerializeField] float aliveTime = 3f;

    //Private Variabels
    [Tooltip("Instansiate Gameobjects on transform for a clearer Hierarchy")] private GameObject antiHierarchySpam;
    private Quaternion spawnRotation;
    private bool startFriction = false;

    //Cached References
    Rigidbody myRigidbody;

    private void Start()
    {
        //Get Cached References
        myRigidbody = GetComponent<Rigidbody>();

        //Set object rotation
        spawnRotation = FindFirstObjectByType<PlayerLook>().GetComponentInParent<Transform>().rotation;
        transform.rotation = spawnRotation;

        //Initial force
        Vector3 throwDirection = transform.forward * speed + transform.up * upwardForce;
        myRigidbody.AddForce(throwDirection, ForceMode.VelocityChange);

        //AntiHierarchySpam
        if (antiHierarchySpam == null) { antiHierarchySpam = GameObject.FindGameObjectWithTag("antiHierarchySpam"); }
    }
    private void FixedUpdate()
    {
        if (myRigidbody != null)
        {
            // Apply custom gravity to make the grenade fall faster
            myRigidbody.AddForce(Physics.gravity * (customGravityMultiplier - 1), ForceMode.Acceleration);

            //If the grenade has touched the ground
            if (startFriction) { myRigidbody.linearVelocity = Vector3.Lerp(myRigidbody.linearVelocity, Vector3.zero, grenadeSlowdownRate * Time.deltaTime); }
        }

        if(aliveTime > 0) { aliveTime -= Time.deltaTime; }
        else { Explode(); }
    }
    public void Explode()
    {
        if (temporaryExplosion != null) { Instantiate(temporaryExplosion, transform.position, quaternion.identity, antiHierarchySpam.transform); }
        if (explosion != null) { explosion.Play(); }

        Destroy(this.gameObject);
    }
    private void OnCollisionEnter(Collision other)
    {
        //Use LayerMask to check if the collision is with the slowdown layer
        if (((1 << other.gameObject.layer) & slowdownLayer) != 0)
        {
            startFriction = true;
        }
    }
}