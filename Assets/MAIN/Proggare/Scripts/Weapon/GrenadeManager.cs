using UnityEngine;

public class GrenadeManager : MonoBehaviour
{
    //Configurable Parameters
    [SerializeField] GameObject grenadePrefab;
    [SerializeField] Vector3 spawnOffset;

    //Private Variabels
    [SerializeField] private GameObject antiHierarchySpam;
    
    public void ThrowGrenade()
    {
        antiHierarchySpam = GameObject.FindGameObjectWithTag("antiHierarchySpam");

        Vector3 spawnPos = GetComponentInParent<Transform>().position + spawnOffset;

        if (antiHierarchySpam != null && grenadePrefab != null) 
        { 
            Instantiate(grenadePrefab, spawnPos, GetComponentInParent<Transform>().rotation, antiHierarchySpam.transform);

            Debug.Log("Threw Grenade");
        }
        else
        {
            Debug.Log("grenade error");
        }

    }
}