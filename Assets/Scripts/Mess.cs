using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mess : MonoBehaviour
{
    private GameObject _confetti;
    private GameObject _collider;
    [SerializeField] private Transform _messTransorm;
    private Vector3 _startingScale;
    [SerializeField] float _hitsToClear = 30;
    float _maxHits;

    private bool _done = false;

    private void Start()
    {
        if(_messTransorm == null)
            _messTransorm = GetComponentInChildren<MeshRenderer>().transform;

        _startingScale = _messTransorm.localScale;
        //get starting scale and scale from that

        _confetti = GetComponentInChildren<ParticleSystem>(true).gameObject;
        _collider = GetComponentInChildren<Collider>(true).gameObject;
        _maxHits = _hitsToClear;
    }

    public void RegisterHit()
    {
        if (_hitsToClear <= 0)
            return;

        _hitsToClear--;

        if (_hitsToClear == 0)
        {
            Debug.Log(this + " is done");
            _hitsToClear = 0;

            //play particle effect w sound when disappearing
            _confetti.SetActive(true);
            _collider.SetActive(false);
            //todo send message to messHandler 
        }

        float progress = Mathf.Clamp01(_hitsToClear / _maxHits);
        //TODO play audio and maybe have a particle effect when shrinking
        _messTransorm.localScale = Mathf.SmoothStep(0f, 1.0f, progress) * _startingScale;
    }
}
