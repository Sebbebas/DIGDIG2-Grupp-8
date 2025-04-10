using UnityEngine;

public class DropCrates : MonoBehaviour
{
    [SerializeField] GameObject reinforcedCrates;
    [SerializeField] Transform[] spawnPoints;

    bool hasTriggerd = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggerd) return;

        if (other.CompareTag("Player"))
        {
            foreach (Transform t in spawnPoints)
            { 
                Instantiate(reinforcedCrates, t.position, t.rotation);
                hasTriggerd = true;
            }
        }
    }
}
