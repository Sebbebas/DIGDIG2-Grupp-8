using UnityEngine;
using UnityEngine.Rendering;

// Sebbe

public class Shotgun : Weapon
{
    [Header("Shotgun")]
    [SerializeField] GameObject temporaryHitParticel;
    [SerializeField] GameObject hitParticlePrefab;
    [SerializeField] float spread = 5;
    [SerializeField] int pellets = 10;
    public float pelletDamage = 40f;

    [SerializeField] private LayerMask headLayer;
    [SerializeField] private LayerMask torsoLayer;
    [SerializeField] private LayerMask leftArmLayer;
    [SerializeField] private LayerMask rightArmLayer;

    public new void Start()
    {
        base.Start();
    }

    public override bool Fire()
    {
        if (!base.Fire())
        {
            return false;
        }
        ShotGunFire();
        return true;
    }

    private void ShotGunFire()
    {
        for (int i = 0; i < pellets; i++)
        {
            Vector3 randomDirection = GetRandomSpreadDirection();
            Ray weaponRay = new Ray(mainCam.transform.position, randomDirection);
            RaycastHit hit = new();

            Debug.DrawRay(mainCam.transform.position, randomDirection * weaponRange, Color.red, 1.0f);

            if (Physics.Raycast(weaponRay, out hit, weaponRange, hitMask))
            {
                Vector3 hitPosition = hit.point;
                Quaternion hitRotation = Quaternion.LookRotation(hit.normal);

                //if(hit.transform.gameObject.tag == "Enemy Head" || hit.transform.tag == "Enemy Torso" || hit.transform.tag == "Enemy Left Arm" || hit.transform.tag == "Enemy Right Arm")
                //{
                //    Debug.Log("KYS");
                //    hit.transform.SendMessage("PartDetected");
                //}

                if (hit.transform.CompareTag("Grenade"))
                {
                    hit.transform.GetComponent<Grenade>().Explode();
                }
                else if (hit.transform.CompareTag("Enemies"))
                {
                    EnemyScript enemy = hit.transform.GetComponent<EnemyScript>();
                    Instantiate(hitParticlePrefab, hitPosition, hitRotation);
                    
                    //Debug.Log("Hit object with tag " + hit.transform.tag);

                    if (enemy != null)
                    {
                        enemy.ApplyDamage(pelletDamage);
                    }

                    /*if (hit.transform.CompareTag("Enemy Head"))
                    //{
                    //    //EnemyScript enemy = hit.transform.GetComponentInParent<EnemyScript>();
                    //    if (enemy != null)
                    //    {
                    //        enemy.ApplyDamageHead(pelletDamage);
                    //        Debug.Log("Head took " + pelletDamage);
                    //    }
                    //}
                    //else if (hit.transform.CompareTag("Enemy Torso"))
                    //{
                    //    //EnemyScript enemy = hit.transform.GetComponent<EnemyScript>();
                    //    if (enemy != null)
                    //    {
                    //        enemy.ApplyTorsoDamage(pelletDamage);
                    //        Debug.Log("Body took " + pelletDamage);
                    //    }
                    //}
                    //else if (hit.transform.CompareTag("Enemy Left Arm"))
                    //{
                    //    //EnemyScript enemy = hit.transform.GetComponent<EnemyScript>();
                    //    if (enemy != null)
                    //    {
                    //        enemy.ApplyLeftArmDamage(pelletDamage);
                    //        Debug.Log("Left arm took " + pelletDamage);
                    //    }
                    //}
                    //else if (hit.transform.CompareTag("Enemy Right Arm"))
                    //{
                    //    //EnemyScript enemy = hit.transform.GetComponent<EnemyScript>();
                    //    if (enemy != null)
                    //    {
                    //        enemy.ApplyRightArmDamage(pelletDamage);
                    //        Debug.Log("Right arm took " + pelletDamage);
                    //    }
                    //}*/
                }
                else if (hit.transform.CompareTag("Plank") && hit.transform.GetComponent<Plank>())
                {
                    Plank plank = hit.transform.GetComponent<Plank>();
                    plank.BreakPlanks(weaponRay.direction, 1, 15);
                }
                else
                {
                    //Instantiate(temporaryHitParticel, hitPosition, hitRotation, antiHierarchySpam.transform);
                }
            }
        }
    }

    private Vector3 GetRandomSpreadDirection()
    {
        float randomX = Random.Range(-spread, spread);
        float randomY = Random.Range(-spread, spread);
        float randomZ = Random.Range(-spread, spread);

        Vector3 direction = mainCam.transform.forward;
        direction = Quaternion.Euler(randomX, randomY, randomZ) * direction;

        return direction;
    }
}