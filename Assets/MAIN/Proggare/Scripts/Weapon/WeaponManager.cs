using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    //Configurable Parameters
    [Header("InputActionMap")]
    [SerializeField] InputActionAsset inputActions;

    [Header("Weapon")]
    [SerializeField] GameObject currentWeapon;
    [SerializeField] List<GameObject> WeaponsList = new List<GameObject>();

    //Private Variables
    private int currentWeaponInt = 0;
    private int totalWeapons;

    //Cached References
    InputAction scrollAction;
    Transform weaponsParent;

    void Start()
    {
        //Get Cached References
        weaponsParent = GetComponent<Transform>();

        UpdateWeaponList();
    }
    private void OnEnable()
    {
        //Get the action map and the action
        var actionMap = inputActions.FindActionMap("Player");
        scrollAction = actionMap.FindAction("SwitchWeapon");

        //Enable the action
        scrollAction.Enable();

        //Subscribe to the performed callback
        scrollAction.performed += OnSwitchWeapon;
    }

    private void OnDisable()
    {
        //Unsubscribe from the input when the object is disabled
        scrollAction.performed -= OnSwitchWeapon;
    }

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
}
