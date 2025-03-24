using Autohand;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    // Start is called before the first frame update
    public static SceneTransitioner instance;
    [SerializeField] [Range(0.0f, 4.0f)] public float _fadeTime = 1.0f;

    public UnityEvent onLoadScene = new UnityEvent();
    public UnityEvent onLoadScenePersistent = new UnityEvent();
    public UnityEvent onSceneDoneLoadingPersistent = new UnityEvent();

    //Set to true when exiting main scene so that next time it loads near game and video stations
    private bool _shouldStartAtEntrance = true;


    private void Start()
    {
        StartCoroutine(MovePlayerWithFade(false));
        SetupTeleportTutorial();
    }


    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);
    }

    public void StartTransitionToScene(string sceneName)
    {
        StartCoroutine(TransitionToScene(sceneName));
    }

    IEnumerator TransitionToScene(string sceneName)
    {
        Debug.Log(this + ": going to scene " + sceneName);

        yield return StartCoroutine(Fade(0.0f, 1.0f));
        //load new scene and fire events
        onLoadScene.Invoke();
        onLoadScenePersistent.Invoke();
        yield return null;
        onLoadScene.RemoveAllListeners();
        SceneManager.LoadScene(sceneName);

        StartCoroutine(MovePlayerWithFade(false));

        yield return StartCoroutine(Fade(1.0f, 0.0f));
        onSceneDoneLoadingPersistent.Invoke();


    SetupTeleportTutorial();
    }

    private void SetupTeleportTutorial()
    {
        foreach (TutorialCanvas tutorial in GameObject.FindObjectsOfType<TutorialCanvas>(true))
        {
            if (tutorial.GetTutorialType() == TutorialCanvas.TutorialType.teleportButton)
            {
                tutorial.gameObject.SetActive(true);
                tutorial.onTutorialCompleted.AddListener(() => _shouldStartAtEntrance = false);
            }
        }
    }

    public void StartMovePlayerWithFade(bool fadeIn)
    {
        StartCoroutine(MovePlayerWithFade(fadeIn));
    }

    IEnumerator MovePlayerWithFade(bool fadeIn)
    {
        PlayerManager.instance._screenFader.alpha = 1.0f;

        if(fadeIn)
            yield return StartCoroutine(Fade(0.0f, 1.0f));
        else
            yield return null;

        MovePlayerToStartingPosition();
        yield return StartCoroutine(Fade(1.0f, 0.0f));
    }

    IEnumerator Fade(float from, float to)
    {
        float timer = 0.0f;
        float halfFade = _fadeTime / 2.0f;
        PlayerManager.instance._screenFader.alpha = from;

        while (timer < halfFade)
        {
            float progress = Mathf.SmoothStep(from, to, timer / halfFade);
            PlayerManager.instance._screenFader.alpha = progress;
            timer += Time.deltaTime;
            yield return null;
        }
        PlayerManager.instance._screenFader.alpha = to;
    }

    public void MovePlayerToStartingPosition()
    {
        //if game has not been restarted or paused, check for a resturn position
        //if found, go to this instead 

        Transform startingPosition = GameObject.FindGameObjectWithTag("ReturningPosition")?.transform;

        if(startingPosition == null || _shouldStartAtEntrance)
            startingPosition = GameObject.FindGameObjectWithTag("StartingPosition")?.transform;

        if(startingPosition == null)
        {
            Debug.LogError(this + " No starting position found");
            return;
        }

        Debug.Log(this + " Found starting pos. Moving player");
        
        Vector3 targetPos = startingPosition.position;
        Quaternion targetRot = startingPosition.rotation;

        MovePlayerTo(targetPos, targetRot);
    }

    public void MovePlayerTo(Vector3 targetPos, Quaternion targetRot)
    {
        var trackingContainer = PlayerManager.instance.GetComponentInChildren<AutoHandPlayer>().trackingContainer;
        var cameraTrans = PlayerManager.instance.GetComponentInChildren<Camera>().transform;

        trackingContainer.transform.rotation = Quaternion.Euler(0, trackingContainer.transform.eulerAngles.y + targetRot.eulerAngles.y - cameraTrans.eulerAngles.y, 0);
        PlayerManager.instance.GetComponentInChildren<AutoHandPlayer>().SetPosition(targetPos);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) //on pause, mark player to start at entrance and reset tutorials done
        {
            _shouldStartAtEntrance = true;
            PlayerPrefs.SetInt("TeleportTutorialDone", 0);
            PlayerPrefs.SetInt("GunTutorialsDone", 0);
        }
        else      //on unpause, move player to entrance
        {
            //reload scene if we're in the game scene
            string sceneName = SceneManager.GetActiveScene().name;

            Debug.Log(this + " Unapusing in scene " + sceneName);

            StopAllCoroutines();

            if (sceneName == "EXTRA_Interior")
                StartTransitionToScene(sceneName);
            else
                StartCoroutine(MovePlayerWithFade(false));
        }
    }

    //void OnApplicationFocus(bool focus)
    //{
    //    if (!focus) //on pause, mark player to start at entrance and reset tutorials done
    //    {
    //        _shouldStartAtEntrance = true;
    //        PlayerPrefs.SetInt("TeleportTutorialDone", 0);
    //        PlayerPrefs.SetInt("GunTutorialsDone", 0);
    //    }
    //    else      //on unpause, move player to entrance
    //    {
    //        //reload scene if we're in the game scene
    //        string sceneName = SceneManager.GetActiveScene().name;

    //        Debug.Log(this + " Unapusing in scene " + sceneName);

    //        StopAllCoroutines();

    //        if (sceneName == "EXTRA_Interior")
    //            StartTransitionToScene(sceneName);
    //        else
    //            StartCoroutine(MovePlayerDelayed());
    //    }
    //}
}
