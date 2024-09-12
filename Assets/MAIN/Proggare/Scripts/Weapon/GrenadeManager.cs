using UnityEngine;

public class GrenadeManager : MonoBehaviour
{
    //Configurable Parameters
    [SerializeField] GameObject grenadePrefab;

    public void ThrowGrenade()
    {
        if (grenadePrefab != null) { Instantiate(grenadePrefab); }

        Debug.Log("Threw Grenade");
    }
}