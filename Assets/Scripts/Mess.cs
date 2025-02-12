using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Mess : MonoBehaviour
{
    [Serializable]
    private enum TransitionType
    {
        size,
        material,
        missingStuff,
    }

    [SerializeField] TransitionType _type = TransitionType.size;

    private GameObject _confetti;
    private GameObject _collider;
    [SerializeField] private Transform _messTransorm;
    private Vector3 _startingScale;
    private Vector3 _targetScale;
    [SerializeField] float _hitsToClear = 30;
    private AudioSource _audioSource;
    float _maxHits;

    private const float _startingPitch = 1.0f;
    private const float _endingPitch = 2.0f;

    private bool _done = false;

    private void Start()
    {
        if(_messTransorm == null)
            _messTransorm = GetComponentInChildren<MeshRenderer>().transform;

        _startingScale = _messTransorm.localScale;
        //get starting scale and scale from that

        _confetti = GetComponentInChildren<ParticleSystem>(true).gameObject;
        _collider = GetComponentInChildren<Collider>(true).gameObject;
        _audioSource = GetComponent<AudioSource>();
        _maxHits = _hitsToClear;

        if(_type == TransitionType.material)
        {
            //TODO get materials from renderers and store in list
        }
    }

    public void RegisterHit()
    {
        if (_hitsToClear <= 0)
            return;

        _hitsToClear--;

        switch (_type)
        {
            case TransitionType.size: //Give disappearing object size a little bump when hit
                float bumpMagnitude = 0.03f * _startingScale.magnitude;
                _messTransorm.localScale = Vector3.MoveTowards(_messTransorm.localScale, _targetScale * 1.2f, bumpMagnitude);
                break;
            case TransitionType.material: //TODO give material a bump towards white color when hit
                
                break;
            case TransitionType.missingStuff: //TODO give appearing objects a bump in size when hit

                break;
        }


        //make a sound that scales in pitch when hit
        if (_audioSource != null && _audioSource.clip != null)
        {
            float progress = 1 - Mathf.Clamp01(_hitsToClear / _maxHits);
            float pitch = Mathf.Lerp(_startingPitch, _endingPitch, progress);
            _audioSource.pitch = pitch;
            _audioSource.Play();
        }

        if (_hitsToClear == 0)
        {
            Debug.Log(this + " is done");
            _done = true;
            _hitsToClear = 0;

            //play particle effect w sound when disappearing
            _confetti.SetActive(true);
            _collider.SetActive(false);

            switch (_type)
            {
                case TransitionType.size:
                    _messTransorm.localScale = Vector3.zero; //hide mesh
                    break;
                case TransitionType.material:
                    //make good material fully opaque
                    break;
                case TransitionType.missingStuff:
                    //add the last stuff missing
                    break;
            }
            MessHandler.instance.MessCleaned(this);
        }
    }

    private void Update()
    {
        if (_done)
            return;

        switch (_type)
        {
            case TransitionType.size:
                HandleSizeUpdate();
                break;
            case TransitionType.material:
                //step towards correct material
                break;
            case TransitionType.missingStuff:
                //step towards fully appearing transform(s)
                break;
        }
    }

    private void HandleSizeUpdate()
    {
        float progress = Mathf.Clamp01(_hitsToClear / _maxHits);
        _targetScale = Mathf.SmoothStep(0f, 1.0f, progress) * _startingScale;
        _messTransorm.localScale = Vector3.MoveTowards(_messTransorm.localScale, _targetScale, Time.deltaTime * 3f * _startingScale.magnitude);
    }
}
