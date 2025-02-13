using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public CanvasGroup _screenFader;
    // Start is called before the first frame update

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(true);
        }
        else
            Destroy(this);

        if (_screenFader == null)
            _screenFader = GetComponentInChildren<CanvasGroup>();
    }
}
