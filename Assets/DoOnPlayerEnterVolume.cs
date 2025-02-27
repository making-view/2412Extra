using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoOnPlayerEnterVolume : MonoBehaviour
{
    public UnityEvent onPlayerEnter;
    public bool travelOnEnter;
    public UnityEvent onPlayerExit;
    public bool travelOnExit;

    [SerializeField] private string travelToScene = "";

    float _cooldown = -1.0f;

    private void OnEnable()
    {
        if(travelToScene != "" && travelOnEnter)
        {
            onPlayerEnter.AddListener(() => { SceneTransitioner.instance.StartTransitionToScene(travelToScene); });
        }

        if (travelToScene != "" && travelOnExit)
        {
            onPlayerExit.AddListener(() => { SceneTransitioner.instance.StartTransitionToScene(travelToScene); });
        }
    }

    private void OnDisable()
    {
        if (travelToScene != "" && travelOnEnter)
        {
            onPlayerEnter.RemoveListener(() => { SceneTransitioner.instance.StartTransitionToScene(travelToScene); });
        }

        if (travelToScene != "" && travelOnExit)
        {
            onPlayerExit.RemoveListener(() => { SceneTransitioner.instance.StartTransitionToScene(travelToScene); });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(_cooldown <= 0f)
            {
                onPlayerEnter.Invoke();
                StartCoroutine(HandleCooldown(1f));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player exiting with cooldown " + _cooldown);

            if (_cooldown <= 0f)
            {
                onPlayerExit.Invoke();
                StartCoroutine(HandleCooldown(1f));
            }
        }
    }

    private IEnumerator HandleCooldown(float cooldown)
    {
        _cooldown = cooldown;

        while(_cooldown >= 0f)
        {
            yield return null;
            _cooldown -= Time.deltaTime;
        }
    }
}
