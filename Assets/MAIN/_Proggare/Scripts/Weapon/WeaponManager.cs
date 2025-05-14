using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

//Sebbe

public class WeaponManager : MonoBehaviour
{
    //Configurable Parameters
    [Header("InputActionMap")]
    [SerializeField, Tooltip("The PlayerInput actionmap")] InputActionAsset inputActions;

    [Header("Weapon")]
    [SerializeField, Tooltip("Current weapon gameobject")] GameObject currentWeapon;
    [SerializeField, Tooltip("A list of all the weapon gameobjects")] List<GameObject> WeaponsList = new List<GameObject>();

    ///////////////////////////////////////////////////////////////////////////WIP
    [Header("Kick")]
    [SerializeField] LayerMask kickLayerMask;
    [SerializeField] Animator legAnimator;
    [SerializeField] GameObject enemyHitEffect;

    [Space]

    [SerializeField] float kickDamage = 150f;

    [Space]

    [SerializeField] float kickCooldown = 0.5f;
    [SerializeField] float kickForce = 2.4f;
    [SerializeField] float maxKickDistance = 3f; //Max distance of the detection
    [SerializeField] float coneRadius = 1.5f; //Radius of the sphere cast
    [SerializeField] float coneAngle = 20f; //Angle of the cone
    [SerializeField] int coneResolution = 5;

    [Space]

    [SerializeField] AudioClip kickSound;
    [SerializeField, Range(0, 1)] float kickSoundVolume = 1f;
    [SerializeField, Range(0, 256)] int kickSoundPriority = 256;
    [SerializeField] Vector2 kickSoundPitch;
    ////////////////////////////////////////////////////////////////////

    [Header("Screen Shake")]
    [SerializeField] float screenShakeDuration = 0.1f;
    [SerializeField] float screenShakeIntensity = 0.1f;

    [Header("Grenade")]
    [SerializeField, Tooltip("Grenade Prefab")] GameObject grenadePrefab;
    [SerializeField, Tooltip("Grenade spawn offset")] Vector3 spawnOffset;
    [SerializeField, Tooltip("Grenade throw cooldown")] float throwCooldownTime = 3f;

    [Space]

    [SerializeField] AudioClip grenadeThrowSound;
    [SerializeField, Range(0, 1)] float grenadeThrowSoundVolume = 1f;
    [SerializeField, Range(0, 256)] int grenadeThrowSoundPriority = 256;

    //Private Variabels
    private GameObject antiHierarchySpam;
    private int currentWeaponInt = 0;
    private int totalWeapons;

    //Throwing
    private float currentThrowCooldownTime;
    private bool throwCoolodown = false;

    //Kick
    private Vector3 kickOrigin;
    private Vector3 forward;
    private float currentKickCooldownTime;
    private bool kickCooldownActive = false;

    //Switching
    private float currentSwitchTime;
    private float switchCooldown;
    private bool isSwitching = false;

    //Cached References
    InputAction scrollAction;
    InputAction fireAction;
    InputAction reloadAction;
    InputAction throwAction;
    InputAction kickAction;
    Transform weaponsParent;
    ScreenShake screenShake;

    #region Base Methods
    void Start()
    {
        //Get Cached References
        weaponsParent = GetComponent<Transform>();
        screenShake = FindFirstObjectByType<ScreenShake>();

        if (currentWeapon != null)
        {
            switchCooldown = currentWeapon.GetComponent<Weapon>().GetSwitchDelay();
            currentSwitchTime = switchCooldown;
        }

        AntiHierarchySpam();

        //Check if there are any weapons in the list
        if (WeaponsList.Count > 0) { WeaponsList.Clear(); }

        //Find all child objects under the "weaponsParent" and add them to the list
        foreach (Transform weapon in weaponsParent)
        {
            WeaponsList.Add(weapon.gameObject);
        }

        //Get the current weapon index
        totalWeapons = WeaponsList.Count;
        currentWeapon = WeaponsList[currentWeaponInt];

        //Set currentWeapon to active
        foreach (Transform weapon in weaponsParent)
        {
            weapon.gameObject.GetComponent<Weapon>().StopAllWeaponCorutines();

            if (weapon.gameObject != currentWeapon) { weapon.gameObject.SetActive(false); }
            else { weapon.gameObject.SetActive(true); }
        }
    }
    private void FixedUpdate()
    {
        kickOrigin = transform.position;
        forward = transform.forward;

        if (throwCoolodown && currentThrowCooldownTime > 0) { currentThrowCooldownTime -= Time.deltaTime; }
        else { currentThrowCooldownTime = throwCooldownTime; throwCoolodown = false; }

        if (kickCooldownActive && currentKickCooldownTime > 0) { currentKickCooldownTime -= Time.deltaTime; }
        else { currentKickCooldownTime = kickCooldown; kickCooldownActive = false; }

        if (currentSwitchTime > 0) { currentSwitchTime -= Time.deltaTime; }
        else { currentSwitchTime = switchCooldown; isSwitching = false; }
    }
    private void OnEnable()
    {
        //Get player ActionMap
        var actionMap = inputActions.FindActionMap("Player");

        //Get actions from player ActionMap
        scrollAction = actionMap.FindAction("SwitchWeapon");
        fireAction = actionMap.FindAction("Fire");
        reloadAction = actionMap.FindAction("Reload");
        throwAction = actionMap.FindAction("Throw");
        kickAction = actionMap.FindAction("Kick");

        //Enable the action's
        scrollAction.Enable();
        fireAction.Enable();
        reloadAction.Enable();
        throwAction.Enable();
        kickAction.Enable();

        //Subscribe to the performed callback 
        scrollAction.performed += OnSwitchWeapon;
        fireAction.performed += OnFire;
        reloadAction.performed += OnReload;
        throwAction.performed += OnThrow;
        kickAction.performed += OnKick;
    }
    private void OnDisable()
    {
        StopAllCoroutines();

        //Unsubscribe from the input when the object is disabled
        scrollAction.performed -= OnSwitchWeapon;
        fireAction.performed -= OnFire;
        reloadAction.performed -= OnReload;
        throwAction.performed -= OnThrow;
        kickAction.performed -= OnKick;
    }
    #endregion

