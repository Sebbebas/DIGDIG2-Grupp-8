using UnityEngine;

//Alexander

public class LookAtPlayer : MonoBehaviour
{
    public GameObject player;

    void Update()
    {
        transform.LookAt(player.transform);
    }
}