using UnityEngine;

// Seb

public class Shotgun : Weapon
{
    [Header("<color=magenta> Weapon Variabels")]

    [Header("Shotgun")]
    [SerializeField] float spread = 5;
    [SerializeField] int pellets = 10;
    [SerializeField] int pelletDamage = 40;

    [Space]

    [Header("GUI")]
    [SerializeField, Tooltip("Add empty and Loaded Shell Objects")] GameObject[] shellOne;
    [SerializeField, Tooltip("Add empty and Loaded Shell Objects")] GameObject[] shellTwo;

    [Header("borde flytta till weapon när man ska implementera")]
    [SerializeField] private LayerMask headLayer;
    [SerializeField] private LayerMask torsoLayer;
    [SerializeField] private LayerMask leftArmLayer;
    [SerializeField] private LayerMask rightArmLayer;

    public new void Start()
    {
        //Call base Method
        base.Start();
        
        SetIsHoldToFire(false);
    }
    public new void OnEnable()
    {
        //Call base Method
        base.OnEnable();

        UpdateShellGUI();
    }
    public new void OnDisable()
    {
        //Call base Method
        base.OnDisable();

        //Deactivate all shell objects
        for (int i = 0; i < shellOne.Length; i++)
        {
            shellOne[i].SetActive(false);
            shellTwo[i].SetActive(false);
        }
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

    public override bool FinishedReload()
    {
        if (base.FinishedReload())
        {
            UpdateShellGUI();
        }
        return base.FinishedReload();
    }

    private void ShotGunFire()
    {
        UpdateShellGUI();

        //FIRE
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

    private void UpdateShellGUI()
    {
        if (currentAmmo == 2)
        {
            shellOne[0].SetActive(true);
            shellTwo[0].SetActive(true);
        }
        else if (currentAmmo == 1)
        {
            shellOne[0].SetActive(false);
            shellTwo[0].SetActive(true);
        }
        else if (currentAmmo == 0)
        {
            shellOne[0].SetActive(false);
            shellTwo[0].SetActive(false);
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