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
        Ray weaponRay = new Ray(mainCam.transform.position, mainCam.transform.forward);
        RaycastHit hit = new();

        Debug.DrawRay(mainCam.transform.position, mainCam.transform.forward * weaponRange, Color.red, 1.0f);

        if (Physics.Raycast(weaponRay, out hit, weaponRange, hitMask))
        {
            Vector3 hitPosition = hit.point;
            Quaternion hitRotation = Quaternion.LookRotation(hit.normal);

            HitDetection(hit, weaponRay, damage);
        }
    }
}