    #region Inputs 
    void OnFire(InputAction.CallbackContext context)
    {
        if (currentWeapon != null)
        {
            Weapon weaponComponent = currentWeapon.GetComponent<Weapon>();

            if (weaponComponent != null && !weaponComponent.GetIsHeldToFire())
            {
                weaponComponent.Fire();
            }
            else if (weaponComponent != null && weaponComponent.GetIsHeldToFire())
            {
                StartCoroutine(FireHeldToFire(weaponComponent, context));
            }
        }
    }

    IEnumerator FireHeldToFire(Weapon weaponComponent, InputAction.CallbackContext context)
    {
        while (true)
        {
            if (!context.performed)
            {
                weaponComponent.SetButtonPressed(false);
                StopCoroutine(FireHeldToFire(weaponComponent, context));
            }
            else
            {
                weaponComponent.SetButtonPressed(true);
                weaponComponent.Fire();
            }

            yield return new WaitForSeconds(weaponComponent.GetFireRate());
        }
    }

    void OnReload(InputAction.CallbackContext context)
    {
        if (currentWeapon != null)
        {
            Weapon weaponComponent = currentWeapon.GetComponent<Weapon>();

            if (weaponComponent != null)
            {
                weaponComponent.Reload();
            }
        }
    }
    #endregion
    public void OnKick(InputAction.CallbackContext context)
    {
        if (kickCooldownActive) { return; }

        //Kick Cooldown
        kickCooldownActive = true;

        //Animation
        if (legAnimator != null) 
        { 
            legAnimator.SetTrigger("kick");
        }

        //List to hold kicked objects
        List<GameObject> kickedObjects = new List<GameObject>();

        //Iterate over the cone resolution
        for (int i = 0; i <= coneResolution; i++)
        {
            //Angle spread between the rays
            float currentAngle = Mathf.Lerp(-coneAngle, coneAngle, i / (float)coneResolution);

            //Calculate the direction of the cone's side ray
            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 direction = rotation * transform.forward;

            //Create the ray
            Ray kickRay = new Ray(transform.position, direction);
            RaycastHit hit;

            //Perform raycast and check if we hit something in the kick layer mask
            if (Physics.Raycast(kickRay, out hit, maxKickDistance, kickLayerMask))
            {
                //Add the hit object to the list
                kickedObjects.Add(hit.transform.gameObject);

                //Gameobject Logic
                if (hit.transform.GetComponent<EnemyScript>() != null) 
                { 
                    hit.transform.GetComponent<EnemyScript>().Kicked(direction * kickForce);
                    hit.transform.GetComponent<EnemyScript>().ApplyDamage(kickDamage);

                    Instantiate(enemyHitEffect, hit.transform.position, Quaternion.identity);
                    PlayKickSound();
                }
                if (hit.transform.GetComponent<Plank>() != null)
                {
                    hit.transform.GetComponent<Plank>().BreakPlanks(direction * kickForce, 0, 5);
                    screenShake.Shake(screenShakeDuration, screenShakeIntensity);
                }
            }
        }
        //foreach (var obj in kickedObjects)
        //{
        //    Debug.Log("Kicked Object: " + obj.name);
        //}
    }

    void PlayKickSound()
    {
        //Audio
        GameObject kickSoundObject = new();
        kickSoundObject.AddComponent<AudioSource>();
        kickSoundObject.GetComponent<AudioSource>().clip = kickSound;
        kickSoundObject.GetComponent<AudioSource>().playOnAwake = true;
        kickSoundObject.GetComponent<AudioSource>().volume = kickSoundVolume;
        kickSoundObject.GetComponent<AudioSource>().priority = kickSoundPriority;
        kickSoundObject.GetComponent<AudioSource>().pitch = Random.Range(kickSoundPitch.x, kickSoundPitch.y);
        Instantiate(kickSoundObject, transform.position, Quaternion.identity);
    }

