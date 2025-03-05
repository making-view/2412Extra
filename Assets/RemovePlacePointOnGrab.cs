using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovePlacePointOnGrab : MonoBehaviour
{
    PlacePoint _placePoint;
    GameObject _visual;
    void Start()
    {
        _visual = GetComponentInChildren<MeshRenderer>().gameObject;
        _placePoint = GetComponent<PlacePoint>();
        //_placePoint.GetGrabbable().onGrab.AddListener((hand) => StartCoroutine(RemovePlacePoint(hand)));
        _placePoint.OnRemove.AddListener((placepoint, grabbable) => StartCoroutine(RemovePlacePoint()));
    
    }

    IEnumerator RemovePlacePoint()
    {
        _placePoint.ignoreMe = true;
        yield return null;
        _visual.SetActive(false);
    }
}
