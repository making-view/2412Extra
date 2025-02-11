using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessTrigger : MonoBehaviour
{
    private Mess _myMess;


    void Start()
    {
        _myMess = GetComponentInParent<Mess>();
    }

    private void OnParticleCollision(GameObject other)
    {
        //Debug.Log(other + " particle entered on layer " + LayerMask.LayerToName(other.gameObject.layer) + " with size: " + other.transform.localScale.magnitude);
        _myMess.RegisterHit();
    }
}
