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

    [Header("Kick")]
    [SerializeField] float kickCooldown;
    [SerializeField] float coneAngle = 30f; // Angle of the cone
    [SerializeField] float maxDistance = 10f; // Max distance of the detection
    [SerializeField] float radius = 2f; // Radius of the sphere cast
    [SerializeField] int coneResolution = 20;
    [SerializeField] LayerMask layerMask; // Layer mask for collision detection

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
    public void OnKick(InputAction.CallbackContext context)
    {
        //Perform a sphere cast to detect objects within the sphere's radius
        RaycastHit[] hits = Physics.SphereCastAll(kickOrigin, radius, forward, maxDistance, layerMask);
        
        //Check if the objects are within the cone angle
        foreach (RaycastHit hit in hits)
        {
            //Get direction from the origin to the hit point
            Vector3 directionToHit = (hit.point - kickOrigin);

            //Calculate the angle between the player's forward direction and the hit point
            float angleToHit = Vector3.Angle(forward, directionToHit);

            //Only proceed if the hit object is within the defined cone angle
            if (angleToHit <= coneAngle)
            {
                Debug.DrawLine(kickOrigin, hit.point, Color.red, 1.0f);
                Debug.Log("Hit within cone: " + hit.collider.name);
                Instantiate(currentWeapon, hit.point, Quaternion.identity, antiHierarchySpam.transform);
            }
        }
    }
    #endregion

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

        //Kick radius something kick something idk pls heölp im losing my mind
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(kickOrigin, radius);
    }
}
