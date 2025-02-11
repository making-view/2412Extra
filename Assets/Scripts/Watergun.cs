using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watergun : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _firing = false;

    //mode where ray interactor is enabled and bubbles are disabled
    private bool _menuMode = false;

    private ParticleSystem _particles = null;

    [SerializeField] private GameObject _goop = null;
    [SerializeField] private float _maxGoop = 3.0f;
    private float _goopLevel = 0f;

    private void Start()
    {
        _particles = GetComponentInChildren<ParticleSystem>();
    }

    public void StartFiring()
    {
        _firing = true;
    }

    public void StopFiring()
    {
        _firing = false;
    }

    public void SetMenuModeActive(bool active)
    { 
        _menuMode = active; 
    }

    private void Update()
    {
        if( _firing )
        {
            _particles.Play();

            _goopLevel = Mathf.Clamp(_goopLevel - Time.deltaTime, 0f, _maxGoop);

            if (_goopLevel <= 0f)
                _particles.Stop();

            //TODO make sure sfx is playing, stretch: change pitch based on tank fullness
            //also haptics
            //also gradually empty tank and update renderer
            //also apply recoil with haptics

            //todo if empty, play empty sound, maybe fizzle effect
        }
        else 
        {

            _particles.Stop();

            _goopLevel = Mathf.Clamp(_goopLevel + Time.deltaTime, 0f, _maxGoop);


            //todo stretch, play refilling sound while filling
            //todo play sound when tank is full

            //if not, then gradually recharge tank and update renderer
        }

        var goopSize = Vector3.one;
        goopSize.y = _goopLevel / _maxGoop;
        _goop.transform.localScale = goopSize;
    }
}
