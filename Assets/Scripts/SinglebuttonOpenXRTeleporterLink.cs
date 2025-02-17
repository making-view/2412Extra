using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Autohand;

public class SinglebuttonOpenXRTeleporterLink : MonoBehaviour
{
    public Teleporter hand;
    public InputActionProperty teleportAction;

    bool teleporting = false;

    void OnEnable()
    {
        if (teleportAction.action != null)
        {
            teleportAction.action.Enable();
            teleportAction.action.performed += StartTeleportAction;
            teleportAction.action.canceled += FinishTeleportAction;
        }
    }

    void OnDisable()
    {
        if (teleportAction.action != null)
        {
            teleportAction.action.performed -= StartTeleportAction;
            teleportAction.action.canceled -= FinishTeleportAction;
        }
    }


    void StartTeleportAction(InputAction.CallbackContext a)
    {
        if (!teleporting)
        {
            hand.StartTeleport();
            teleporting = true;
        }
    }

    void FinishTeleportAction(InputAction.CallbackContext a)
    {
        if (teleporting)
        {
            hand.Teleport();
            teleporting = false;
        }
    }




}
