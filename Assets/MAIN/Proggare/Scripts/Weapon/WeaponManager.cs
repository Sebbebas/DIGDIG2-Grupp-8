using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

//Sebbe

public class WeaponManager : MonoBehaviour
{
    //Configurable Parameters
    [Header("InputActionMap")]
    [SerializeField, Tooltip("The PlayerInput actionmap")] InputActionAsset inputActions;

    [Header("Weapon")]
    [SerializeField, Tooltip("Current weapon gameobject")] GameObject currentWeapon;
    [SerializeField, Tooltip("A list of all the weapon gameobjects")] List<GameObject> WeaponsList = new List<GameObject>();

    //WIP
    [Header("Kick")]
    [SerializeField] float kickCooldown;
    [SerializeField] float coneAngle = 30f; // Angle of the cone
    [SerializeField] float maxKickDistance = 10f; // Max distance of the detection
    [SerializeField] float kickForce = 10f;
    [SerializeField] float radius = 2f; // Radius of the sphere cast
    [SerializeField] int coneResolution = 20;
    [SerializeField] LayerMask kickLayerMask; // Layer mask for collision detection

    private Vector3 kickOrigin;
    private Vector3 forward;

    [Header("Grenade")]
    [SerializeField, Tooltip("Grenade Prefab")] GameObject grenadePrefab;
    [SerializeField, Tooltip("Grenade spawn offset")] Vector3 spawnOffset;
    [SerializeField, Tooltip("Grenade throw cooldown")] float throwCooldownTime = 3f;

    //Private Variabels
    private GameObject antiHierarchySpam;
    private float currentThrowCooldownTime;
    private int currentWeaponInt = 0;
    private int totalWeapons;
    private bool throwCoolodown = false;

    //Cached References
    InputAction scrollAction;
    InputAction fireAction;
    InputAction reloadAction;
    InputAction throwAction;
    InputAction kickAction;
    Transform weaponsParent;

    #region Base Methods
    void Start()
    {
        //Get Cached References
        weaponsParent = GetComponent<Transform>();

        AntiHierarchySpam();
        UpdateWeaponList();
    }
    private void FixedUpdate()
    {
        kickOrigin = transform.position;
        forward = transform.forward;

        if (throwCoolodown && currentThrowCooldownTime > 0) { currentThrowCooldownTime -= Time.deltaTime; }
        else { currentThrowCooldownTime = throwCooldownTime; throwCoolodown = false; }
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
            if (weaponComponent != null)
            {
                weaponComponent.Fire();
            }
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
        //Animation


        // List to hold kicked objects
        List<GameObject> kickedObjects = new List<GameObject>();

        // Iterate over the cone resolution
        for (int i = 0; i <= coneResolution; i++)
        {
            // Angle spread between the rays
            float currentAngle = Mathf.Lerp(-coneAngle, coneAngle, i / (float)coneResolution);

            // Calculate the direction of the cone's side ray
            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 direction = rotation * transform.forward;

            // Create the ray
            Ray kickRay = new Ray(transform.position, direction);
            RaycastHit hit;

            // Perform raycast and check if we hit something in the kick layer mask
            if (Physics.Raycast(kickRay, out hit, maxKickDistance, kickLayerMask))
            {
                // Add the hit object to the list
                kickedObjects.Add(hit.transform.gameObject);

                //Apply force or logic to the kicked object
                //hit.rigidbody?.AddForce(direction * kickForce, ForceMode.Impulse);
                if (hit.transform.GetComponent<EnemyScript>() != null) { hit.transform.GetComponent<EnemyScript>().Kicked(direction * kickForce); }
            }
        }
        foreach (var obj in kickedObjects)
        {
            Debug.Log("Kicked Object: " + obj.name);
        }
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
        //Get the scroll value (Vector2, using y-axis for scrolling)
        Vector2 scrollValue = context.ReadValue<Vector2>();

        //Switch weapon based on scroll direction
        if (scrollValue.y > 0)
        {
            //Scrolling up (next weapon)
            currentWeaponInt = (currentWeaponInt + 1) % totalWeapons;
            currentWeapon = WeaponsList[currentWeaponInt];
        }
        else if (scrollValue.y < 0)
        {
            //Scrolling down (previous weapon)
            currentWeaponInt = (currentWeaponInt - 1 + totalWeapons) % totalWeapons;
            currentWeapon = WeaponsList[currentWeaponInt];
        }

        UpdateWeaponList();
    }
    void UpdateWeaponList()
    {
        if(WeaponsList.Count > 0) { WeaponsList.Clear(); }

        //Find all child objects under the "weaponsParent" and add them to the list
        foreach (Transform weapon in weaponsParent)
        {
            WeaponsList.Add(weapon.gameObject);
        }

        //Change Values based on list
        totalWeapons = WeaponsList.Count;
        currentWeapon = WeaponsList[currentWeaponInt];

        //Set currentWeapon to active
        foreach (Transform weapon in weaponsParent)
        {
            if (weapon.gameObject != currentWeapon) { weapon.gameObject.SetActive(false); }
            else { weapon.gameObject.SetActive(true); }
        }
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

    private void OnDrawGizmosSelected()
    {
        //Kick Cone
        Gizmos.color = Color.yellow;

        //Render 1 line for every "coneResolution"
        for (int i = 0; i <= coneResolution; i++)
        {
            //Angle spread between the rays
            float currentAngle = Mathf.Lerp(-coneAngle, coneAngle, i / (float)coneResolution);

            //Calculate the direction of the cone's side ray
            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 direction = rotation * transform.forward;

            //Draw the ray
            Gizmos.DrawLine(kickOrigin, kickOrigin + direction * radius);
        }
    }

    public GameObject GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
