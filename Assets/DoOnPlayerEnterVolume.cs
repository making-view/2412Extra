using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoOnPlayerEnterVolume : MonoBehaviour
{
    public UnityEvent onPlayerEnter;
    [SerializeField] private string travelToScene = "";


    private void OnEnable()
    {
        if(travelToScene != "")
        {
            onPlayerEnter.AddListener(() => { SceneTransitioner.instance.StartTransitionToScene(travelToScene); });
        }
    }

    private void OnDisable()
    {
        if (travelToScene != "")
        {
            onPlayerEnter.RemoveListener(() => { SceneTransitioner.instance.StartTransitionToScene(travelToScene); });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            onPlayerEnter.Invoke();
        }
    
    
    }
}
