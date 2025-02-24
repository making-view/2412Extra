using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Autohand;
using Unity.XR.PXR;

public class SinglebuttonOpenXRTeleporterLink : MonoBehaviour
{
    public Teleporter hand;
    public InputActionProperty teleportAction;

    private bool _teleporting = false;
    Watergun _waterGun = null;

    private void Start()
    {
        hand = GetComponent<Teleporter>();
        _waterGun = PlayerManager.instance.GetComponentInChildren<Watergun>();
    }

    private void Update()
    {
        if(_teleporting)
        {
            foreach (Autohand.Hand hand in _waterGun._grabbingHands)
            {
                hand.PlayHapticVibration(0.1f, 0.1f);
            }
        }
    }

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
        if (!_teleporting)
        {
            hand.StartTeleport();
            _teleporting = true;
        }
    }

    void FinishTeleportAction(InputAction.CallbackContext a)
    {
        if (_teleporting)
        {
            hand.Teleport();
            _teleporting = false;

            foreach (Autohand.Hand hand in _waterGun._grabbingHands)
            {
                hand.PlayHapticVibration(0.05f, 0.6f);
            }
        }
    }
}
