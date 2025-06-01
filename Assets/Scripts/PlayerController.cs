using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] string actionMapName = "Player";
    [SerializeField] Transform playerTransfom;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float lookSpeed = 30f;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 lookDirection = Vector3.zero;


    private void Start()
    {
        playerInput.onActionTriggered += HandleActionTriggered;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        playerTransfom.Rotate(lookDirection * lookSpeed * Time.deltaTime, Space.Self);
        playerTransfom.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);
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
                default:
                    break;
            }
        }
    }



















    //[SerializeField] PlayerInput playerInput;
    //[SerializeField] Transform playerTransform;

    //[SerializeField] float moveSpeed = 1f;
    //[SerializeField] float lookSpeed = 30f;

    //string playerActionMap = "Player";

    //Vector3 moveDirection = Vector3.zero;
    //Vector3 lookDirection = Vector3.zero;

    //void Start()
    //{
    //    playerInput.onActionTriggered += HandleActionTriggered;
    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;
    //}

    //private void HandleActionTriggered(InputAction.CallbackContext context)
    //{

    //    if(context.action.actionMap.name == playerActionMap)
    //    {
    //        switch (context.action.name)
    //        {
    //            case "Move":
    //                Vector2 moveInput = context.ReadValue<Vector2>();
    //                Debug.Log($"Move Input: {moveInput}");
    //                // Handle movement logic here
    //                moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
    //                break;
    //            case "Look":
    //                Vector2 lookInput = context.ReadValue<Vector2>();
    //                Debug.Log($"Look Input: {lookInput}");
    //                // Handle looking logic here
    //                float lookX = lookInput.x * lookSpeed * Time.deltaTime;
    //                lookDirection = new Vector3(0, lookX, 0);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //}

    //void Update()
    //{
    //    playerTransform.Rotate(lookDirection, Space.World);
    //    playerTransform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);
    //}
}
