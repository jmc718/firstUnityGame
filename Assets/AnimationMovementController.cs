using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationMovementController : MonoBehaviour
{
    // declare reference variables
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    // variables to store optimized setter/getter parameter IDs
    int isWalkingHash;
    int isRunningHash;

    // variables to store player input values
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isMovementPressed;
    bool isRunPressed;
    float rotationFactorPerFrame = 15f;
    float runMultiplier = 6.0f;
    float walkMultiplier = 2.0f;

    Camera m_camera;

    // Camera camera;

    // Awake is called earlier than Start in Unity's event life cycle
    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

        // Set the player input callbacks
        playerInput.CharacterControls.Move.started += onMovementInput;
        // for when you stop pressing the button
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        // continues to update the changes in the players input, mostly for controller
        playerInput.CharacterControls.Move.performed += onMovementInput;

        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;
    }

    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void handleRotation()
    {
        Vector3 positionToLookAt;
        // the change in position our character should point to
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;
        // the current rotation of our character
        Quaternion currentRotation = transform.rotation;

        // creates a new rotation based on where the player is currently pressing
        

        if (isMovementPressed)
        {
            // creates a new rotation based on where the player is currently pressing
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);

        }

    }


/*
Function: onMovementInput
Description: Handles the direction the character will be moving when 
movement input is entered. 

*/


    void onMovementInput (InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x * walkMultiplier;
        currentMovement.z = currentMovementInput.y * walkMultiplier;
        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;

        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }



    // void onMovementInput (InputAction.CallbackContext context)
    // {
    //     m_camera = Camera.main;
    //     currentMovementInput = context.ReadValue<Vector2>();
    //     currentMovement = m_camera.transform.forward * currentMovementInput.y +
    //                        m_camera.transform.right * currentMovementInput.x;
    //     currentMovement.y = 0;
    //     currentMovement = currentMovement.normalized * walkMultiplier;
        
    //     currentRunMovement = m_camera.transform.forward * currentMovementInput.y +
    //                        m_camera.transform.right * currentMovementInput.x;
    //     currentRunMovement.y = 0;
    //     currentRunMovement = currentRunMovement.normalized * runMultiplier;

    //     isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    // }

    void handleAnimation()
    {
        // get parameter values from animator
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        // start walking if movement pressed is true and not already walking
        if (isMovementPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }

        // stop walking if isMovementPressed is false and already walking
        if (!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }

        // stop running if movement or run pressed are false and currently running
        if ((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }

        // stop running if movement or run pressed are false and currently running
        else if ((!isMovementPressed || !isRunPressed) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }



    }


    void handleGravity()
    {
        if (characterController.isGrounded)
        {
            float groundedGravity = -.05f;
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }

        else
        {
            float gravity = -9.8f;
            currentMovement.y += gravity;
            currentRunMovement.y += gravity;
        }


    }

    // Update is called once per frame
    void Update()
    {
        handleGravity();
        handleAnimation();
        handleRotation();

        if (isRunPressed) {
            characterController.Move(currentRunMovement * Time.deltaTime);
        } else {
            characterController.Move(currentMovement * Time.deltaTime);
        }
        
    }

    void OnEnable()
    {
        // enable the character controler action map
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        // disable the character controler action map
        playerInput.CharacterControls.Disable();
    }

}
