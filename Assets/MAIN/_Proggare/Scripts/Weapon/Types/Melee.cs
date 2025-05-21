using UnityEngine;

// Seb

public class Melee : Weapon
{
    //[Header("<color=magenta> Weapon Variabels")]
    //USING CUSTOM EDITOR SCRIPT

    [Header("Melee")]
    [SerializeField] int damage = 10;

    public new void Start()
    {
        base.Start();
        SetIsHoldToFire(false);
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
            if (hit.transform.GetComponent<EnemyScript>()) { hit.transform.GetComponent<EnemyScript>().TakeKnockback(-direction); }
        }
    }
}
