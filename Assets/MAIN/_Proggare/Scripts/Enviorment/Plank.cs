using UnityEngine;

public class Plank : MonoBehaviour
{
    [SerializeField] GameObject[] plankGibs;
    [SerializeField] int gibAmmout;

    [Space]

    [SerializeField] AudioClip kickPlankSound, shootPlankSound;

    [Space]

    [SerializeField] BoxCollider hitBox;
    [SerializeField] MeshRenderer plank, brokenPlank;

    bool isBroken = false;

    AudioSource breakSound;

    void Start()
    {
        breakSound = GetComponent<AudioSource>();

        if(plank != null) { plank.enabled = true; }
        if(brokenPlank != null) { brokenPlank.enabled = false; }
    }

    public void BreakPlanks(Vector3 direction, int soundClip, int power)
    {
        Debug.Log("BreakPlanks called with soundClip: " + soundClip + " and power: " + power);

        if (isBroken)
        {
            return;
        }
        else if (soundClip == 0)
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

        isBroken = true;

        breakSound.Play();

        // Set the layer to default
        gameObject.layer = 0;

        if (hitBox != null) { hitBox.enabled = false; }
        if (plank != null) { plank.enabled = false; }
        if (brokenPlank != null) { brokenPlank.enabled = true; }

        for (int i = 0; i < gibAmmout; i++)
        {
            int randomIndex = Random.Range(0, plankGibs.Length);

            GameObject newPlank = Instantiate(plankGibs[randomIndex], new Vector3(transform.position.x, transform.position.y + i, transform.position.z), Quaternion.identity);
            Rigidbody plankRigidbody = newPlank.GetComponent<Rigidbody>();

            if (plankRigidbody != null)
            {
                plankRigidbody.AddForce(direction * power, ForceMode.Impulse);
            }
        }
    }
}
