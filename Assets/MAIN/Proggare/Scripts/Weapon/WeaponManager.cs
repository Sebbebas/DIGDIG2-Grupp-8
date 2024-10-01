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

        //Enable the action's
        scrollAction.Enable();
        fireAction.Enable();
        reloadAction.Enable();
        throwAction.Enable();

        //Subscribe to the performed callback 
        scrollAction.performed += OnSwitchWeapon;
        fireAction.performed += OnFire;
        reloadAction.performed += OnReload;
        throwAction.performed += OnThrow;
    }
    private void OnDisable()
    {
        //Unsubscribe from the input when the object is disabled
        scrollAction.performed -= OnSwitchWeapon;
        fireAction.performed -= OnFire;
        reloadAction.performed -= OnReload;
        throwAction.performed -= OnThrow;
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
}
