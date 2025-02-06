using UnityEngine;

// Sebbe

public class Shotgun : Weapon
{
    [Header("Shotgun")]
    [SerializeField] GameObject temporaryHitParticel;
    [SerializeField] float spread = 5;
    [SerializeField] int pellets = 10;
    [SerializeField] float pelletDamage = 40f;

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
                else if (hit.transform.CompareTag("Enemies"))
                {
                    EnemyScript enemy = hit.transform.GetComponent<EnemyScript>();
                    if (enemy != null)
                    {
                        enemy.ApplyDamage(pelletDamage);
                        //Debug.Log(pelletDamage);
                    }
                }
                else if (hit.transform.CompareTag("Plank"))
                {
                    Plank plank = hit.transform.GetComponent<Plank>();
                    plank.BreakPlanks();
                }

                /*if (hit.transform.CompareTag("Enemy Head"))
                {
                    EnemyScript enemy = hit.transform.GetComponent<EnemyScript>();
                    if (enemy != null)
                    {
                        enemy.ApplyDamage(2 * pelletDamage);
                        Debug.Log(pelletDamage);
                    }
                }
                if (hit.transform.CompareTag("Enemy Torso"))
                {
                    EnemyScript enemy = hit.transform.GetComponent<EnemyScript>();
                    if (enemy != null)
                    {
                        enemy.ApplyDamage(pelletDamage);
                        Debug.Log(pelletDamage);
                    }
                }
                if (hit.transform.CompareTag("Enemy Leg"))
                {
                    EnemyScript enemy = hit.transform.GetComponent<EnemyScript>();
                    if (enemy != null)
                    {
                        enemy.ApplyDamage(pelletDamage);
                        Debug.Log(pelletDamage);
                    }
                }
                if (hit.transform.CompareTag("Enemy Arm"))
                {
                    EnemyScript enemy = hit.transform.GetComponent<EnemyScript>();
                    if (enemy != null)
                    {
                        enemy.ApplyDamage(pelletDamage);
                        Debug.Log(pelletDamage);
                    }
                }*/
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