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
    [SerializeField] int pelletDamage = 40;

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

                HitDetection(hit, weaponRay, pelletDamage);
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