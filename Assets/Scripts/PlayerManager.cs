using System.Collections;
using System.Collections.Generic;
using Autohand;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public CanvasGroup _screenFader;
    // Start is called before the first frame update

    private bool _initialized = false;

    private void OnEnable()
    {
        Initialize();
    }


    private void OnLevelWasLoaded(int level)
    {
        Debug.Log("on level loaded");

        foreach (Autohand.Hand hand in GetComponentsInChildren<Autohand.Hand>(true))
        {
            hand.collisionTracker.CleanUp();
            hand.handAnimator.ClearPoseArea();
            hand.handAnimator.CancelPose();
        }
    }

    private void Initialize()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }

        if (_initialized)
            return;

        instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(true);


        if (_screenFader == null)
            _screenFader = GetComponentInChildren<CanvasGroup>();

        _initialized = true;
    }
}
