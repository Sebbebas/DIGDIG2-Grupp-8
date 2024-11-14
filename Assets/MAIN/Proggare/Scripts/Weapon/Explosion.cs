using UnityEngine;
using System.Collections.Generic;

public class Explosion : MonoBehaviour
{
    [SerializeField] float maxDamage = 100f;
    [SerializeField] float damageReductionOverDistance = 3;
    [SerializeField] LayerMask effectedObjects;
    [SerializeField] List<Transform> effectedObjectList = new List<Transform>();


    private void OnTriggerEnter(Collider other)
    {
        // Check if the object's layer is within the specified LayerMask
        if (((1 << other.gameObject.layer) & effectedObjects) != 0)
        {
            //Add the object to the list if it isn't already in it
            if (!effectedObjectList.Contains(other.transform))
            {
                effectedObjectList.Add(other.transform);
                CalcuateDamage(other.transform);
            }
        }
    }

    void CalcuateDamage(Transform other)
    {
        float distance = Vector3.Distance(transform.position, other.position);

        float damage = maxDamage - Mathf.RoundToInt(distance * damageReductionOverDistance);

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().ApplyDamage(damage);
        }
        else
        {
            other.gameObject.GetComponent<EnemyScript>().ApplyDamage(damage);
        }
        Debug.Log(damage, other.transform.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        //Visualize the explosion radius in the editor
        Gizmos.color = Color.red;

        //Remove when have expolsion paricle effect
        Gizmos.DrawWireSphere(transform.position, transform.localScale.y / 2);

        //Use when have expolsion paricle effect
        //Gizmos.DrawSphere(transform.position, transform.localScale.y / 2);
    }
}
