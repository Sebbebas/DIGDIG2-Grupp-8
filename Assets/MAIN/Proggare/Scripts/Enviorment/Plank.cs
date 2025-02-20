using UnityEngine;

public class Plank : MonoBehaviour
{
    [SerializeField] GameObject plankObject;

    [Space]

    [SerializeField] BoxCollider hitBox;
    [SerializeField] MeshRenderer plank, brokenPlank;

    void Start()
    {
        plank.enabled = true;
        brokenPlank.enabled = false;
    }

    public void BreakPlanks(Vector3 direction)
    {
        hitBox.enabled = false;
        plank.enabled = false;
        brokenPlank.enabled = true;

        for (int i = 0; i < 3; i++)
        {
            GameObject newPlank = Instantiate(plankObject, transform.position, Quaternion.identity);
            Rigidbody plankRigidbody = newPlank.GetComponent<Rigidbody>();

            if (plankRigidbody != null)
            {
                plankRigidbody.AddForce(direction * 5f, ForceMode.Impulse);
            }
        }
    }
}
