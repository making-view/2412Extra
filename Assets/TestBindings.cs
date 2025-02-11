using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TestBindings : MonoBehaviour
{
    [Serializable]
    public class TestBinding
    {
        public string name;
        public UnityEvent unityEvent;
        public InputAction input;
    }

    public List<TestBinding> _bindings;


    private void OnEnable()
    {
        if (!Application.isEditor)
            return;

        foreach (var binding in _bindings)
            binding.input.Enable();
    }

    private void OnDisable()
    {
        foreach (var binding in _bindings)
            binding.input.Disable();
    }

    private void Update()
    {
        foreach (var binding in _bindings)
        {
            if (binding.input.WasPressedThisFrame())
            {
                Debug.Log(this + " pressed: " + binding.input.name + "\nStarting " + binding.name);
                binding.unityEvent.Invoke();
            }    
        }
    }
}
