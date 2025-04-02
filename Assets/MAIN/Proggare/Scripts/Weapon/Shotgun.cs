using UnityEngine;

// Sebbe

public class Shotgun : Weapon
{
    [Header("Shotgun")]
    [SerializeField] GameObject temporaryHitParticel;
    [SerializeField] float spread = 5;
    [SerializeField] int pellets = 10;
    public float pelletDamage = 40f;

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

                if (hit.transform.CompareTag("Grenade"))
                {
                    hit.transform.GetComponent<Grenade>().Explode();
                }

                //else if (hit.transform.CompareTag("Enemies"))
                //{
                //    ZombieBodyParts enemyPart = hit.collider.GetComponent<ZombieBodyParts>();
                //    if (hit.transform.CompareTag("Enemy Head"))
                //    {
                //        if (enemyPart != null)
                //        {
                //            enemyPart.headHealth.TakeDamage(enemyPart.bodyPartHead, pelletDamage);
                //        }
                //    }
                //    else if (hit.transform.CompareTag("Enemy Torso"))
                //    {
                //        if (enemyPart != null)
                //        {
                //            enemyPart.headHealth.TakeDamage(enemyPart.bodyPartTorso, pelletDamage);
                //        }
                //    }
                //    else if (hit.transform.CompareTag("Enemy Left Arm"))
                //    { 
                //        if (enemyPart != null)
                //        {
                //            enemyPart.headHealth.TakeDamage(enemyPart.bodyPartLeftArm, pelletDamage);
                //        }
                //    }
                //    else if (hit.transform.CompareTag("Enemy Right Arm"))
                //    {
                //        if (enemyPart != null)
                //        {
                //            enemyPart.headHealth.TakeDamage(enemyPart.bodyPartRightArm, pelletDamage);
                //        }
                //    }

                else if (hit.transform.CompareTag("Enemies"))
                {
                    EnemyScript enemy = hit.transform.GetComponent<EnemyScript>();
                    if (enemy != null)
                    {
                        enemy.ApplyDamage(pelletDamage / 1000);
                        //Debug.Log(pelletDamage);
                    }

                }

                if (hit.transform.CompareTag("Enemy Head"))
                {
                    EnemyScript enemy = hit.transform.GetComponentInParent<EnemyScript>();
                    if (enemy != null)
                    {
                        enemy.ApplyDamageHead(pelletDamage);
                        Debug.Log(pelletDamage);
                    }
                }

                else if (hit.transform.CompareTag("Plank"))
                {
                    Plank plank = hit.transform.GetComponent<Plank>();
                    plank.BreakPlanks(weaponRay.direction);
                }
                else
                {
                    Instantiate(temporaryHitParticel, hitPosition, hitRotation, antiHierarchySpam.transform);
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