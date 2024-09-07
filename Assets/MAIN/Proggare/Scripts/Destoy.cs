using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destoy : MonoBehaviour
{
    [SerializeField] float aliveTime = 3f;

    void Start()
    {
        Destroy(this.gameObject, aliveTime);
    }
}
