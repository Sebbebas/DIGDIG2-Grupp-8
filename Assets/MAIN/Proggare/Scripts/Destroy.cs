using UnityEngine;

public class Destroy : MonoBehaviour
{
    [SerializeField] float aliveTime = 3f;

    void Start()
    {
        Destroy(this.gameObject, aliveTime);
    }

    public void SetAliveTime(float time)
    {
        aliveTime = time;
    }
}