    #region Grenade
    void OnThrow(InputAction.CallbackContext context)
    {
        //Call throw method
        ThrowGrenade();
    }
    public void ThrowGrenade()
    {
        //cooldown?
        if (throwCoolodown) { return; }

        //start a new cooldown
        throwCoolodown = true;

        //Audio
        GameObject grenadeThrowSoundObject = new();
        grenadeThrowSoundObject.AddComponent<AudioSource>();
        grenadeThrowSoundObject.GetComponent<AudioSource>().clip = grenadeThrowSound;
        grenadeThrowSoundObject.GetComponent<AudioSource>().playOnAwake = true;
        grenadeThrowSoundObject.GetComponent<AudioSource>().volume = grenadeThrowSoundVolume;
        grenadeThrowSoundObject.GetComponent<AudioSource>().priority = grenadeThrowSoundPriority;
        Instantiate(grenadeThrowSoundObject, transform.position, Quaternion.identity);

        //antiHierarchySpam
        antiHierarchySpam = GameObject.FindGameObjectWithTag("antiHierarchySpam");

        //Get spawn location
        Vector3 spawnPos = GetComponentInParent<Transform>().position + spawnOffset;

        //Instansiate object
        if (antiHierarchySpam != null && grenadePrefab != null)
        {
            Instantiate(grenadePrefab, spawnPos, GetComponentInParent<Transform>().rotation, antiHierarchySpam.transform);
        }
    }
    #endregion

    #region Switch Weapon
    void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        if (currentSwitchTime > 0) { return; }

        //Get the scroll value (Vector2, using y-axis for scrolling)
        Vector2 scrollValue = context.ReadValue<Vector2>();

        //Switch weapon based on scroll direction
        if (scrollValue.y < 0)
        {
            //Scrolling up (next weapon)
            currentWeaponInt = (currentWeaponInt + 1) % totalWeapons;
            currentWeapon = WeaponsList[currentWeaponInt];
        }
        else if (scrollValue.y > 0)
        {
            //Scrolling down (previous weapon)
            currentWeaponInt = (currentWeaponInt - 1 + totalWeapons) % totalWeapons;
            currentWeapon = WeaponsList[currentWeaponInt];
        }

        StartCoroutine(UpdateWeaponList());
    }
    IEnumerator UpdateWeaponList()
    {
        //Check if there are any weapons in the list
        if (WeaponsList.Count > 0) { WeaponsList.Clear(); }

        //Find all child objects under the "weaponsParent" and add them to the list
        foreach (Transform weapon in weaponsParent)
        {
            WeaponsList.Add(weapon.gameObject);
        }

        //Change Values based on list
        isSwitching = true;

        //Get the current weapon index
        totalWeapons = WeaponsList.Count;
        currentWeapon = WeaponsList[currentWeaponInt];

        //Get the switch delay from the current weapon
        currentSwitchTime = currentWeapon.GetComponent<Weapon>().GetSwitchDelay();

        //Play Switch Animation
        StartCoroutine(currentWeapon.GetComponent<Weapon>().EffectsCoroutine(EffectType.Switch));

        //Wait before switching
        yield return new WaitForSeconds(currentSwitchTime);

        //Set currentWeapon to active
        foreach (Transform weapon in weaponsParent)
        {
            weapon.gameObject.GetComponent<Weapon>().StopAllWeaponCorutines();

            if (weapon.gameObject != currentWeapon) { weapon.gameObject.SetActive(false); }
            else { weapon.gameObject.SetActive(true); }
        }

        isSwitching = false;

        StopCoroutine(UpdateWeaponList());
    }
    #endregion

    #region AntiHierarchySpam
    /// <summary>
    /// Instansiates AntiHierarchySpam
    /// </summary>
    public void AntiHierarchySpam()
    {
        GameObject antiSpam = new("AntiHierarchySpam");
        antiSpam.tag = "antiHierarchySpam";

        antiHierarchySpam = antiSpam;
    }
    #endregion

    private void OnDrawGizmos()
    {
        kickOrigin = transform.position;
        forward = transform.forward;

        Gizmos.color = Color.yellow;

        for (int i = 0; i <= coneResolution; i++)
        {
            float currentAngle = Mathf.Lerp(-coneAngle, coneAngle, i / (float)coneResolution);
            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 direction = rotation * forward;

            // Draw lines extending to maxKickDistance
            Gizmos.DrawLine(kickOrigin, kickOrigin + direction * maxKickDistance);
        }
    }

    public bool GetIsSwitching()
    {
        return isSwitching;
    }
    public GameObject GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
