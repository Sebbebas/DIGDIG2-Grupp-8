using UnityEngine;

public class Grenade : MonoBehaviour
{
    //Configurable Parameters
    [Header("General")]
    [SerializeField] ParticleSystem explosionParticle;
    [SerializeField] float speed = 8f;
    [SerializeField] Vector3 directionOffset;

    [Header("Current direction")]
    [SerializeField] Vector3 direction;

    //Cached References
    Rigidbody myRigidbody;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        direction = transform.forward.normalized;
        direction = direction + directionOffset.normalized;

        if (myRigidbody != null)
        {
            myRigidbody.AddForce(direction * speed);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player")) { return; }

        Vector3 hitPos = transform.position;
        if (explosionParticle != null) { Instantiate(explosionParticle, hitPos, Quaternion.identity); }
        Destroy(gameObject);
    }
}