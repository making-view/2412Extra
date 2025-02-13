using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    [SerializeField] private Transform _messTransform;
    [SerializeField] public Transform _popupPosition;
    [SerializeField] float _hitsToClear = 30f;
    [SerializeField] float _progressThreshold = 0.8f;
    private AudioSource _audioSource;
    float _maxHits;

    //scale type specific
    private Vector3 _startingScale;
    private Vector3 _targetScale;

    //material type specific
    private List<Material> _materials = new List<Material>();

    //missing stuff specific 
    private List<GameObject> _missingObjects = new List<GameObject>();

    private const float _startingPitch = 1.0f;
    private const float _endingPitch = 2.0f;

    private bool _done = false;
    private bool _initialzied = false;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if(_initialzied) return;

        if (_messTransform == null)
            _messTransform = GetComponentInChildren<MeshRenderer>().transform;

        if (_popupPosition == null)
            _popupPosition = GetComponentInChildren<MeshRenderer>().transform;

        _collider = GetComponentInChildren<MessTrigger>(true).gameObject;
        _startingScale = _messTransform.localScale;
        //get starting scale and scale from that
        _confetti = GetComponentInChildren<ParticleSystem>(true).gameObject;
        _audioSource = GetComponent<AudioSource>();
        _maxHits = _hitsToClear;

        if (_type == TransitionType.material)
        {
            foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
                _materials.Add(meshRenderer.material);
        }
        else if (_type == TransitionType.missingStuff)
        {
            _hitsToClear = _messTransform.childCount; //shuold we override this? If not then change onhit of missing type

            for (int i = 0; i < _messTransform.childCount; i++)
                _missingObjects.Add(_messTransform.GetChild(i).gameObject);
        }

        _initialzied = true;
    }

    public void EnableMess(bool active)
    {
        Initialize();

        _done = !active;
        _collider.SetActive(active);

        if (!active)
        {
            switch (_type)
            {
                case TransitionType.size:
                    _messTransform.localScale = Vector3.zero; //hide mesh
                    break;
                case TransitionType.material:
                    foreach (Material material in _materials) //make good material fully opaque
                        material.SetFloat("_Badness", 0f);
                    break;
                case TransitionType.missingStuff:
                    foreach (GameObject stuff in _missingObjects)
                        stuff.SetActive(true);
                    break;
            }
        }
        else
        {
            switch (_type)
            {
                case TransitionType.size:
                    _messTransform.localScale = _startingScale; //show mesh
                    break;
                case TransitionType.material:
                    foreach (Material material in _materials) //make bad material fully opaque
                        material.SetFloat("_Badness", 1f);
                    break;
                case TransitionType.missingStuff:
                    foreach (GameObject stuff in _missingObjects) //hide objects
                        stuff.SetActive(false);
                    break;
            }
        }
    }

    public void RegisterHit()
    {
        if (_hitsToClear <= 0)
            return;

        _hitsToClear--;
        float progress = 1f - Mathf.Clamp01(_hitsToClear / _maxHits);

        switch (_type)
        {
            case TransitionType.size: //Give disappearing object size a little bump when hit
                float bumpMagnitude = 0.03f * _startingScale.magnitude;
                _messTransform.localScale = Vector3.MoveTowards(_messTransform.localScale, _targetScale * 1.2f, bumpMagnitude);
                break;
            case TransitionType.material: //TODO maybe give material a bump towards white color when hit
                
                break;
            case TransitionType.missingStuff: //TODO maybe give appearing objects a bump in size when hit
                _missingObjects[Mathf.FloorToInt(_hitsToClear - 1)].SetActive(true);
                break;
        }

        
        //make a sound that scales in pitch when hit
        if (_audioSource != null && _audioSource.clip != null)
        {
            float pitch = Mathf.Lerp(_startingPitch, _endingPitch, progress);
            _audioSource.pitch = pitch;
            _audioSource.Play();
        }

        if (_hitsToClear == 0 || progress > _progressThreshold)
        {
            Debug.Log(this + " is done");
            _done = true;
            _hitsToClear = 0;
            MessHandler.instance.MessCleaned(this);

            //play particle effect w sound when disappearing
            _confetti.SetActive(true);
            _collider.SetActive(false);

            switch (_type)
            {
                case TransitionType.size:
                    _messTransform.localScale = Vector3.zero; //hide mesh
                    break;
                case TransitionType.material:
                    foreach(Material material in _materials) //make good material fully opaque
                        material.SetFloat("_Badness", 0f);
                    break;
                case TransitionType.missingStuff:
                    foreach(GameObject stuff in _missingObjects)
                        stuff.SetActive(true);
                    break;
            }
        }
    }

    private void Update()
    {
        if (_done)
            return;

        //from 1 at start, to 0 at end
        float progress = Mathf.Clamp01(_hitsToClear / _maxHits);
        float smoothProgress = Mathf.SmoothStep(0f, 1.0f, progress);

        switch (_type)
        {
            case TransitionType.size: //change size towards target based on progress
                _targetScale = smoothProgress * _startingScale;
                _messTransform.localScale = Vector3.MoveTowards(_messTransform.localScale, _targetScale, Time.deltaTime * 3f * _startingScale.magnitude);
                break;
            case TransitionType.material: //change color towards good based on progress
                foreach (Material material in _materials) //make good material more opaque
                    material.SetFloat("_Badness", smoothProgress);
                break;
            case TransitionType.missingStuff:
                //step towards fully appearing transform(s)
                break;
        }
    }
}
