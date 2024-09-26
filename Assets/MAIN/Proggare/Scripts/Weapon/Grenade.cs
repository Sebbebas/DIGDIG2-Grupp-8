using UnityEngine;

public class Grenade : MonoBehaviour
{
    //Configurable Parameters
    [Header("General")]
    [SerializeField] ParticleSystem explosionParticle;
    [SerializeField] float speed = 8f;
    [SerializeField] float grenadeDrop = 3;

    //Private Variabels
    private Quaternion spawnRotation;
    private bool startFriction = false;

    //Cached References
    Rigidbody myRigidbody;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        spawnRotation = FindFirstObjectByType<PlayerLook>().GetComponentInParent<Transform>().rotation;

        transform.rotation = spawnRotation;
    }
    private void FixedUpdate()
    {
        if (myRigidbody != null)
        {
            if(!startFriction) { myRigidbody.AddForce(transform.forward * speed); }
            
            myRigidbody.mass += Time.deltaTime * grenadeDrop;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        //if (other.gameObject.CompareTag("Player")) { return; }
        //if (other.gameObject.layer = "Walkable") { startFriction = true; }
    }
}