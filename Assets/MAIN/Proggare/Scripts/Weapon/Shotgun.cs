using UnityEngine;

//Sebbe

public class Shotgun : Weapon
{
    [Header("Shotgun")]
    [SerializeField] GameObject temporaryHitParticel; //Going to use actuall particals later
    [SerializeField] float spread = 5;
    [SerializeField] int pellets = 10;

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
            //Randomize the direction within the spread angle
            Vector3 randomDirection = GetRandomSpreadDirection();

            //Create a ray with randomized direction
            Ray weaponRay = new Ray(mainCam.transform.position, randomDirection);
            RaycastHit hit = new();

            //Debug draw each pellet's trajectory
            Debug.DrawRay(mainCam.transform.position, randomDirection * weaponRange, Color.red, 1.0f);

            //Cast the ray
            if (Physics.Raycast(weaponRay, out hit, weaponRange, hitMask))
            {
                //Object Transform
                Vector3 hitPosition = hit.point;
                Quaternion hitRotation = Quaternion.LookRotation(hit.normal);

                //Instansiate Objects on hit
                if (hit.transform.CompareTag("Grenade") == true) { hit.transform.GetComponent<Grenade>().Explode(); }
                else { Instantiate(temporaryHitParticel, hitPosition, hitRotation, antiHierarchySpam.transform); }
            }
        }
    }
    private Vector3 GetRandomSpreadDirection()
    {
        //Get a random offset
        float randomX = Random.Range(-spread, spread);
        float randomY = Random.Range(-spread, spread);
        float randomZ = Random.Range(-spread, spread);

        //Get the direction of the camera
        Vector3 direction = mainCam.transform.forward;

        //Apply the random spread
        direction = Quaternion.Euler(randomX, randomY, randomZ) * direction;

        return direction;
    }
}