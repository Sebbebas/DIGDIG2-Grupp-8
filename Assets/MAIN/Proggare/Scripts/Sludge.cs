using UnityEngine;

//Alexander

public class Sludge : MonoBehaviour
{
    [SerializeField] float slowMultiplier = 0.70f; //15% slowdown
    [SerializeField] float slowDuration = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

            if (playerMovement != null)
            {
                playerMovement.ApplySpeedMultiplier(slowMultiplier, slowDuration);
            }
        }
    }
}