using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public CanvasGroup _screenFader;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);

        if (_screenFader == null)
            _screenFader = GetComponentInChildren<Camera>().GetComponentInChildren<CanvasGroup>();
    }
}
