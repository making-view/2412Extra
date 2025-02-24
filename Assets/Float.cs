using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : MonoBehaviour
{
    [SerializeField] private float  _verticalRange = 1.0f;
    [SerializeField] private float  _spin = 10.0f;
    [SerializeField] private float  _speed = 1.0f;
    private float _offset;

    Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        _offset = Random.Range(0, 2 * Mathf.PI);
        position = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float newY = position.y + Mathf.Sin(Time.timeSinceLevelLoad * _speed + _offset) * _verticalRange;
        
        if(_spin != 0)
        {
            float newRot = transform.rotation.eulerAngles.y + _spin * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, newRot, 0);
        }

        transform.localPosition = new Vector3(position.x, newY, position.z);
    }
}
