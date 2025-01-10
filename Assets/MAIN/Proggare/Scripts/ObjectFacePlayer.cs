using UnityEngine;

public class ObjectFacePlayer : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
    }

    void Update()
    {
        if (player != null)
        {
            transform.LookAt(player.transform);
        }
    }
}