using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessHighlighter : MonoBehaviour
{
    private Light _lightSource;
    private float _startingIntensity;
    private float _time = 0f;
    private GameObject _fairy;
    private Transform _target;

    [SerializeField] private Transform _testTarget;

    [SerializeField] [Range(0.1f, 5.0f)] private float _blinkingSpeed = 2.0f;
    [SerializeField] private float _moveTime = 3.0f;
    [SerializeField] private float _hoverDistance = 1.0f;

    void Start()
    {
        _lightSource= GetComponentInChildren<Light>();
        _startingIntensity= _lightSource.intensity;
        _fairy = transform.GetChild(0).gameObject;

        //if (_testTarget != null)
        //    StartMovingTowardsTarget(_testTarget);

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _time = 0f;
        //fade in audiosource
    }

    // Update is called once per frame
    void Update()
    {
        _lightSource.intensity= (Mathf.Cos(_time * _blinkingSpeed) + 1f) / 2f * _startingIntensity;
        _time += Time.deltaTime;
        _fairy.transform.LookAt(PlayerManager.instance._screenFader.transform.position);

        if (_target != null)
            _lightSource.transform.LookAt(_target);
    }

    public void StartMovingTowardsTarget(Transform target)
    {
        StopAllCoroutines();
        StartCoroutine(MoveTowardsTarget(target));
    }

    private IEnumerator MoveTowardsTarget(Transform target)
    {
        _target = target;
        float moveTimer = _moveTime;
        Vector3 startingPosition = transform.position;
        Vector3 targetPosition = target.position + Vector3.up * _hoverDistance;

        Vector3 toPlayer = (targetPosition - PlayerManager.instance._screenFader.transform.position).normalized;
        toPlayer.y = 0;
        //maybe also place the angel a bit in the direction of the player to highlight posters better
        targetPosition += toPlayer * 0.2f;

        while (moveTimer > 0f)
        {
            float progress = 1 - (moveTimer / _moveTime);
            float smoothProgress = Mathf.SmoothStep(0, 1, progress);
            //TODO smooth step
            transform.position = Vector3.Slerp(startingPosition, targetPosition, smoothProgress);
            moveTimer -= Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
}
