using UnityEngine;
using System.Collections;
using TMPro;

public class Weapon : MonoBehaviour
{
    //Configurable Perameters
    [Header("Ammo")]
    public int currentAmmo;
    public int totalAmmo = 90;
    public int magSize = 30;
    public TextMeshProUGUI ammoText;

    [Header("Delays")]
    public float reloadTime = 3f;
    public float firedelay = 3f;

    [Header("Extra")]
    [Tooltip("Instansiate bullets on this transform to not fill Hierarchy")] public GameObject parentForBullets;
    public LayerMask hitMask = 0;
    public float weaponRange = 100f;
    protected Camera mainCam = null;

    [Header("Effects")]
    public ParticleSystem hitParticle;
    public Animator animator;

    //Private Variabels
    private float currentFireDelay;
    private bool reloading = false; 

    protected void Start()
    {
        mainCam = Camera.main;

        if (currentAmmo == 0)
        {
            currentAmmo = magSize;
        }
    }
    private void Update()
    {
        if (gameObject.activeSelf && ammoText != null) { ammoText.text = currentAmmo.ToString() + "/" + totalAmmo.ToString(); }

        //Cooldowns
        if (currentFireDelay > 0) { currentFireDelay -= Time.deltaTime; }
        else { currentFireDelay = 0; if (animator != null) { animator.SetBool("Fire", false); } }
    }
    public virtual bool Fire()
    {
        //true
        if(currentAmmo > 0 && currentFireDelay == 0 && reloading == false)
        {
            currentAmmo--;
            currentFireDelay = firedelay;
            if (animator != null) { animator.SetBool("Fire", true); }
            if (currentAmmo == 0) { Reload(); }
            return true;
        }
        //false
        if (currentAmmo == 0) { Reload(); }
        return false;
    }
    public virtual bool Reload()
    {
        //Reload
        if (currentAmmo == magSize || totalAmmo == 0 || reloading == true)
        {
            Debug.Log("cant reload");
            return false;
        }

        StartCoroutine(ReloadRoutine());
        if (animator != null) { animator.SetBool("Reload", true); }
        return true;
    }
    public IEnumerator ReloadRoutine()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        int ammoToAdd = magSize - currentAmmo;

        if (totalAmmo >= ammoToAdd)
        {
            currentAmmo = magSize;
            totalAmmo -= ammoToAdd;
        }
        else
        {
            currentAmmo += currentAmmo;
            currentAmmo = 0;
        }

        reloading = false;
        StopCoroutine(ReloadRoutine());
    }
}