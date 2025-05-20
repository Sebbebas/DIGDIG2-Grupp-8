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
            Vector3 hitPosition = hit.point;
            Quaternion hitRotation = Quaternion.LookRotation(hit.normal);

            // Apply damage (existing logic)
            HitDetection(hit, weaponRay, damage);

            // --- Begin Kick Logic Integration ---

            // Apply force if the hit object has a Rigidbody
            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null && !rb.isKinematic)
            {
                // Use a force value similar to kickForce from WeaponManager, or define your own
                float meleeForce = 2.4f; // You can expose this as a [SerializeField] if needed
                rb.AddForce(mainCam.transform.forward * meleeForce, ForceMode.Impulse);
            }

            // Optionally, you can implement a cooldown for melee attacks here if desired
            // (not shown, as your Weapon base class may already handle fire delays)
            // --- End Kick Logic Integration ---
        }
    }
}
