using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] string actionMapName = "Player";
    [SerializeField] string uiMapName = "UI";
    [SerializeField] Transform playerTransfom;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float lookSpeed = 30f;
    [SerializeField] Animator playerAnimator;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] CapsuleCollider playerCollider;
    [SerializeField] PlayerDimensions standingDimensions;
    [SerializeField] PlayerDimensions crouchedDimensions;
    [SerializeField] WeaponSelectionUI weaponSelectionUI;
    [SerializeField] Attacker attacker;

    private WeaponController weaponController;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 lookDirection = Vector3.zero;

    private float groundedCheckDistance = 0.2f;
    private float groundedCheckOffset = 0.1f;

    private bool crouched = false;

    [Serializable]
    public struct PlayerDimensions
    {
        public float Height;
        public float Radius;
        public Vector3 Center;
    }

    private void Start()
    {
        playerInput.onActionTriggered += HandleActionTriggered;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        weaponController = playerTransfom.GetComponent<WeaponController>();

        weaponSelectionUI.UIStateChanged.AddListener((isOpen) =>
        {
            if(isOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                playerInput.SwitchCurrentActionMap(uiMapName);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                playerInput.SwitchCurrentActionMap(actionMapName);
            }
        });
    }

    void OnDestroy()
    {
        playerInput.onActionTriggered -= HandleActionTriggered;
    }

    private void Update()
    {
        playerTransfom.Rotate(lookDirection * lookSpeed * Time.deltaTime, Space.Self);
    }

    private void FixedUpdate()
    {
        playerTransfom.Translate(moveDirection * moveSpeed * Time.fixedDeltaTime, Space.Self);

        // Update Animator parameters based on movement
        playerAnimator.SetFloat("Forward", moveDirection.z);
        playerAnimator.SetFloat("Strafe", moveDirection.x);
        playerAnimator.SetBool("Grounded", IsGrounded());
    }

    private bool IsGrounded()
    {
        if(Physics.Raycast(playerTransfom.position + Vector3.up * groundedCheckOffset, Vector3.down, out RaycastHit hit, groundedCheckDistance))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jump()
    {
        Debug.Log("Jump Triggered");
        if(!IsGrounded())
        {
            return;
        }
        var rigidbody = playerTransfom.GetComponent<Rigidbody>();
        rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        playerAnimator.SetTrigger("Jump");
        playerAnimator.SetBool("Grounded", false);
    }

    void Crouch()
    {
        crouched = true;
        playerAnimator.SetBool("Crouched", true);

        playerCollider.height = crouchedDimensions.Height;
        playerCollider.radius = crouchedDimensions.Radius;
        playerCollider.center = crouchedDimensions.Center;
    }

    void StandUp()
    {
        crouched = false;
        playerAnimator.SetBool("Crouched", false);
        playerCollider.height = standingDimensions.Height;
        playerCollider.radius = standingDimensions.Radius;
        playerCollider.center = standingDimensions.Center;
    }

    private void HandleActionTriggered(InputAction.CallbackContext context)
    {
        if(context.action.actionMap.name == actionMapName)
        {
            switch (context.action.name)
            {
                case "Move":
                    // Move Player
                    Vector2 moveInput = context.ReadValue<Vector2>();
                    moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
                    break;
                case "Look":
                    // Look Around
                    Vector2 lookInput = context.ReadValue<Vector2>();
                    lookDirection = new Vector3(0f, lookInput.x, 0f);
                    break;
                case "Jump":
                    if (context.performed)
                    {
                        // Trigger Jump
                        Jump();
                    }
                    break;
                case "Crouch":
                    if (context.phase == InputActionPhase.Performed)
                    {
                        Crouch();
                    }
                    else if (context.phase == InputActionPhase.Canceled)
                    {
                        StandUp();
                    }
                    break;

                case "NextWeapon":
                    if (context.phase == InputActionPhase.Performed)
                    {
                        weaponController.NextWeapon();
                    }
                    break;
                case "PreviousWeapon":
                    if (context.phase == InputActionPhase.Performed)
                    {
                        weaponController.PreviousWeapon();
                    }
                    break;
                case "OpenWeaponsMenu":
                    if (context.phase == InputActionPhase.Performed)
                    {
                        if (weaponSelectionUI.IsOpen)
                        {
                            weaponSelectionUI.Close();
                            Cursor.lockState = CursorLockMode.Locked;
                            Cursor.visible = false;
                        }
                        else
                        {
                            weaponSelectionUI.Open();
                            Cursor.lockState = CursorLockMode.None;
                            Cursor.visible = true;
                        }
                    }
                    break;
                case "Attack":
                    if (context.phase == InputActionPhase.Performed)
                    {
                        Debug.Log("Attack Triggered");
                        var attackCommand = attacker.CreateAttackCommand();
                        CommandController.Instance.ExecuteCommand(attackCommand);
                    }
                    break;
                default:
                    break;
            }
        }
        else if (context.action.actionMap.name == uiMapName)
        {
            switch (context.action.name)
            {
                case "Cancel":
                    if (context.phase == InputActionPhase.Performed)
                    {
                        if (weaponSelectionUI.IsOpen)
                        {
                            weaponSelectionUI.Close();
                            Cursor.lockState = CursorLockMode.Locked;
                            Cursor.visible = false;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
