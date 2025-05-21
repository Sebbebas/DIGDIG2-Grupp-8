using UnityEngine;

// Seb

public class Melee : Weapon
{
    //[Header("<color=magenta> Weapon Variabels")]
    //USING CUSTOM EDITOR SCRIPT

    [Header("Melee")]
    [SerializeField] AudioClip hitSound;
    [SerializeField] int damage = 10;

    //Chaced References
    ScreenShake screenShake;

    public new void Start()
    {
        base.Start();
        SetIsHoldToFire(false);

        screenShake = FindFirstObjectByType<ScreenShake>();
    }

    public override bool Fire()
    {
        if (!base.Fire())
        {
            return false;
        }
        MeleeAttack();
        return true;
    }

    public void MeleeAttack()
    {
        // Raycast setup
        Ray weaponRay = new Ray(mainCam.transform.position, mainCam.transform.forward);
        RaycastHit hit = new();

        Debug.DrawRay(mainCam.transform.position, mainCam.transform.forward * weaponRange, Color.red, 1.0f);

        if (Physics.Raycast(weaponRay, out hit, weaponRange, hitMask))
        {
            Quaternion hitRotation = Quaternion.LookRotation(hit.normal);
            Vector3 direction = (hitRotation * Vector3.forward).normalized;

            // Apply damage (existing logic)
            base.HitDetection(hit, weaponRay, damage);

            // Apply force if the hit object has a Rigidbody
            if (hit.transform.GetComponent<EnemyScript>()) 
            { 
                hit.transform.GetComponent<EnemyScript>().TakeKnockback(-direction);

                if (shakeOnHit) { screenShake.Shake(screenShakeDuration, screenShakeIntensity); }

                //Object
                GameObject hitSpawnSoundObject = new();

                //Destroy
                hitSpawnSoundObject.AddComponent<Destroy>();
                hitSpawnSoundObject.GetComponent<Destroy>().SetAliveTime(hitSound.length);

                //Audio
                hitSpawnSoundObject.AddComponent<AudioSource>();
                hitSpawnSoundObject.GetComponent<AudioSource>().clip = hitSound;
                hitSpawnSoundObject.GetComponent<AudioSource>().Play();

                //Spawn Object
                Instantiate(hitSpawnSoundObject, hit.point, hitRotation);
            }
        }
    }
}
