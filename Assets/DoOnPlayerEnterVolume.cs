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
            onPlayerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            onPlayerExit.Invoke();
        }
    }
}
