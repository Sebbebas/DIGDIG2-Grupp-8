using UnityEngine;
using System.Collections.Generic;

public class Explosion : MonoBehaviour
{
    //Configurable
    [Header("Main")]
    [SerializeField] float maxDamage = 100f;
    [SerializeField] float damageReductionOverDistance = 3;
    [SerializeField] LayerMask effectedObjects;
    [SerializeField] List<Transform> effectedObjectList = new List<Transform>();

    [Header("damage over % of distance")]
    [SerializeField] float take100;
    [SerializeField] float take50;
    [SerializeField] float take20;

    private float explosionRadius;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object's layer is within the specified LayerMask
        if (((1 << other.gameObject.layer) & effectedObjects) != 0)
        {
            //Add the object to the list if it isn't already in it
            if (!effectedObjectList.Contains(other.transform))
            {
                effectedObjectList.Add(other.transform);
                CalculateDamage(other.transform);
            }
        }
    }

    private void CalculateDamage(Transform other)
    {
        explosionRadius = transform.localScale.y / 2;

        float distance = Vector3.Distance(transform.position, other.position);
        float distancePercent = (distance / explosionRadius) * 100f;
        float calculatedDamage = 0f;

        Debug.Log(distancePercent);

        if (distancePercent <= take100)
        {
            calculatedDamage = maxDamage; //100% damage
        }
        else if (distancePercent <= take50)
        {
            calculatedDamage = maxDamage * 0.5f; // 50% damage
        }
        else if (distancePercent >= take20)
        {
            calculatedDamage = maxDamage * 0.3f; // 30% damage
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().ApplyDamage(calculatedDamage);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            other.gameObject.GetComponent<EnemyScript>().ApplyDamage(calculatedDamage);
        }

        Debug.Log($"Applied damage: {calculatedDamage}", other.transform.gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        //Damage Radius
        explosionRadius = transform.localScale.y / 2;

        //Every thing inside takes 100% of maxDamage
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius * (take100 / 100f));

        //Every thing inside takes 50% of maxDamage
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, explosionRadius * (take50 / 100f));

        //Every thing inside takes 30% of maxDamage
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, explosionRadius * (take20 / 100f));
    }
}
