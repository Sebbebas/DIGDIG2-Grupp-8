using UnityEngine;

public class Plank : MonoBehaviour
{
    [SerializeField] GameObject plankObject;

    [Space]

    [SerializeField] AudioClip kickPlankSound, shootPlankSound;

    [Space]

    [SerializeField] BoxCollider hitBox;
    [SerializeField] MeshRenderer plank, brokenPlank;
    
    AudioSource breakSound;

    void Start()
    {
        breakSound = GetComponent<AudioSource>();

        plank.enabled = true;
        brokenPlank.enabled = false;
    }

    public void BreakPlanks(Vector3 direction, int soundClip, int power)
    {
        if (soundClip == 0)
        {
            breakSound.clip = kickPlankSound;
        }
        else if (soundClip == 1)
        {
            breakSound.clip = shootPlankSound;
        }
        else
        {
            Debug.LogError("Invalid sound clip index. Use 0 for kickPlankSound or 1 for shootPlankSound.");
        }

        breakSound.Play();

        hitBox.enabled = false;
        plank.enabled = false;
        brokenPlank.enabled = true;

        for (int i = 0; i < 3; i++)
        {
            GameObject newPlank = Instantiate(plankObject, new Vector3 (transform.position.x, transform.position.y + i, transform.position.z), Quaternion.identity);
            Rigidbody plankRigidbody = newPlank.GetComponent<Rigidbody>();

            if (plankRigidbody != null)
            {
                plankRigidbody.AddForce(direction * power, ForceMode.Impulse);
            }
        }
    }
}
