using Autohand;
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
    private Rigidbody _rigidbody = null;
    private Grabbable _grabbable = null;
    [SerializeField] private GameObject _goop = null;

    [SerializeField] private float _maxGoop = 3.0f;
    [SerializeField] private float _recoilStrength = 1.0f;
    private float _goopLevel = 0f;

    private float _timePerBubble = 0f;
    private float _firingTime = 0f;

    [SerializeField] private List<AudioClip> _shootingSFX = new List<AudioClip>();
    [SerializeField] private AudioSource _shootingAudioSource;

    private List<Hand> _grabbingHands = new List<Hand>();


    private void Start()
    {
        _particles = GetComponentInChildren<ParticleSystem>();
        _timePerBubble = 1.0f / _particles.emission.rateOverTime.constantMax;
        _grabbable = GetComponentInChildren<Grabbable>();
        _rigidbody = GetComponentInChildren<Rigidbody>();

        //keep track of all hands grabbing gun
        foreach(Grabbable grabbable in GetComponentsInChildren<Grabbable>())
        {
            grabbable.onGrab.AddListener((hand, grabbalbe) => {_grabbingHands.Add(hand);});
            grabbable.onRelease.AddListener((hand, grabbalbe) => {_grabbingHands.Remove(hand);});
        }
    }

    public void StartFiring()
    {
        _firing = true;
        _particles.Play();
    }

    public void StopFiring()
    {
        _firing = false;
        _particles.Stop();
    }

    public void SetMenuModeActive(bool active)
    { 
        _menuMode = active; 
    }

    private void Update()
    {
        if( _firing )
        {
            _goopLevel = Mathf.Clamp(_goopLevel - Time.deltaTime, 0f, _maxGoop);

            if (_goopLevel <= 0f)
                _particles.Stop();
            else
            {
                _firingTime += Time.deltaTime;
                int bubblesToFire = (int)Mathf.Floor(_firingTime / _timePerBubble);
                
                if( bubblesToFire > 0 )
                {
                    FireBubbles(bubblesToFire);
                }
            }

            //TODO make sure sfx is playing, stretch: change pitch based on tank fullness
            //stretch make gun fire progressivly faster, empty tank based on bubbles spawned instead of deltatime
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
        goopSize.y = Mathf.Clamp(_goopLevel / _maxGoop, 0.01f, 1.0f);
        _goop.transform.localScale = goopSize;
    }

    private void FireBubbles(int bubblesToFire)
    {
        int sfxint = Random.Range(0, _shootingSFX.Count);
        float sfxpitch = Random.Range(0.8f, 1.2f);
        _shootingAudioSource.pitch = sfxpitch;
        _shootingAudioSource.PlayOneShot(_shootingSFX[sfxint]);

        _firingTime = _firingTime - (_timePerBubble * bubblesToFire);
        _particles.Emit(bubblesToFire);
        Vector3 backRecoil = -_particles.transform.forward * _recoilStrength * bubblesToFire / 10f;
        Vector3 upRecoil = _particles.transform.up * _recoilStrength * bubblesToFire;

        _rigidbody.AddForceAtPosition(backRecoil, _particles.transform.position, ForceMode.Impulse);
        _rigidbody.AddForceAtPosition(upRecoil, _particles.transform.position, ForceMode.Impulse);

        foreach (Hand hand in _grabbingHands)
        {
            //Debug.Log("Sending haptic to " + hand.name);
            hand.PlayHapticVibration(_timePerBubble / 2f, 0.5f);
        }
    }
}
