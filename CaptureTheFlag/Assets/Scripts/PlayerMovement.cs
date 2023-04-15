using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float speed = 5f;
    //[SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed;
    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 movementInput;
    //private Vector3 movementDirection;
    static PlayerMovement instance;
    Animator anim;

    private int isFallingdCounter = 0; // it is always alternating between true and false, if there is 3 in a row we can conclude its falling.
    private Vector3 lastSeenPos;
    

    public static PlayerMovement Instance { get {
            if (instance == null) instance = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
            return instance;
        } }

    private CaptureTheFlag controls;

    private void Awake() {
        controller = GetComponent<CharacterController>();

        // Initialize the new input system controls
        controls = new CaptureTheFlag();
        controls.Player.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Player.Movement.canceled += _ => movementInput = Vector2.zero;
        //controls.Player.Jump.performed += _ => Jump();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start() {
       
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }

    private void Update() {
        if (controller.isGrounded)
        {
            isFallingdCounter = 0;
            lastSeenPos = transform.position;
        }
        else
        {
            isFallingdCounter++;
        }

        if (isFallingdCounter >= 5)
        {
            transform.position = lastSeenPos;
            isFallingdCounter = 0;
        }
        Move();
    }

    private void Move() {
        // Get the camera's forward vector and flatten it on the XZ plane
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f;
        cameraForward = cameraForward.normalized;

        // Calculate the movement direction based on player input and camera direction
        Vector3 moveDirection = (cameraForward * movementInput.y + Camera.main.transform.right * movementInput.x).normalized;

        // Rotate the character towards the movement direction
        if (moveDirection.magnitude > 0.1f) {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            anim.SetFloat("Speed", 2);
        }
        else {
            anim.SetFloat("Speed", 0);
        }

        // Move the character
        Vector3 movement = moveDirection * speed * Time.deltaTime;

        // Apply gravity
        if (controller.isGrounded) {
            velocity = Vector3.zero;
            controller.Move(movement);
        }
        else {
            velocity.y = gravity * Time.deltaTime * 0.3f;
            controller.Move(movement + velocity);
        }

    }

    /*private void Jump() {
        if (controller.isGrounded) {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }*/
    

    public void EnableControls(bool enable)
    {
        (enable ? (Action)controls.Enable : (Action)controls.Disable)();
    }
    
}