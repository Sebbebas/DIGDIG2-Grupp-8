using UnityEngine;

// Seb

public class FullAutomatic : Weapon
{
    [Header("<color=magenta> Weapon Variabels")]

    [Header("Full Automatic")]
    [SerializeField] int damage = 10;

    public new void Start()
    {
        base.Start();
        SetIsHoldToFire(true);
    }

    public override bool Fire()
    {
        if (!base.Fire())
        {
            return false;
        }
        AutomaticFire();
        return true;
    }

    public void AutomaticFire()
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
