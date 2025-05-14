using UnityEngine;

// Seb

public class Template : Weapon
{
    //[Header("<color=magenta> Weapon Variabels")]
    //USING CUSTOM EDITOR SCRIPT

    [Header("Template")]
    [SerializeField] int damage = 10;

    public new void Start()
    {
        base.Start();
        SetIsHoldToFire(true); //Set to true if you want to hold the Fire button to shoot
    }

    public override bool Fire()
    {
        if (!base.Fire())
        {
            return false;
        }
        TemplateFire();
        return true;
    }

    public void TemplateFire() //Give resonable name to the function
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
