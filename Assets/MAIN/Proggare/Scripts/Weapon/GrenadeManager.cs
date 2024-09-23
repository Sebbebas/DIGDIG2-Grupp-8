using UnityEngine;

public class GrenadeManager : MonoBehaviour
{
    //Configurable Parameters
    [SerializeField] GameObject grenadePrefab;

    //Private Variabels
    [SerializeField] private GameObject antiHierarchySpam;
    
    public void ThrowGrenade()
    {
        antiHierarchySpam = GameObject.FindGameObjectWithTag("antiHierarchySpam");

        if (antiHierarchySpam != null && grenadePrefab != null) 
        { 
            Instantiate(grenadePrefab, GetComponentInParent<Transform>().position, GetComponentInParent<Transform>().rotation, antiHierarchySpam.transform);

            Debug.Log("Threw Grenade");
        }
        else
        {
            Debug.Log("grenade error");
        }

    }
}