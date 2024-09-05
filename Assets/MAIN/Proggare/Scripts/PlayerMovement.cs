using UnityEngine;
using UnityEngine.InputSystem;

//Elian

public class PlayerMovement : MonoBehaviour
{
    //variables
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float acceleration = 1f;
    [SerializeField] float jumpForce = 5f;

    CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        
    }

    void OnJump()
    {

    }
}
